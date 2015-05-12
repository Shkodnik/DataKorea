namespace Korea.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class import51 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CodeSnippets",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Info = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        FinishedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CodeSnippets");
        }
    }
}
