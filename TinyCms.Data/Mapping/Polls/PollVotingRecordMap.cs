using TinyCms.Core.Domain.Polls;

namespace TinyCms.Data.Mapping.Polls
{
    public class PollVotingRecordMap : NopEntityTypeConfiguration<PollVotingRecord>
    {
        public PollVotingRecordMap()
        {
            ToTable("PollVotingRecord");
            HasKey(pr => pr.Id);

            HasRequired(pvr => pvr.PollAnswer)
                .WithMany(pa => pa.PollVotingRecords)
                .HasForeignKey(pvr => pvr.PollAnswerId);

            HasRequired(cc => cc.Customer)
                .WithMany()
                .HasForeignKey(cc => cc.CustomerId);
        }
    }
}