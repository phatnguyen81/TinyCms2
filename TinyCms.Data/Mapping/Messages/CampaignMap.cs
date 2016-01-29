using TinyCms.Core.Domain.Messages;

namespace TinyCms.Data.Mapping.Messages
{
    public class CampaignMap : NopEntityTypeConfiguration<Campaign>
    {
        public CampaignMap()
        {
            ToTable("Campaign");
            HasKey(ea => ea.Id);

            Property(ea => ea.Name).IsRequired();
            Property(ea => ea.Subject).IsRequired();
            Property(ea => ea.Body).IsRequired();
        }
    }
}