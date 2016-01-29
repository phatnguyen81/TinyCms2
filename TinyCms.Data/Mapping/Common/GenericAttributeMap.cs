using TinyCms.Core.Domain.Common;

namespace TinyCms.Data.Mapping.Common
{
    public class GenericAttributeMap : NopEntityTypeConfiguration<GenericAttribute>
    {
        public GenericAttributeMap()
        {
            ToTable("GenericAttribute");
            HasKey(ga => ga.Id);

            Property(ga => ga.KeyGroup).IsRequired().HasMaxLength(400);
            Property(ga => ga.Key).IsRequired().HasMaxLength(400);
            Property(ga => ga.Value).IsRequired();
        }
    }
}