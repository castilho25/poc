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
using SFC.Models.SFC.Inst;
using System.Threading.Tasks;
using SFC.Models.SFC;
using System.Web.UI.WebControls;

namespace SFC.Controllers
{
    [Authorize(Roles ="Admin")]
    public class InstituicoesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private InstituicaoRepositorio InstituicaoRepositorio = new InstituicaoRepositorio();
        private DetalheRepositorio DetalheRepositorio = new DetalheRepositorio();
        private AvaliacoesRepositorio AvaliacoesRepositorio = new AvaliacoesRepositorio();
        private RegistoFilhosRepositorio RegistoFilhosRepositorio = new RegistoFilhosRepositorio();
        private AlunosRepositorio AlunosRepositorio = new AlunosRepositorio();

        // GET: Instituicoes
        [AllowAnonymous]
        public ActionResult Index(string searchString, DropDownList dropDownList)
        {
            ViewBag.Search = new SelectList(new[]
                {
                    new { ID = 0 , SearchType = "Nome"},
                    new { ID = 1 , SearchType = "Concelho"},
                    new { ID = 2 , SearchType = "Freguesia"}

                }
            , "ID", "SearchType");
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (Convert.ToInt32(dropDownList.ID))
                {
                    case 0: /*Pesquisa por Nome*/
                    {
                        return View(InstituicaoRepositorio.GetInstituicoesAprovedByName(searchString));
                    }
                    case 1: /*Pesquisa por Concelho*/
                    {
                        return View(InstituicaoRepositorio.GetInstituicoesAprovedByConcelho(searchString));
                       
                    }
                    case 2: /*Pesquisa por Freguesia*/
                    {
                        return View(InstituicaoRepositorio.GetInstituicoesAprovedByFreguesia(searchString));
                    }
                }
            }
            return View(InstituicaoRepositorio.GetInstituicoesAproved());
        }
        //GET: Instituicoes/NotAproved
        public ActionResult NotAproved(string searchString, DropDownList dropDownList)
        {
            ViewBag.Search = new SelectList(new[]
                {
                    new { ID = 0 , SearchType = "Nome"},
                    new { ID = 1 , SearchType = "Concelho"},
                    new { ID = 2 , SearchType = "Freguesia"}

                }
            , "ID", "SearchType");
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (Convert.ToInt32(dropDownList.ID))
                {
                    case 0: /*Pesquisa por Nome*/
                        {
                            return View(InstituicaoRepositorio.GetInstituicoesNotAprovedByName(searchString));
                        }
                    case 1: /*Pesquisa por Concelho*/
                        {
                            return View(InstituicaoRepositorio.GetInstituicoesNotAprovedByConcelho(searchString));

                        }
                    case 2: /*Pesquisa por Freguesia*/
                        {
                            return View(InstituicaoRepositorio.GetInstituicoesNotAprovedByFreguesia(searchString));
                        }
                }
            }
            return View(InstituicaoRepositorio.GetInstituicoesNotAproved());
          
        }

        //GET : Instituicoes/Avaliacoes
        public ActionResult Avaliacoes(Guid id)
        {
            ViewBag.Name = InstituicaoRepositorio.Get(id).Nome;
            return View(AvaliacoesRepositorio.GetAvaliacoesByIdInstituicao(id));
        }

        //GET : Instituicoes/PedidoRegistoAlunos
        public ActionResult PedidoRegistoAlunos(Guid id){
            ViewBag.Name = InstituicaoRepositorio.Get(id).Nome;
            return View(RegistoFilhosRepositorio.GetRegistoFilhosByInsituicaoID(id));
        }

        //GET : Instituicoes/AlunosRegistados
        public ActionResult AlunosRegistados(Guid id){
            ViewBag.Name = InstituicaoRepositorio.Get(id).Nome;
            return View(AlunosRepositorio.GetAlunosByIdInstituicao(id));
        }
        

        // GET: Instituicoes/Details/5
        [AllowAnonymous]
        public ActionResult Details(Guid id)
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
        // GET: Instituicoes/Edit/5
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

        // POST: Instituicoes/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InstituicaoID,Nome,Concelho,Freguesia,Endereco,TipoInstituicao,Aproved")] Instituicoes instituicoes)
        {
            Instituicoes inst = InstituicaoRepositorio.Get(instituicoes.InstituicaoID);
            if (ModelState.IsValid)
            {
                instituicoes.Aproved = inst.Aproved;
                db.Entry(instituicoes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(instituicoes);
        }

        //
        // GET: /Users/Aproved/5
        public ActionResult Aproved(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var instituicao = InstituicaoRepositorio.Get(id);
            if (instituicao == null)
            {
                return HttpNotFound();
            }
            return View(instituicao);
        }

        //
        // POST: /Users/Aproved/5
        [HttpPost, ActionName("Aproved")]
        [ValidateAntiForgeryToken]
        public ActionResult AprovedConfirmed(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var instituicao = InstituicaoRepositorio.Get(id);
            if (instituicao == null)
            {
                return HttpNotFound();
            }
            instituicao.Aproved = true;
            InstituicaoRepositorio.SaveChanges();
            return RedirectToAction("NotAproved");
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
