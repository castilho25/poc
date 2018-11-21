namespace SFC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DB_Alunos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Alunos", "DataDeRegisto", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Alunos", "DataDeRegisto");
        }
    }
}
