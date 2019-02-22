namespace GigHub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mergeDeleteGig : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Gigs", "IsCanceled");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Gigs", "IsCanceled", c => c.Boolean(nullable: false));
        }
    }
}
