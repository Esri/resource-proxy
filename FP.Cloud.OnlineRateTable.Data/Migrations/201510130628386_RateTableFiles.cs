namespace FP.Cloud.OnlineRateTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RateTableFiles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RateTableFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RateTableId = c.Int(nullable: false),
                        FileName = c.String(),
                        FileType = c.Int(nullable: false),
                        FileData = c.Binary(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RateTables", t => t.RateTableId, cascadeDelete: true)
                .Index(t => t.RateTableId);
            
            AddColumn("dbo.RateTables", "Variant", c => c.String());
            AddColumn("dbo.RateTables", "VersionNumber", c => c.String());
            AddColumn("dbo.RateTables", "CarrierDetails", c => c.Int(nullable: false));
            DropColumn("dbo.RateTables", "CountryIso3Code");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RateTables", "CountryIso3Code", c => c.String());
            DropForeignKey("dbo.RateTableFiles", "RateTableId", "dbo.RateTables");
            DropIndex("dbo.RateTableFiles", new[] { "RateTableId" });
            DropColumn("dbo.RateTables", "CarrierDetails");
            DropColumn("dbo.RateTables", "VersionNumber");
            DropColumn("dbo.RateTables", "Variant");
            DropTable("dbo.RateTableFiles");
        }
    }
}
