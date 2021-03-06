﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TinyCms.Core;
using TinyCms.Core.Caching;
using TinyCms.Core.Domain.Customers;
using TinyCms.Core.Domain.Polls;
using TinyCms.Services.Localization;
using TinyCms.Services.Polls;
using TinyCms.Web.Infrastructure.Cache;
using TinyCms.Web.Models.Polls;

namespace TinyCms.Web.Controllers
{
    public class PollController : BasePublicController
    {
        #region Constructors

        public PollController(ILocalizationService localizationService,
            IWorkContext workContext,
            IPollService pollService,
            ICacheManager cacheManager)
        {
            _localizationService = localizationService;
            _workContext = workContext;
            _pollService = pollService;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual PollModel PreparePollModel(Poll poll, bool? setAlreadyVotedProperty)
        {
            var model = new PollModel
            {
                Id = poll.Id,
                AlreadyVoted =
                    setAlreadyVotedProperty == null
                        ? _pollService.AlreadyVoted(poll.Id, _workContext.CurrentCustomer.Id)
                        : setAlreadyVotedProperty.Value,
                Name = poll.Name
            };
            var answers = poll.PollAnswers.OrderBy(x => x.DisplayOrder);
            foreach (var answer in answers)
                model.TotalVotes += answer.NumberOfVotes;
            foreach (var pa in answers)
            {
                model.Answers.Add(new PollAnswerModel
                {
                    Id = pa.Id,
                    Name = pa.Name,
                    NumberOfVotes = pa.NumberOfVotes,
                    PercentOfTotalVotes =
                        model.TotalVotes > 0
                            ? ((Convert.ToDouble(pa.NumberOfVotes)/Convert.ToDouble(model.TotalVotes))*
                               Convert.ToDouble(100))
                            : 0
                });
            }

            return model;
        }

        #endregion

        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IPollService _pollService;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Methods

        [ChildActionOnly]
        public ActionResult PollBlock(string systemKeyword, bool showResult = false)
        {
            if (String.IsNullOrWhiteSpace(systemKeyword))
                return Content("");

            var cacheKey = string.Format(ModelCacheEventConsumer.POLL_BY_SYSTEMNAME__MODEL_KEY, systemKeyword,
                _workContext.WorkingLanguage.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var poll = _pollService.GetPollBySystemKeyword(systemKeyword, _workContext.WorkingLanguage.Id);
                if (poll == null ||
                    !poll.Published ||
                    (poll.StartDateUtc.HasValue && poll.StartDateUtc.Value > DateTime.UtcNow) ||
                    (poll.EndDateUtc.HasValue && poll.EndDateUtc.Value < DateTime.UtcNow))
                    //we do not cache nulls. that's why let's return an empty record (ID = 0)
                    return new PollModel {Id = 0};

                return PreparePollModel(poll, false);
            });
            if (cachedModel == null || cachedModel.Id == 0)
                return Content("");

            //"AlreadyVoted" property of "PollModel" object depends on the current customer. Let's update it.
            //But first we need to clone the cached model (the updated one should not be cached)
            var model = (PollModel) cachedModel.Clone();
            model.AlreadyVoted = showResult
                ? true
                : _pollService.AlreadyVoted(model.Id, _workContext.CurrentCustomer.Id);

            return PartialView(model);
        }


        [HttpPost]
        public ActionResult VoteResult(int pollAnswerId)
        {
            var pollAnswer = _pollService.GetPollAnswerById(pollAnswerId);
            if (pollAnswer == null)
                return Json(new
                {
                    error = "No poll answer found with the specified id"
                });

            var poll = pollAnswer.Poll;

            return Json(new
            {
                html = RenderPartialViewToString("_Poll", PreparePollModel(poll, true))
            });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Vote(int pollAnswerId)
        {
            var pollAnswer = _pollService.GetPollAnswerById(pollAnswerId);
            if (pollAnswer == null)
                return Json(new
                {
                    error = "No poll answer found with the specified id"
                });

            var poll = pollAnswer.Poll;
            if (!poll.Published)
                return Json(new
                {
                    error = "Poll is not available"
                });

            if (_workContext.CurrentCustomer.IsGuest() && !poll.AllowGuestsToVote)
                return Json(new
                {
                    error = _localizationService.GetResource("Polls.OnlyRegisteredUsersVote")
                });

            var alreadyVoted = _pollService.AlreadyVoted(poll.Id, _workContext.CurrentCustomer.Id);
            if (!alreadyVoted)
            {
                //vote
                pollAnswer.PollVotingRecords.Add(new PollVotingRecord
                {
                    PollAnswerId = pollAnswer.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    CreatedOnUtc = DateTime.UtcNow
                });
                //update totals
                pollAnswer.NumberOfVotes = pollAnswer.PollVotingRecords.Count;
                _pollService.UpdatePoll(poll);
            }

            return Json(new
            {
                html = RenderPartialViewToString("_Poll", PreparePollModel(poll, null))
            });
        }

        [ChildActionOnly]
        public ActionResult HomePagePolls()
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.HOMEPAGE_POLLS_MODEL_KEY,
                _workContext.WorkingLanguage.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
                _pollService.GetPolls(_workContext.WorkingLanguage.Id, true)
                    .Select(x => PreparePollModel(x, false))
                    .ToList());
            //"AlreadyVoted" property of "PollModel" object depends on the current customer. Let's update it.
            //But first we need to clone the cached model (the updated one should not be cached)
            var model = new List<PollModel>();
            foreach (var p in cachedModel)
            {
                var pollModel = (PollModel) p.Clone();
                pollModel.AlreadyVoted = _pollService.AlreadyVoted(pollModel.Id, _workContext.CurrentCustomer.Id);
                model.Add(pollModel);
            }

            if (model.Count == 0)
                Content("");

            return PartialView(model);
        }


        [ChildActionOnly]
        public ActionResult RandomPolls()
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.HOMEPAGE_POLLS_MODEL_KEY,
                _workContext.WorkingLanguage.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
                _pollService.GetPolls(_workContext.WorkingLanguage.Id, true)
                    .Select(x => PreparePollModel(x, false))
                    .ToList());
            //"AlreadyVoted" property of "PollModel" object depends on the current customer. Let's update it.
            //But first we need to clone the cached model (the updated one should not be cached)
            var model = new List<PollModel>();
            foreach (var p in cachedModel)
            {
                var pollModel = (PollModel) p.Clone();
                pollModel.AlreadyVoted = _pollService.AlreadyVoted(pollModel.Id, _workContext.CurrentCustomer.Id);
                model.Add(pollModel);
            }

            if (model.Count == 0)
                Content("");

            return PartialView(model);
        }

        #endregion
    }
}