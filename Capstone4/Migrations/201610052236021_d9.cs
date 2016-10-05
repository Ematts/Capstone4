namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class d9 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PayPalListenerModels",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        _PayPalCheckoutInfo_receiver_email = c.String(),
                        _PayPalCheckoutInfo_receiver_id = c.String(),
                        _PayPalCheckoutInfo_txn_id = c.String(),
                        _PayPalCheckoutInfo_txn_type = c.String(),
                        _PayPalCheckoutInfo_verify_sign = c.String(),
                        _PayPalCheckoutInfo_address_country = c.String(),
                        _PayPalCheckoutInfo_address_city = c.String(),
                        _PayPalCheckoutInfo_address_country_code = c.String(),
                        _PayPalCheckoutInfo_address_name = c.String(),
                        _PayPalCheckoutInfo_address_state = c.String(),
                        _PayPalCheckoutInfo_address_status = c.String(),
                        _PayPalCheckoutInfo_address_street = c.String(),
                        _PayPalCheckoutInfo_address_zip = c.String(),
                        _PayPalCheckoutInfo_contact_phone = c.String(),
                        _PayPalCheckoutInfo_first_name = c.String(),
                        _PayPalCheckoutInfo_last_name = c.String(),
                        _PayPalCheckoutInfo_payer_email = c.String(),
                        _PayPalCheckoutInfo_payer_id = c.String(),
                        _PayPalCheckoutInfo_token = c.String(),
                        _PayPalCheckoutInfo_invoice = c.String(),
                        _PayPalCheckoutInfo_item_name1 = c.String(),
                        _PayPalCheckoutInfo_item_number1 = c.String(),
                        _PayPalCheckoutInfo_mc_currency = c.String(),
                        _PayPalCheckoutInfo_mc_fee = c.String(),
                        _PayPalCheckoutInfo_mc_gross = c.String(),
                        _PayPalCheckoutInfo_payer_status = c.String(),
                        _PayPalCheckoutInfo_payment_date = c.String(),
                        _PayPalCheckoutInfo_memo = c.String(),
                        _PayPalCheckoutInfo_payment_status = c.String(),
                        _PayPalCheckoutInfo_payment_type = c.String(),
                        _PayPalCheckoutInfo_pending_reason = c.String(),
                        _PayPalCheckoutInfo_protection_eligibility = c.String(),
                        _PayPalCheckoutInfo_quantity = c.String(),
                        _PayPalCheckoutInfo_reason_code = c.String(),
                        _PayPalCheckoutInfo_correlationID = c.String(),
                        _PayPalCheckoutInfo_ack = c.String(),
                        _PayPalCheckoutInfo_errmsg = c.String(),
                        _PayPalCheckoutInfo_errcode = c.Int(),
                        _PayPalCheckoutInfo_custom = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.ServiceRequests", "PayPalListenerModelID", c => c.Int());
            CreateIndex("dbo.ServiceRequests", "PayPalListenerModelID");
            AddForeignKey("dbo.ServiceRequests", "PayPalListenerModelID", "dbo.PayPalListenerModels", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceRequests", "PayPalListenerModelID", "dbo.PayPalListenerModels");
            DropIndex("dbo.ServiceRequests", new[] { "PayPalListenerModelID" });
            DropColumn("dbo.ServiceRequests", "PayPalListenerModelID");
            DropTable("dbo.PayPalListenerModels");
        }
    }
}
