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
using SFC.Models.SFC.Paiis;
using SFC.Models.SFC.Inst;
using System.Web.UI.WebControls;

namespace SFC.Controllers
{
    [Authorize(Roles = "Pais")]
    public class User_PaisController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private PaisRepositorio PaisRepositorio = new PaisRepositorio();
        private FilhosRepositorio FilhosRepositorio = new FilhosRepositorio();
        private InstituicaoRepositorio InstituicaoRepositorio = new InstituicaoRepositorio();
        private RegistoFilhosRepositorio RegistoFilhosRepositorio = new RegistoFilhosRepositorio();
        private AlunosRepositorio AlunosRepositorio = new AlunosRepositorio();
        private AvaliacoesRepositorio AvaliacoesRepositorio = new AvaliacoesRepositorio();

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

        // GET: User_Pais
        public ActionResult Index()
        {
            return View(PaisRepositorio.Get(UserManager.FindById(User.Identity.GetUserId()).IdPai));
        }

        // GET: Pais/Edit/5
        public ActionResult Edit(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pais pais = PaisRepositorio.Get(id);
            if (pais == null)
            {
                return HttpNotFound();
            }
            return View(pais);
        }

        // POST: Pais/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PaisID,Name,CivilID,DataNascimento,Nacionalidade,Profissao,Contacto,Endereco,Genero")] Pais pais)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pais).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pais);
        }

        // GET: /User_Pais/EditFilho/5
        public ActionResult EditFilho(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Filhos filhos = FilhosRepositorio.Get(id);
            if (filhos == null)
            {
                return HttpNotFound();
            }
            ViewBag.PaisID = filhos.PaisID;
            return View(filhos);
        }

        // POST: /User_Pais/EditFilho/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFilho([Bind(Include = "FilhoID,Name,CivilID,DataNascimento,Genero,EstadoCivil,Nacionalidade,Concelho,Freguesia,Endereço,PaisID")] Filhos filhos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(filhos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(filhos);
        }

        // GET: User_Pais/Create/5
        public ActionResult Create(Guid id)
        {
            return View();
        }

        // POST: User_Pais/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FilhoID,Name,CivilID,DataNascimento,Genero,EstadoCivil,Nacionalidade,Concelho,Freguesia,Endereço,PaisID")] Filhos filhos)
        {
            filhos.PaisID = PaisRepositorio.Get(Guid.Parse(Request.Url.Segments.GetValue(3).ToString())).PaisID;
            if (ModelState.IsValid)
            {
                filhos.FilhoID = Guid.NewGuid();
                FilhosRepositorio.Add(filhos);
                FilhosRepositorio.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(filhos);
        }

        // GET: User_Pais/RegistarFilho/5
        public ActionResult RegistarFilho(Guid id) {
            ViewBag.Filhos = PaisRepositorio.Get(UserManager.FindById(User.Identity.GetUserId()).IdPai).Filhos;
            var Registo = new RegistoFilhos{Instituicao = InstituicaoRepositorio.Get(id), InstituicaoID = id};
            
            return View(Registo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: User_Pais/RegistarFilho/5
        public ActionResult RegistarFilho(RegistoFilhos registoFilhos, FormCollection form)
        {
            var registo = RegistoFilhosRepositorio.GetRegistoFilhosByFilhoID(Guid.Parse(form["Nome"]), Guid.Parse(Request.Url.Segments.GetValue(3).ToString()));
            if (registo != null)
            {
                var mensg = "Seu estado se aprovação encontra-se";
                if (registo.Aproved.Equals(EstadoAproved.Aceite)) { mensg += " aceite"; }
                if (registo.Aproved.Equals(EstadoAproved.Recusado)) { mensg += " recusado"; }
                if (registo.Aproved.Equals(EstadoAproved.Pendente)) { mensg += " pendente"; }
                ModelState.AddModelError("", "Invalid Regist");
                //ViewBag.msg = "Seu filho já se encontra registado nesta Instituição. " + mensg;
            }
            else
            {
                if (AlunosRepositorio.Get(Guid.Parse(form["Nome"])) != null)
                {
                    // O BURRO DE MERDA JÁ ESTÁ INSCRITO
                    ModelState.AddModelError("", "Invalid Regist");
                    //ViewBag.msg =  "O Burro do seu filho já está inscrito numa instituição. ";
                }
            }
            if (ModelState.IsValid)
            {
                
                registoFilhos.RegistoID = Guid.NewGuid();
                registoFilhos.FilhoID = Guid.Parse(form["Nome"]);
                registoFilhos.InstituicaoID = Guid.Parse(Request.Url.Segments.GetValue(3).ToString());
                registoFilhos.Aproved = EstadoAproved.Pendente;
                
                RegistoFilhosRepositorio.Add(registoFilhos);
                RegistoFilhosRepositorio.SaveChanges();
                return RedirectToAction("Index", "Instituicoes");
            }

            return RedirectToAction("RegistarFilho");
        }

        // GET: User_Pais/RealizarAvaliacao
        public ActionResult RealizarAvaliacao()
        {
            List<Filhos> filhos = PaisRepositorio.Get(UserManager.FindById(User.Identity.GetUserId()).IdPai).Filhos;
            List<Alunos> alunos = new List<Alunos>();
            foreach (var filho in filhos)
            {
                Alunos aluno = AlunosRepositorio.Get(filho.FilhoID);
                Avaliacoes avaliacoes = AvaliacoesRepositorio.GetAvaliacaoByIdFilho(filho.FilhoID);
                if(aluno != null && avaliacoes == null){alunos.Add(aluno);}
            }
            return View(alunos);
        }



        // GET: User_Pais/AvaliarInstituicao/5
        public ActionResult AvaliarInstituicao(Guid idInstituicao , Guid idFilho)
        {
            Avaliacoes avaliacao = new Avaliacoes
            {
                AvaliacaoID = Guid.NewGuid(),
                FilhoID = idFilho,
                InstituicaoID = idInstituicao,
                PaiID = UserManager.FindById(User.Identity.GetUserId()).IdPai
            };
            return View(avaliacao);
        }

        // POST: User_Pais/AvaliarInstituicao
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AvaliarInstituicao([Bind(Include = "AvaliacaoID,InstituicaoID,PaiID,FilhoID,Nota,Critica")] Avaliacoes avaliacoes)
        {
            if (ModelState.IsValid)
            {
                avaliacoes.AvaliacaoID = Guid.NewGuid();
                db.Avaliacoes.Add(avaliacoes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FilhoID = new SelectList(db.Filhos, "FilhoID", "Name", avaliacoes.FilhoID);
            ViewBag.InstituicaoID = new SelectList(db.Instituicao, "InstituicaoID", "Nome", avaliacoes.InstituicaoID);
            ViewBag.PaiID = new SelectList(db.Pai, "PaisID", "Name", avaliacoes.PaiID);
            return View(avaliacoes);
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
