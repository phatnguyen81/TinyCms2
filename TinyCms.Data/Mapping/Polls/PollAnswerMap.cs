using TinyCms.Core.Domain.Polls;

namespace TinyCms.Data.Mapping.Polls
{
    public class PollAnswerMap : NopEntityTypeConfiguration<PollAnswer>
    {
        public PollAnswerMap()
        {
            ToTable("PollAnswer");
            HasKey(pa => pa.Id);
            Property(pa => pa.Name).IsRequired();

            HasRequired(pa => pa.Poll)
                .WithMany(p => p.PollAnswers)
                .HasForeignKey(pa => pa.PollId).WillCascadeOnDelete(true);
        }
    }
}