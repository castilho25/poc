using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SFC.Models;
using SFC.Models.SFC;
using SFC.Models.SFC.Inst;
using SFC.Models.SFC.Paiis;
using SFC.Models.UserProfiles;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentitySample.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {

            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public virtual Guid IdPai { get; set; }
        public virtual Guid IdInstituicao { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        static ApplicationDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        
        public DbSet<Instituicoes> Instituicao { get; set; }
        public DbSet<Pais> Pai { get; set; }
        public DbSet<Detalhe> Detalhe { get; set; }
        public DbSet<TipoEnsino> TipoEnsino { get; set; }
        public DbSet<Filhos> Filhos { get; set; }
        public DbSet<Avaliacoes> Avaliacoes { get; set; }
        public DbSet<RegistoFilhos> RegistoFilhos { get; set; }
        public DbSet<Alunos> Alunos { get; set; }
        public DbSet<Atividades> Atividades { get; set; }
        public DbSet<Mensagens> Mensagens { get; set; }
    }
}

    