namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class f : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Contractors", "Rating", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Contractors", "Rating", c => c.Double(nullable: false));
        }
    }
}
