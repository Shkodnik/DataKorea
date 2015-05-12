namespace Korea.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class import : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnalogsForImports",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Сode1 = c.String(nullable: false, maxLength: 16),
                        Сode2 = c.String(nullable: false, maxLength: 16),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Сode1)
                .Index(t => t.Сode2);
            
            CreateTable(
                "dbo.BrandForImports",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Abbreviation = c.String(nullable: false),
                        ExtId = c.Int(nullable: false),
                        Title = c.String(nullable: false),
                        Weight = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ExtId);
            
            CreateTable(
                "dbo.CategoryForImports",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CategoryId = c.Guid(),
                        OuterKey = c.String(maxLength: 16),
                        ExtId = c.Int(nullable: false),
                        Title = c.String(nullable: false),
                        Weight = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CategoryForImports", t => t.CategoryId)
                .Index(t => t.CategoryId)
                .Index(t => t.OuterKey)
                .Index(t => t.ExtId);
            
            CreateTable(
                "dbo.GenerationForImports",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ModelId = c.Guid(nullable: false),
                        Title = c.String(nullable: false),
                        Weight = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ModelForImports", t => t.ModelId, cascadeDelete: true)
                .Index(t => t.ModelId);
            
            CreateTable(
                "dbo.ModelForImports",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BrandId = c.Guid(nullable: false),
                        ExtId = c.Int(nullable: false),
                        Title = c.String(nullable: false),
                        Weight = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BrandForImports", t => t.BrandId, cascadeDelete: true)
                .Index(t => t.BrandId)
                .Index(t => t.ExtId);
            
            CreateTable(
                "dbo.ProductForImports",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 256),
                        Сode = c.String(nullable: false, maxLength: 16),
                        CategoryId = c.Guid(nullable: false),
                        Description = c.String(),
                        SupplierId = c.Guid(),
                        Tale = c.Int(nullable: false),
                        Cost = c.Double(nullable: false),
                        ExtId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CategoryForImports", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.SupplierForImports", t => t.SupplierId)
                .Index(t => t.Name)
                .Index(t => t.Сode)
                .Index(t => t.CategoryId)
                .Index(t => t.SupplierId)
                .Index(t => t.ExtId);
            
            CreateTable(
                "dbo.SupplierForImports",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(nullable: false),
                        Weight = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.YearForImports",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                        Title = c.String(nullable: false),
                        Weight = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductForImports", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.ProductForImportGenerationForImports",
                c => new
                    {
                        ProductForImport_Id = c.Guid(nullable: false),
                        GenerationForImport_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProductForImport_Id, t.GenerationForImport_Id })
                .ForeignKey("dbo.ProductForImports", t => t.ProductForImport_Id, cascadeDelete: true)
                .ForeignKey("dbo.GenerationForImports", t => t.GenerationForImport_Id, cascadeDelete: true)
                .Index(t => t.ProductForImport_Id)
                .Index(t => t.GenerationForImport_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.YearForImports", "ProductId", "dbo.ProductForImports");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ProductForImports", "SupplierId", "dbo.SupplierForImports");
            DropForeignKey("dbo.ProductForImportGenerationForImports", "GenerationForImport_Id", "dbo.GenerationForImports");
            DropForeignKey("dbo.ProductForImportGenerationForImports", "ProductForImport_Id", "dbo.ProductForImports");
            DropForeignKey("dbo.ProductForImports", "CategoryId", "dbo.CategoryForImports");
            DropForeignKey("dbo.GenerationForImports", "ModelId", "dbo.ModelForImports");
            DropForeignKey("dbo.ModelForImports", "BrandId", "dbo.BrandForImports");
            DropForeignKey("dbo.CategoryForImports", "CategoryId", "dbo.CategoryForImports");
            DropIndex("dbo.ProductForImportGenerationForImports", new[] { "GenerationForImport_Id" });
            DropIndex("dbo.ProductForImportGenerationForImports", new[] { "ProductForImport_Id" });
            DropIndex("dbo.YearForImports", new[] { "ProductId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.ProductForImports", new[] { "ExtId" });
            DropIndex("dbo.ProductForImports", new[] { "SupplierId" });
            DropIndex("dbo.ProductForImports", new[] { "CategoryId" });
            DropIndex("dbo.ProductForImports", new[] { "Сode" });
            DropIndex("dbo.ProductForImports", new[] { "Name" });
            DropIndex("dbo.ModelForImports", new[] { "ExtId" });
            DropIndex("dbo.ModelForImports", new[] { "BrandId" });
            DropIndex("dbo.GenerationForImports", new[] { "ModelId" });
            DropIndex("dbo.CategoryForImports", new[] { "ExtId" });
            DropIndex("dbo.CategoryForImports", new[] { "OuterKey" });
            DropIndex("dbo.CategoryForImports", new[] { "CategoryId" });
            DropIndex("dbo.BrandForImports", new[] { "ExtId" });
            DropIndex("dbo.AnalogsForImports", new[] { "Сode2" });
            DropIndex("dbo.AnalogsForImports", new[] { "Сode1" });
            DropTable("dbo.ProductForImportGenerationForImports");
            DropTable("dbo.YearForImports");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.SupplierForImports");
            DropTable("dbo.ProductForImports");
            DropTable("dbo.ModelForImports");
            DropTable("dbo.GenerationForImports");
            DropTable("dbo.CategoryForImports");
            DropTable("dbo.BrandForImports");
            DropTable("dbo.AnalogsForImports");
        }
    }
}
