namespace Korea.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class import4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PositionForImports", "Weight", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PositionForImports", "Weight");
        }
    }
}
