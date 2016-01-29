using TinyCms.Core.Domain.Messages;

namespace TinyCms.Data.Mapping.Messages
{
    public class EmailAccountMap : NopEntityTypeConfiguration<EmailAccount>
    {
        public EmailAccountMap()
        {
            ToTable("EmailAccount");
            HasKey(ea => ea.Id);

            Property(ea => ea.Email).IsRequired().HasMaxLength(255);
            Property(ea => ea.DisplayName).HasMaxLength(255);
            Property(ea => ea.Host).IsRequired().HasMaxLength(255);
            Property(ea => ea.Username).IsRequired().HasMaxLength(255);
            Property(ea => ea.Password).IsRequired().HasMaxLength(255);

            Ignore(ea => ea.FriendlyName);
        }
    }
}