namespace SFC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DB_Mensagens : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Mensagens",
                c => new
                    {
                        MensagemID = c.Guid(nullable: false, identity: true),
                        SenderID = c.Guid(nullable: false),
                        ReceiverID = c.Guid(nullable: false),
                        Assunto = c.String(nullable: false),
                        Mensagem = c.String(nullable: false),
                        DataEnvio = c.DateTime(nullable: false),
                        Vista = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.MensagemID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Mensagens");
        }
    }
}
