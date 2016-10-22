namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Addresses", "vacant", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Addresses", "vacant", c => c.Boolean(nullable: false));
        }
    }
}
