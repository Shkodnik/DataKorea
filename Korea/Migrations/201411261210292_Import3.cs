namespace Korea.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Import3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.YearForImports", "ProductId", "dbo.ProductForImports");
            DropIndex("dbo.YearForImports", new[] { "ProductId" });
            CreateTable(
                "dbo.PositionForImports",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PositionForImportProductForImports",
                c => new
                    {
                        PositionForImport_Id = c.Guid(nullable: false),
                        ProductForImport_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.PositionForImport_Id, t.ProductForImport_Id })
                .ForeignKey("dbo.PositionForImports", t => t.PositionForImport_Id, cascadeDelete: true)
                .ForeignKey("dbo.ProductForImports", t => t.ProductForImport_Id, cascadeDelete: true)
                .Index(t => t.PositionForImport_Id)
                .Index(t => t.ProductForImport_Id);
            
            DropTable("dbo.YearForImports");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.YearForImports",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                        Title = c.String(nullable: false),
                        Weight = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.PositionForImportProductForImports", "ProductForImport_Id", "dbo.ProductForImports");
            DropForeignKey("dbo.PositionForImportProductForImports", "PositionForImport_Id", "dbo.PositionForImports");
            DropIndex("dbo.PositionForImportProductForImports", new[] { "ProductForImport_Id" });
            DropIndex("dbo.PositionForImportProductForImports", new[] { "PositionForImport_Id" });
            DropTable("dbo.PositionForImportProductForImports");
            DropTable("dbo.PositionForImports");
            CreateIndex("dbo.YearForImports", "ProductId");
            AddForeignKey("dbo.YearForImports", "ProductId", "dbo.ProductForImports", "Id", cascadeDelete: true);
        }
    }
}
