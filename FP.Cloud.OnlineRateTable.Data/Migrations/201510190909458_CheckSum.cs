namespace FP.Cloud.OnlineRateTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CheckSum : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RateTableFiles", "Checksum", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RateTableFiles", "Checksum");
        }
    }
}
