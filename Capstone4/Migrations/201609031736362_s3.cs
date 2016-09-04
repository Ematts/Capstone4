namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class s3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contractors", "travelDistance", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contractors", "travelDistance");
        }
    }
}
