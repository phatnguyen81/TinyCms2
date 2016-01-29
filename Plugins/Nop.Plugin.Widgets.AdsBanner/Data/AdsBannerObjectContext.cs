using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using TinyCms.Core;
using TinyCms.Data;
using TinyCms.Data.Mapping.Media;

namespace Nop.Plugin.Widgets.AdsBanner.Data
{
    public class AdsBannerObjectContext : DbContext, IDbContext
    {
        public AdsBannerObjectContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            Database.SetInitializer<AdsBannerObjectContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AdsBannerRecordMap());
            modelBuilder.Configurations.Add(new PictureMap());
            base.OnModelCreating(modelBuilder);
        }

        public string CreateDatabaseInstallationScript()
        {
            return ((IObjectContextAdapter) this).ObjectContext.CreateDatabaseScript();
        }

        public void Install()
        {
            Database.SetInitializer<AdsBannerObjectContext>(null);
            const string dbScript = @"CREATE TABLE [AdsBannerRecord](
	                [Id] [int] IDENTITY(1,1) NOT NULL,
	                [Name] [nvarchar](max) NOT NULL,
	                [PictureId] [int] NOT NULL,
	                [WidgetZoneId] [int] NOT NULL,
	                [Url] [nvarchar](max) NULL,
	                [FromDateUtc] [datetime] NULL,
	                [ToDateUtc] [datetime] NULL,
	                [Published] [bit] NOT NULL,
                    [DisplayOrder] [int] NOT NULL,
                    PRIMARY KEY CLUSTERED 
                    (
	                    [Id] ASC
                    )
                );
                ALTER TABLE [AdsBannerRecord]  WITH CHECK ADD  CONSTRAINT [AdsBannerRecord_Picture] FOREIGN KEY([PictureId])
                REFERENCES [Picture] ([Id])
                ON DELETE CASCADE;
                ALTER TABLE [AdsBannerRecord] CHECK CONSTRAINT [AdsBannerRecord_Picture];
            ";
            //Database.ExecuteSqlCommand(CreateDatabaseInstallationScript());
            Database.ExecuteSqlCommand(dbScript);
            SaveChanges();
        }

        public void Uninstall()
        {
            const string dbScript = "DROP TABLE ADSBANNERRECORD";
            Database.ExecuteSqlCommand(dbScript);
            SaveChanges();
        }

        #region IDbContext Implementations

        public IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters)
            where TEntity : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null,
            params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void Detach(object entity)
        {
            throw new NotImplementedException();
        }

        public bool ProxyCreationEnabled { get; set; }
        public bool AutoDetectChangesEnabled { get; set; }

        #endregion
    }
}