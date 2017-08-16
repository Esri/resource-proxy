namespace FP.Cloud.OnlineRateTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RateTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RateTables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CountryIso3Code = c.String(),
                        CarrierId = c.Int(nullable: false),
                        ValidFrom = c.DateTime(nullable: false),
                        Culture = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RateTables");
        }
    }
}
