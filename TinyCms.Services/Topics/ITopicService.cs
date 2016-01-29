using System.Collections.Generic;
using TinyCms.Core.Domain.Topics;

namespace TinyCms.Services.Topics
{
    /// <summary>
    ///     Topic service interface
    /// </summary>
    public interface ITopicService
    {
        /// <summary>
        ///     Deletes a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        void DeleteTopic(Topic topic);

        /// <summary>
        ///     Gets a topic
        /// </summary>
        /// <param name="topicId">The topic identifier</param>
        /// <returns>Topic</returns>
        Topic GetTopicById(int topicId);

        /// <summary>
        ///     Gets a topic
        /// </summary>
        /// <param name="systemName">The topic system name</param>
        /// <param name="storeId">Store identifier; pass 0 to ignore filtering by store and load the first one</param>
        /// <returns>Topic</returns>
        Topic GetTopicBySystemName(string systemName);

        /// <summary>
        ///     Gets all topics
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="ignorAcl">A value indicating whether to ignore ACL rules</param>
        /// <returns>Topics</returns>
        IList<Topic> GetAllTopics(bool ignorAcl = false);

        /// <summary>
        ///     Inserts a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        void InsertTopic(Topic topic);

        /// <summary>
        ///     Updates the topic
        /// </summary>
        /// <param name="topic">Topic</param>
        void UpdateTopic(Topic topic);
    }
}