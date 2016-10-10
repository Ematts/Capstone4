namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class d11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayPalListenerModels", "_PayPalCheckoutInfo_TrxnDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PayPalListenerModels", "_PayPalCheckoutInfo_TrxnDate");
        }
    }
}
