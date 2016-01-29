using TinyCms.Core.Domain.Common;

namespace TinyCms.Data.Mapping.Common
{
    public class SearchTermMap : NopEntityTypeConfiguration<SearchTerm>
    {
        public SearchTermMap()
        {
            ToTable("SearchTerm");
            HasKey(st => st.Id);
        }
    }
}