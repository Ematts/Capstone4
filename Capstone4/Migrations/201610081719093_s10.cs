namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class s10 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PayPalListenerModels", "_PayPalCheckoutInfo_TrxnDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PayPalListenerModels", "_PayPalCheckoutInfo_TrxnDate", c => c.DateTime(nullable: false));
        }
    }
}
