using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdentitySample.Models;
using SFC.Models.SFC.Paiis;
using SFC.Models.Repositorio;
using System.Web.UI.WebControls;
using SFC.Models.SFC.Inst;

namespace SFC.Controllers
{
    [Authorize(Roles = "Admin,Pais,Instituicoes")]
    public class FilhosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private FilhosRepositorio FilhosRepositorio = new FilhosRepositorio();
        private RegistoFilhosRepositorio RegistoFilhosRepositorio = new RegistoFilhosRepositorio();
        private AlunosRepositorio AlunosRepositorio = new AlunosRepositorio();

        // GET: Filhos
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string searchString, DropDownList dropDownList)
        {
            ViewBag.Search = new SelectList(new[]
                {
                    new { ID = 0 , SearchType = "Nome"},
                    new { ID = 1 , SearchType = "Nome Pai"},
                    new { ID = 2 , SearchType = "ID Civil"},
                    new { ID = 3 , SearchType = "Data de Nascimento"},
                    new { ID = 4 , SearchType = "Nacionalidade"},
                    new { ID = 5 , SearchType = "Concelho"},
                    new { ID = 6 , SearchType = "Freguesia"},

                }
            , "ID", "SearchType");
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (Convert.ToInt32(dropDownList.ID))
                {
                    case 0: /*Pesquisa por Nome*/
                    {
                        return View(FilhosRepositorio.GetFilhosByNome(searchString));
                    }
                    case 1: /*Pesquisa por Nome Pai*/
                    {
                        return View(FilhosRepositorio.GetFilhosByPaiNome(searchString));
                    }
                    case 2: /*Pesquisa por ID Civil"*/
                    {
                        return View(FilhosRepositorio.GetFilhosByCivilID(searchString));
                    }
                    case 3: /*Pesquisa por Data de Nascimento*/
                    {
                        return View(FilhosRepositorio.GetFilhosByDataDeNascimento(searchString));
                    }
                    case 4: /*Pesquisa por Nacionalidade*/
                    {
                        return View(FilhosRepositorio.GetFilhosByNacionalidade(searchString));
                    }
                    case 5: /*Pesquisa por Concelho*/
                    {
                        return View(FilhosRepositorio.GetFilhosByConcelho(searchString));
                    }
                    case 6: /*Pesquisa por Freguesia*/
                    {
                        return View(FilhosRepositorio.GetFilhosByFreguesia(searchString));
                    }
                }
            }
            return View(FilhosRepositorio.GetList());
        }

        // GET: Filhos/Details/5
        [Authorize(Roles = "Pais,Admin")]
        public ActionResult Details(Guid id) {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Alunos alunos = AlunosRepositorio.Get(id);
            Filhos filhos = FilhosRepositorio.Get(id);
            if(alunos != null)
            {
                filhos.Instituicao = alunos.Instituicao;
            }
            if (filhos == null)
            {
                return HttpNotFound();
            }
            ViewBag.Registos = RegistoFilhosRepositorio.GetRegistoFilhosByFilhoID(id);
            return View(filhos);
        }

        // GET: Filhos/FilhoDetails/5
        [Authorize(Roles = "Instituicoes,Admin")]
        public ActionResult FilhoDetails(Guid id)
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
            return View(filhos);
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
