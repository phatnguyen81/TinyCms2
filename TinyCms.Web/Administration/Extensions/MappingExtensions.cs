using System;
using AutoMapper;
using TinyCms.Admin.Models.Customers;
using TinyCms.Admin.Models.Localization;
using TinyCms.Admin.Models.Logging;
using TinyCms.Admin.Models.Messages;
using TinyCms.Admin.Models.Polls;
using TinyCms.Admin.Models.Settings;
using TinyCms.Core.Domain.Customers;
using TinyCms.Core.Domain.Localization;
using TinyCms.Core.Domain.Logging;
using TinyCms.Core.Domain.Messages;
using TinyCms.Core.Domain.Polls;

namespace TinyCms.Admin.Extensions
{
    public static class MappingExtensions
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }
       

        #region Customer attributes

        //attributes
        public static CustomerAttributeModel ToModel(this CustomerAttribute entity)
        {
            return entity.MapTo<CustomerAttribute, CustomerAttributeModel>();
        }

        public static CustomerAttribute ToEntity(this CustomerAttributeModel model)
        {
            return model.MapTo<CustomerAttributeModel, CustomerAttribute>();
        }

        public static CustomerAttribute ToEntity(this CustomerAttributeModel model, CustomerAttribute destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Languages

        public static LanguageModel ToModel(this Language entity)
        {
            return entity.MapTo<Language, LanguageModel>();
        }

        public static Language ToEntity(this LanguageModel model)
        {
            return model.MapTo<LanguageModel, Language>();
        }

        public static Language ToEntity(this LanguageModel model, Language destination)
        {
            return model.MapTo(destination);
        }
        
        #endregion

        #region Email account

        public static EmailAccountModel ToModel(this EmailAccount entity)
        {
            return entity.MapTo<EmailAccount, EmailAccountModel>();
        }

        public static EmailAccount ToEntity(this EmailAccountModel model)
        {
            return model.MapTo<EmailAccountModel, EmailAccount>();
        }

        public static EmailAccount ToEntity(this EmailAccountModel model, EmailAccount destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Message templates

        public static MessageTemplateModel ToModel(this MessageTemplate entity)
        {
            return entity.MapTo<MessageTemplate, MessageTemplateModel>();
        }

        public static MessageTemplate ToEntity(this MessageTemplateModel model)
        {
            return model.MapTo<MessageTemplateModel, MessageTemplate>();
        }

        public static MessageTemplate ToEntity(this MessageTemplateModel model, MessageTemplate destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Queued email

        public static QueuedEmailModel ToModel(this QueuedEmail entity)
        {
            return entity.MapTo<QueuedEmail, QueuedEmailModel>();
        }

        public static QueuedEmail ToEntity(this QueuedEmailModel model)
        {
            return model.MapTo<QueuedEmailModel, QueuedEmail>();
        }

        public static QueuedEmail ToEntity(this QueuedEmailModel model, QueuedEmail destination)
        {
            return model.MapTo(destination);
        }

        #endregion
       
        #region Log

        public static LogModel ToModel(this Log entity)
        {
            return entity.MapTo<Log, LogModel>();
        }

        public static Log ToEntity(this LogModel model)
        {
            return model.MapTo<LogModel, Log>();
        }

        public static Log ToEntity(this LogModel model, Log destination)
        {
            return model.MapTo(destination);
        }

        public static ActivityLogTypeModel ToModel(this ActivityLogType entity)
        {
            return entity.MapTo<ActivityLogType, ActivityLogTypeModel>();
        }

        public static ActivityLogModel ToModel(this ActivityLog entity)
        {
            return entity.MapTo<ActivityLog, ActivityLogModel>();
        }

        #endregion
     
        #region NewsLetter subscriptions

        public static NewsLetterSubscriptionModel ToModel(this NewsLetterSubscription entity)
        {
            return entity.MapTo<NewsLetterSubscription, NewsLetterSubscriptionModel>();
        }

        public static NewsLetterSubscription ToEntity(this NewsLetterSubscriptionModel model)
        {
            return model.MapTo<NewsLetterSubscriptionModel, NewsLetterSubscription>();
        }

        public static NewsLetterSubscription ToEntity(this NewsLetterSubscriptionModel model, NewsLetterSubscription destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Polls

        //news items
        public static PollModel ToModel(this Poll entity)
        {
            return entity.MapTo<Poll, PollModel>();
        }

        public static Poll ToEntity(this PollModel model)
        {
            return model.MapTo<PollModel, Poll>();
        }

        public static Poll ToEntity(this PollModel model, Poll destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Customer roles

        //customer roles
        public static CustomerRoleModel ToModel(this CustomerRole entity)
        {
            return entity.MapTo<CustomerRole, CustomerRoleModel>();
        }

        public static CustomerRole ToEntity(this CustomerRoleModel model)
        {
            return model.MapTo<CustomerRoleModel, CustomerRole>();
        }

        public static CustomerRole ToEntity(this CustomerRoleModel model, CustomerRole destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Settings

        public static CustomerSettings ToEntity(this CustomerUserSettingsModel.CustomerSettingsModel model, CustomerSettings destination)
        {
            return model.MapTo(destination);
        }
      
        #endregion

     

    }
}