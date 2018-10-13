namespace RestBite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration331 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Markers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 20),
                        Address = c.String(nullable: false, maxLength: 40),
                        lat = c.Single(nullable: false),
                        lng = c.Single(nullable: false),
                        type = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Markers");
        }
    }
}
