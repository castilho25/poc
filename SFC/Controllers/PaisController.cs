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
using SFC.Models.SFC.Paiis;
using System.Web.UI.WebControls;

namespace SFC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PaisController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private PaisRepositorio PaisRepositorio = new PaisRepositorio();
        private FilhosRepositorio FilhosRepositorio = new FilhosRepositorio();

        // GET: Pais
        public ActionResult Index(string searchString, DropDownList dropDownList)
        {
            ViewBag.Search = new SelectList(new[]
                {
                    new { ID = 0 , SearchType = "Nome"},
                    new { ID = 1 , SearchType = "ID Civil"},
                    new { ID = 2 , SearchType = "Contacto"}

                }
            , "ID", "SearchType");
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (Convert.ToInt32(dropDownList.ID))
                {
                    case 0: /*Pesquisa por Nome*/
                        {
                            return View(PaisRepositorio.GetPaisByName(searchString));
                        }
                    case 1: /*Pesquisa por CivilID*/
                        {
                            return View(PaisRepositorio.GetPaisByCivilID(searchString));

                        }
                    case 2: /*Pesquisa por Contacto*/
                        {
                            return View(PaisRepositorio.GetPaisByContacto(searchString));
                        }
                }
            }
            return View(PaisRepositorio.GetList());
        }

        // GET: Pais/Details/5
        public ActionResult Details(Guid id)
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
        // GET: /Pais/EditFilho/5
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

        // POST: /Pais/EditFilho/5
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
                return RedirectToAction ("/Details/"+filhos.PaisID);
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
