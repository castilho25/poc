using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdentitySample.Models;
using SFC.Models.UserProfiles;
using SFC.Models.Repositorio;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using SFC.Models.SFC.Inst;
using SFC.Models.SFC;

namespace SFC.Controllers
{
    [Authorize(Roles ="Instituicoes")]
    public class User_InstituicoesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private InstituicaoRepositorio InstituicaoRepositorio = new InstituicaoRepositorio();
        private AtividadesRepositorio AtividadesRepositorio = new AtividadesRepositorio();
        private DetalheRepositorio DetalheRepositorio = new DetalheRepositorio();
        private RegistoFilhosRepositorio RegistoFilhosRepositorio = new RegistoFilhosRepositorio();
        private AlunosRepositorio AlunosRepositorio = new AlunosRepositorio();
        private MensagensRepositorio MensagensRepositorio = new MensagensRepositorio();

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


        // GET: User_Instituicoes
        public ActionResult Index()
        {
            return View(InstituicaoRepositorio.Get(UserManager.FindById(User.Identity.GetUserId()).IdInstituicao));
        }

        // GET: User_Instituicoes/AprovarAlunos
        public ActionResult AprovarAlunos() {
            return View(RegistoFilhosRepositorio.GetRegistoFilhosByInsituicaoID(UserManager.FindById(User.Identity.GetUserId()).IdInstituicao));
        }
        // GET: User_Instituicoes/Edit/5
        public ActionResult Edit(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instituicoes instituicoes = InstituicaoRepositorio.Get(id);
            if (instituicoes == null)
            {
                return HttpNotFound();
            }
            return View(instituicoes);
        }

        // POST: User_Instituicoes/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InstituicaoID,Nome,Concelho,Freguesia,Endereco,TipoInstituicao,Aproved")] Instituicoes instituicoes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(instituicoes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(instituicoes);
        }

        // GET: User_Instituicoes/CreateAtividade
        public ActionResult CreateAtividade(Guid id)
        {
            return View();
        }

        // POST: Atividades/CreateAtividade
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAtividade([Bind(Include = "AtividadeID,Nome,Descricao,InicioAtividade,FimAtividade,DetalheID")] Atividades atividades)
        {
            atividades.DetalheID = DetalheRepositorio.Get(Guid.Parse(Request.Url.Segments.GetValue(3).ToString())).DetalheID;
            if (ModelState.IsValid)
            {
                atividades.AtividadeID = Guid.NewGuid();
                AtividadesRepositorio.Add(atividades);
                AtividadesRepositorio.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(atividades);
        }

        // GET: User_Instituicoes/EditarAtividade/5
        public ActionResult EditarAtividade(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Atividades atividades = AtividadesRepositorio.Get(id);
            if (atividades == null)
            {
                return HttpNotFound();
            }
            return View(atividades);
        }

        // POST: User_Instituicoes/EditarAtividade/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAtividade([Bind(Include = "AtividadeID,Nome,Descricao,InicioAtividade,FimAtividade,DetalheID")] Atividades atividades)
        {
            if (ModelState.IsValid)
            {
                db.Entry(atividades).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(atividades);
        }

        // GET: /User_Instituicoes/AprovedAluno/5
        public ActionResult AprovedAluno(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var registoAluno = RegistoFilhosRepositorio.Get(id);
            if (registoAluno == null)
            {
                return HttpNotFound();
            }
            return View(registoAluno);
        }

        //
        // POST: /User_Instituicoes/AprovedAluno/5
        [HttpPost, ActionName("AprovedAluno")]
        [ValidateAntiForgeryToken]
        public ActionResult AprovedConfirmed(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var registo = RegistoFilhosRepositorio.Get(id);
            if (registo == null)
            {
                return HttpNotFound();
            }
            if (Request.Form["true"] != null)
            {
                var registos = RegistoFilhosRepositorio.GetRegistoFilhosByFilhoID(registo.FilhoID);
                foreach (var reg in registos)
                {
                    reg.Aproved = EstadoAproved.Recusado;
                }
                registo.Aproved = EstadoAproved.Aceite;
                var aluno = new Alunos
                {
                    AlunosID = registo.FilhoID,
                    InstituicaoID = registo.InstituicaoID,
                    DataDeRegisto = DateTime.Now.Date
                };
                AlunosRepositorio.Add(aluno);
                var msg = new Mensagens
                {
                    MensagemID = Guid.NewGuid(),
                    SenderID = registo.InstituicaoID,
                    ReceiverID = registo.Filhos.PaisID,
                    Assunto = "Registo na Instituição " + registo.Instituicao.Nome,
                    Mensagem = "O seu filho foi aprovado !",
                    DataEnvio = DateTime.Now.Date,
                    Vista = false
                };
                MensagensRepositorio.Add(msg);
            }else{
                var msg = new Mensagens
                {
                    MensagemID = Guid.NewGuid(),
                    SenderID = registo.InstituicaoID,
                    ReceiverID = registo.Filhos.PaisID,
                    Assunto = "Registo na Instituição " + registo.Instituicao.Nome,
                    Mensagem = "O seu filho foi recusado !",
                    DataEnvio = DateTime.Now.Date,
                    Vista = false
                };
                MensagensRepositorio.Add(msg);
                registo.Aproved = EstadoAproved.Recusado;
            }
            RegistoFilhosRepositorio.SaveChanges();
            AlunosRepositorio.SaveChanges();
            MensagensRepositorio.SaveChanges();
            return RedirectToAction("AprovarAlunos");
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
