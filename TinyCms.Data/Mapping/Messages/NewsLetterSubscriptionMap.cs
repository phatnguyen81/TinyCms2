using TinyCms.Core.Domain.Messages;

namespace TinyCms.Data.Mapping.Messages
{
    public class NewsLetterSubscriptionMap : NopEntityTypeConfiguration<NewsLetterSubscription>
    {
        public NewsLetterSubscriptionMap()
        {
            ToTable("NewsLetterSubscription");
            HasKey(nls => nls.Id);

            Property(nls => nls.Email).IsRequired().HasMaxLength(255);
        }
    }
}