using Microsoft.AspNetCore.Mvc;
using MvcCoreLinqToSql.Models;
using MvcCoreLinqToSql.Repositories;

namespace MvcCoreLinqToSql.Controllers
{
    public class EnfermosController : Controller
    {
        private RepositoryEnfermos repo;
        public EnfermosController()
        {
            this.repo = new RepositoryEnfermos();
        }
        public IActionResult Index()
        {
            List<Enfermo> enfermos = this.repo.GetEnfermos();
            return View(enfermos);
        }
        public IActionResult Details(string id)
        {
            Enfermo enfermo = this.repo.FindEnfermo(id);
            return View(enfermo);
        }
        public async Task<IActionResult> Delete(string id)
        {
            await this.repo.DeleteEmpleado(id);
            return RedirectToAction("index");
        }
    }
}
