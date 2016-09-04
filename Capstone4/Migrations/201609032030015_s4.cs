namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class s4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contractors", "conAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contractors", "conAddress");
        }
    }
}
