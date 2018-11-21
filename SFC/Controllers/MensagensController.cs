using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdentitySample.Models;
using SFC.Models.SFC;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using SFC.Models.Repositorio;
using SFC.Models.UserProfiles;

namespace SFC.Controllers
{
    [Authorize]
    public class MensagensController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MensagensRepositorio MensagensRepositorio = new MensagensRepositorio();
        private InstituicaoRepositorio InstituicaoRepositorio = new InstituicaoRepositorio();
        private PaisRepositorio PaisRepositorio = new PaisRepositorio();

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Mensagens
        public ActionResult Index()
        {
            Guid id;
            var DefaultGuid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            var Sender = (UserManager.FindById(User.Identity.GetUserId()));
            if (Sender.IdInstituicao.Equals(DefaultGuid))
            {
                if (Sender.IdPai.Equals(DefaultGuid)) { id = Guid.Parse(Sender.Id); }
                else { id = Sender.IdPai; }
            }
            else { id = Sender.IdInstituicao; }
            ViewBag.NovasMsg = MensagensRepositorio.NumeroDeNovasMensagens(id);

            return View(MensagensRepositorio.GetMensagensVistas(id));
        }
        // GET: Mensagens/NaoVistas
        public ActionResult NaoVistas()
        {
            Guid id;
            var DefaultGuid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            var Sender = (UserManager.FindById(User.Identity.GetUserId()));
            if (Sender.IdInstituicao.Equals(DefaultGuid))
            {
                if (Sender.IdPai.Equals(DefaultGuid)) { id = Guid.Parse(Sender.Id); }
                else { id = Sender.IdPai; }
            }
            else { id = Sender.IdInstituicao; }

            return View(MensagensRepositorio.GetMensagensNaoVistas(id));
        }


        // GET: Mensagens/Details/5
        public ActionResult Details(Guid id)
        {
            if (id == null){
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mensagens mensagens = MensagensRepositorio.Get(id);
            
            if (mensagens == null){
                return HttpNotFound();
            }
            if (mensagens.Vista == false){
                mensagens.Vista = true;
                MensagensRepositorio.SaveChanges();
            }
            var SenderI = InstituicaoRepositorio.Get(mensagens.SenderID);
            var SenderP = PaisRepositorio.Get(mensagens.SenderID);
            if(SenderI == null){
                if(SenderP == null){ViewBag.Nome = "Administrador"; }
                else{ ViewBag.Nome = SenderP.Name; }
            }
            else{ ViewBag.Nome = SenderI.Nome; }
            return View(mensagens);
        }


        // GET: Mensagens/EnviarMensagem
        [Authorize(Roles = "Admin,Instituicoes")]
        public ActionResult EnviarMensagem()
        {
            Mensagens msg = new Mensagens();
            if (User.IsInRole("Admin")){
                ViewBag.Instituicoes = InstituicaoRepositorio.GetList();
                ViewBag.Pais = PaisRepositorio.GetList();
                msg.SenderID = Guid.Parse(UserManager.FindById(User.Identity.GetUserId()).Id);
            }
            else{
                List<Pais> pais = new List<Pais>();
                msg.SenderID = UserManager.FindById(User.Identity.GetUserId()).IdInstituicao;
                Instituicoes instituicoes = InstituicaoRepositorio.Get(msg.SenderID);
                if(instituicoes != null)
                {
                    foreach (var aluno in instituicoes.Alunos){
                        pais.Add(aluno.Filhos.Pai);
                    }
                }
                ViewBag.Pais = pais;
             }
            return View(msg);
        }


        // POST: Mensagens/EnviarMensagem
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Instituicoes")]
        public ActionResult EnviarMensagem([Bind(Include = "MensagemID,SenderID,ReceiverID,Assunto,Mensagem,DataEnvio,Vista")] Mensagens mensagens)
        {
            var Sender = (UserManager.FindById(User.Identity.GetUserId()));
            if (ModelState.IsValid)
            {
                mensagens.MensagemID = Guid.NewGuid();
                mensagens.DataEnvio = DateTime.Now.Date;
                mensagens.Vista = false;
                mensagens.ReceiverID = Guid.Parse(Request.Form["NomeReceiver"]);
                db.Mensagens.Add(mensagens);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mensagens);
        }

        // GET: Mensagens/Create/5
        [Authorize(Roles ="Admin,Instituicoes")]
        public ActionResult Create(Guid id)
        {
            return View();
        }

        // POST: Mensagens/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Instituicoes")]
        public ActionResult Create([Bind(Include = "MensagemID,SenderID,ReceiverID,Assunto,Mensagem,DataEnvio,Vista")] Mensagens mensagens)
        {
            var DefaultGuid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            var Sender = (UserManager.FindById(User.Identity.GetUserId()));
            if (Sender.IdInstituicao.Equals(DefaultGuid))
            {
                if (Sender.IdPai.Equals(DefaultGuid)){mensagens.SenderID = Guid.Parse(Sender.Id);}
                else{mensagens.SenderID = Sender.IdPai;}
            }
            else{mensagens.SenderID = Sender.IdInstituicao;}

            if (ModelState.IsValid)
            {
                mensagens.MensagemID = Guid.NewGuid();
                mensagens.DataEnvio = DateTime.Now.Date;
                mensagens.ReceiverID = Guid.Parse(Request.Url.Segments.GetValue(3).ToString());
                mensagens.Vista = false;
                
                db.Mensagens.Add(mensagens);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mensagens);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
