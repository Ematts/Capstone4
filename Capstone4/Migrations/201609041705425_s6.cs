namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class s6 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Contractors", "conAddress");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Contractors", "conAddress", c => c.String());
        }
    }
}
