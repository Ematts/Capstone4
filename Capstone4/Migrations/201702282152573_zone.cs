namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class zone : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayPalListenerModels", "_PayPalCheckoutInfo_Timezone", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PayPalListenerModels", "_PayPalCheckoutInfo_Timezone");
        }
    }
}
