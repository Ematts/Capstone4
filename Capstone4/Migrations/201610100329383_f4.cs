namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class f4 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.PayPalListenerModels", "_PayPalCheckoutInfo_TrxnDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PayPalListenerModels", "_PayPalCheckoutInfo_TrxnDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
    }
}
