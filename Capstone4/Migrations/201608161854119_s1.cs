namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class s1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Contractors", "AddressID", "dbo.Addresses");
            DropIndex("dbo.Contractors", new[] { "AddressID" });
            AlterColumn("dbo.Contractors", "AddressID", c => c.Int());
            CreateIndex("dbo.Contractors", "AddressID");
            AddForeignKey("dbo.Contractors", "AddressID", "dbo.Addresses", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Contractors", "AddressID", "dbo.Addresses");
            DropIndex("dbo.Contractors", new[] { "AddressID" });
            AlterColumn("dbo.Contractors", "AddressID", c => c.Int(nullable: false));
            CreateIndex("dbo.Contractors", "AddressID");
            AddForeignKey("dbo.Contractors", "AddressID", "dbo.Addresses", "ID", cascadeDelete: true);
        }
    }
}
