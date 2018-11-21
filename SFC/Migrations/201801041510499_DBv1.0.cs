namespace SFC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DBv10 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Alunos",
                c => new
                    {
                        AlunosID = c.Guid(nullable: false),
                        InstituicaoID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.AlunosID)
                .ForeignKey("dbo.Filhos", t => t.AlunosID)
                .ForeignKey("dbo.Instituicao", t => t.InstituicaoID, cascadeDelete: true)
                .Index(t => t.AlunosID)
                .Index(t => t.InstituicaoID);
            
            CreateTable(
                "dbo.Filhos",
                c => new
                    {
                        FilhoID = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CivilID = c.String(nullable: false),
                        DataNascimento = c.String(nullable: false),
                        Genero = c.Int(nullable: false),
                        EstadoCivil = c.String(nullable: false),
                        Nacionalidade = c.String(nullable: false),
                        Concelho = c.String(nullable: false),
                        Freguesia = c.String(nullable: false),
                        Endereço = c.String(nullable: false),
                        PaisID = c.Guid(nullable: false),
                        Instituicao_InstituicaoID = c.Guid(),
                    })
                .PrimaryKey(t => t.FilhoID)
                .ForeignKey("dbo.Instituicao", t => t.Instituicao_InstituicaoID)
                .ForeignKey("dbo.Pai", t => t.PaisID, cascadeDelete: true)
                .Index(t => t.PaisID)
                .Index(t => t.Instituicao_InstituicaoID);
            
            CreateTable(
                "dbo.Instituicao",
                c => new
                    {
                        InstituicaoID = c.Guid(nullable: false, identity: true),
                        Nome = c.String(nullable: false),
                        Concelho = c.String(nullable: false),
                        Freguesia = c.String(nullable: false),
                        Endereco = c.String(nullable: false),
                        TipoInstituicao = c.Int(nullable: false),
                        Aproved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.InstituicaoID);
            
            CreateTable(
                "dbo.Avaliacao",
                c => new
                    {
                        AvaliacaoID = c.Guid(nullable: false),
                        InstituicaoID = c.Guid(nullable: false),
                        PaiID = c.Guid(nullable: false),
                        FilhoID = c.Guid(nullable: false),
                        Nota = c.Int(nullable: false),
                        Critica = c.String(),
                    })
                .PrimaryKey(t => t.AvaliacaoID)
                .ForeignKey("dbo.Filhos", t => t.FilhoID, cascadeDelete: true)
                .ForeignKey("dbo.Instituicao", t => t.InstituicaoID, cascadeDelete: true)
                .ForeignKey("dbo.Pai", t => t.PaiID, cascadeDelete: false)
                .Index(t => t.InstituicaoID)
                .Index(t => t.PaiID)
                .Index(t => t.FilhoID);
            
            CreateTable(
                "dbo.Pai",
                c => new
                    {
                        PaisID = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CivilID = c.String(nullable: false),
                        DataNascimento = c.String(nullable: false),
                        Nacionalidade = c.String(nullable: false),
                        Profissao = c.String(nullable: false),
                        Contacto = c.String(nullable: false),
                        Endereco = c.String(nullable: false),
                        Genero = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PaisID);
            
            CreateTable(
                "dbo.Detalhe",
                c => new
                    {
                        DetalheID = c.Guid(nullable: false),
                        Missao = c.String(),
                        Visao = c.String(),
                        Historia = c.String(),
                    })
                .PrimaryKey(t => t.DetalheID)
                .ForeignKey("dbo.Instituicao", t => t.DetalheID)
                .Index(t => t.DetalheID);
            
            CreateTable(
                "dbo.Atividades",
                c => new
                    {
                        AtividadeID = c.Guid(nullable: false, identity: true),
                        Nome = c.String(nullable: false),
                        Descricao = c.String(nullable: false),
                        InicioAtividade = c.DateTime(nullable: false),
                        FimAtividade = c.DateTime(nullable: false),
                        DetalheID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.AtividadeID)
                .ForeignKey("dbo.Detalhe", t => t.DetalheID, cascadeDelete: true)
                .Index(t => t.DetalheID);
            
            CreateTable(
                "dbo.RegistoFilhos",
                c => new
                    {
                        RegistoID = c.Guid(nullable: false),
                        FilhoID = c.Guid(nullable: false),
                        InstituicaoID = c.Guid(nullable: false),
                        Aproved = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RegistoID)
                .ForeignKey("dbo.Filhos", t => t.FilhoID, cascadeDelete: true)
                .ForeignKey("dbo.Instituicao", t => t.InstituicaoID, cascadeDelete: true)
                .Index(t => t.FilhoID)
                .Index(t => t.InstituicaoID);
            
            CreateTable(
                "dbo.TipoEnsino",
                c => new
                    {
                        TipoEnsinoID = c.Guid(nullable: false, identity: true),
                        Nome = c.String(nullable: false),
                        InstituicaoID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.TipoEnsinoID)
                .ForeignKey("dbo.Instituicao", t => t.InstituicaoID, cascadeDelete: true)
                .Index(t => t.InstituicaoID);
            
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
                        IdPai = c.Guid(nullable: false),
                        IdInstituicao = c.Guid(nullable: false),
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Alunos", "InstituicaoID", "dbo.Instituicao");
            DropForeignKey("dbo.Alunos", "AlunosID", "dbo.Filhos");
            DropForeignKey("dbo.Filhos", "PaisID", "dbo.Pai");
            DropForeignKey("dbo.Filhos", "Instituicao_InstituicaoID", "dbo.Instituicao");
            DropForeignKey("dbo.TipoEnsino", "InstituicaoID", "dbo.Instituicao");
            DropForeignKey("dbo.RegistoFilhos", "InstituicaoID", "dbo.Instituicao");
            DropForeignKey("dbo.RegistoFilhos", "FilhoID", "dbo.Filhos");
            DropForeignKey("dbo.Detalhe", "DetalheID", "dbo.Instituicao");
            DropForeignKey("dbo.Atividades", "DetalheID", "dbo.Detalhe");
            DropForeignKey("dbo.Avaliacao", "PaiID", "dbo.Pai");
            DropForeignKey("dbo.Avaliacao", "InstituicaoID", "dbo.Instituicao");
            DropForeignKey("dbo.Avaliacao", "FilhoID", "dbo.Filhos");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.TipoEnsino", new[] { "InstituicaoID" });
            DropIndex("dbo.RegistoFilhos", new[] { "InstituicaoID" });
            DropIndex("dbo.RegistoFilhos", new[] { "FilhoID" });
            DropIndex("dbo.Atividades", new[] { "DetalheID" });
            DropIndex("dbo.Detalhe", new[] { "DetalheID" });
            DropIndex("dbo.Avaliacao", new[] { "FilhoID" });
            DropIndex("dbo.Avaliacao", new[] { "PaiID" });
            DropIndex("dbo.Avaliacao", new[] { "InstituicaoID" });
            DropIndex("dbo.Filhos", new[] { "Instituicao_InstituicaoID" });
            DropIndex("dbo.Filhos", new[] { "PaisID" });
            DropIndex("dbo.Alunos", new[] { "InstituicaoID" });
            DropIndex("dbo.Alunos", new[] { "AlunosID" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.TipoEnsino");
            DropTable("dbo.RegistoFilhos");
            DropTable("dbo.Atividades");
            DropTable("dbo.Detalhe");
            DropTable("dbo.Pai");
            DropTable("dbo.Avaliacao");
            DropTable("dbo.Instituicao");
            DropTable("dbo.Filhos");
            DropTable("dbo.Alunos");
        }
    }
}
