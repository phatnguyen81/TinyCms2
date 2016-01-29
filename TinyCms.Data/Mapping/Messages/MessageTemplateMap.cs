using TinyCms.Core.Domain.Messages;

namespace TinyCms.Data.Mapping.Messages
{
    public class MessageTemplateMap : NopEntityTypeConfiguration<MessageTemplate>
    {
        public MessageTemplateMap()
        {
            ToTable("MessageTemplate");
            HasKey(mt => mt.Id);

            Property(mt => mt.Name).IsRequired().HasMaxLength(200);
            Property(mt => mt.BccEmailAddresses).HasMaxLength(200);
            Property(mt => mt.Subject).HasMaxLength(1000);
            Property(mt => mt.EmailAccountId).IsRequired();
        }
    }
}