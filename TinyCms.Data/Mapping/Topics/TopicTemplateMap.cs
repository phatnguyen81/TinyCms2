using TinyCms.Core.Domain.Topics;

namespace TinyCms.Data.Mapping.Topics
{
    public class TopicTemplateMap : NopEntityTypeConfiguration<TopicTemplate>
    {
        public TopicTemplateMap()
        {
            ToTable("TopicTemplate");
            HasKey(t => t.Id);
            Property(t => t.Name).IsRequired().HasMaxLength(400);
            Property(t => t.ViewPath).IsRequired().HasMaxLength(400);
        }
    }
}