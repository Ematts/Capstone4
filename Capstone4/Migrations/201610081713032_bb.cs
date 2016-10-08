namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bb : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayPalListenerModels", "_PayPalCheckoutInfo_TrxnDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PayPalListenerModels", "_PayPalCheckoutInfo_TrxnDate");
        }
    }
}
