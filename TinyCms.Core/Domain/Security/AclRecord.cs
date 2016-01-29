using TinyCms.Core.Domain.Customers;

namespace TinyCms.Core.Domain.Security
{
    /// <summary>
    ///     Represents an ACL record
    /// </summary>
    public class AclRecord : BaseEntity
    {
        /// <summary>
        ///     Gets or sets the entity identifier
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        ///     Gets or sets the entity name
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        ///     Gets or sets the customer role identifier
        /// </summary>
        public int CustomerRoleId { get; set; }

        /// <summary>
        ///     Gets or sets the customer role
        /// </summary>
        public virtual CustomerRole CustomerRole { get; set; }
    }
}