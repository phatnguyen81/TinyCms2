using TinyCms.Core.Domain.Polls;

namespace TinyCms.Data.Mapping.Polls
{
    public class PollMap : NopEntityTypeConfiguration<Poll>
    {
        public PollMap()
        {
            ToTable("Poll");
            HasKey(p => p.Id);
            Property(p => p.Name).IsRequired();

            HasRequired(p => p.Language)
                .WithMany()
                .HasForeignKey(p => p.LanguageId).WillCascadeOnDelete(true);
        }
    }
}