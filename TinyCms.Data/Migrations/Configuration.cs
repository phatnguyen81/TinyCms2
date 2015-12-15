using System.Data.Entity.Infrastructure;

namespace TinyCms.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<TinyCms.Data.NopObjectContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(TinyCms.Data.NopObjectContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
    public class MyContextFactory : IDbContextFactory<NopObjectContext>
    {
        public NopObjectContext Create()
        {
            return new NopObjectContext("Data Source=.\\SQLEXPRESS2K8;Initial Catalog=TinyCms;Integrated Security=False;Persist Security Info=False;User ID=sa;Password=Admin@123");
        }
    }
}
