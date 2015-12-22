using System;
using System.Collections.Generic;
using TinyCms.Core.Domain.Localization;
using TinyCms.Core.Domain.Media;
using TinyCms.Core.Domain.Security;
using TinyCms.Core.Domain.Seo;

namespace TinyCms.Core.Domain.Posts
{
    /// <summary>
    /// Represents a product
    /// </summary>
    public partial class Post : BaseEntity, ILocalizedEntity, ISlugSupported, IAclSupported
    {
        private ICollection<PostCategory> _postCategories;
        private ICollection<PostPicture> _postPictures;
        private ICollection<PostTag> _postTags;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the short description
        /// </summary>
        public string ShortDescription { get; set; }
        /// <summary>
        /// Gets or sets the full description
        /// </summary>
        public string FullDescription { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        
        /// <summary>
        /// Gets or sets a value indicating whether to show the product on home page
        /// </summary>
        public bool ShowOnHomePage { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public string MetaKeywords { get; set; }
        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public string MetaDescription { get; set; }
        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public string MetaTitle { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether the entity is subject to ACL
        /// </summary>
        public bool SubjectToAcl { get; set; }


     
        /// <summary>
        /// Gets or sets a display order.
        /// This value is used when sorting associated products (used with "grouped" products)
        /// This value is used when sorting home page products
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ApprovedBy { get; set; }


        /// <summary>
        /// Gets or sets the date and time of product creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
        /// <summary>
        /// Gets or sets the date and time of product update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? ApprovedOnUtc { get; set; }
        

        /// <summary>
        /// Gets or sets the collection of PostCategory
        /// </summary>
        public virtual ICollection<PostCategory> PostCategories
        {
            get { return _postCategories ?? (_postCategories = new List<PostCategory>()); }
            protected set { _postCategories = value; }
        }

        /// <summary>
        /// Gets or sets the collection of PostPicture
        /// </summary>
        public virtual ICollection<PostPicture> PostPictures
        {
            get { return _postPictures ?? (_postPictures = new List<PostPicture>()); }
            protected set { _postPictures = value; }
        }


        /// <summary>
        /// Gets or sets the product tags
        /// </summary>
        public virtual ICollection<PostTag> PostTags
        {
            get { return _postTags ?? (_postTags = new List<PostTag>()); }
            protected set { _postTags = value; }
        }

    }
}