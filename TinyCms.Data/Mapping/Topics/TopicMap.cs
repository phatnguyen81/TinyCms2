using TinyCms.Core.Domain.Topics;

namespace TinyCms.Data.Mapping.Topics
{
    public class TopicMap : NopEntityTypeConfiguration<Topic>
    {
        public TopicMap()
        {
            ToTable("Topic");
            HasKey(t => t.Id);
        }
    }
}