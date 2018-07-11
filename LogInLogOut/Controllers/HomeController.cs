using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LogInLogOut.Models;

namespace LogInLogOut.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [Authorize]
        public ActionResult Index()
        {
            using (UsersDbContext db = new UsersDbContext())
            {
                return View(db.Users.ToList());
            }
           
        }
    }
}