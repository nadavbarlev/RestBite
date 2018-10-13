namespace RestBite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration33 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Gender = c.Int(nullable: false),
                        ClientName = c.String(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Password = c.String(nullable: false),
                        isAdmin = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ClientID = c.Int(nullable: false),
                        PostID = c.Int(nullable: false),
                        Content = c.String(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Clients", t => t.ClientID)
                .ForeignKey("dbo.Posts", t => t.PostID)
                .Index(t => t.ClientID)
                .Index(t => t.PostID);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ClientID = c.Int(nullable: false),
                        GenreID = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 20),
                        Content = c.String(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Clients", t => t.ClientID)
                .ForeignKey("dbo.Genres", t => t.GenreID)
                .Index(t => t.ClientID)
                .Index(t => t.GenreID);
            
            CreateTable(
                "dbo.Genres",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "PostID", "dbo.Posts");
            DropForeignKey("dbo.Posts", "GenreID", "dbo.Genres");
            DropForeignKey("dbo.Posts", "ClientID", "dbo.Clients");
            DropForeignKey("dbo.Comments", "ClientID", "dbo.Clients");
            DropIndex("dbo.Posts", new[] { "GenreID" });
            DropIndex("dbo.Posts", new[] { "ClientID" });
            DropIndex("dbo.Comments", new[] { "PostID" });
            DropIndex("dbo.Comments", new[] { "ClientID" });
            DropTable("dbo.Genres");
            DropTable("dbo.Posts");
            DropTable("dbo.Comments");
            DropTable("dbo.Clients");
        }
    }
}
