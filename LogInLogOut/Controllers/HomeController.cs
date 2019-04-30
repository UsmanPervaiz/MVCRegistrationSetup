using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LogInLogOut.Models;
using PagedList;
using PagedList.Mvc;

namespace LogInLogOut.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        //Comment
        [Authorize]
        public ActionResult Index(string searchByNameOrGender, string searchTerm, int? page, string sortBy)
        {
                if (string.IsNullOrEmpty(sortBy) || sortBy.Substring(sortBy.Length - 3) == "Dsc")
                {
                    ViewBag.FirstNameSorting = "FirstNameAsc";
                    ViewBag.LastNameSorting = "LastNameAsc";
                    ViewBag.GenderSorting = "GenderAsc";
                }
                else
                {
                    ViewBag.FirstNameSorting = "FirstNameDsc";
                    ViewBag.LastNameSorting = "LastNameDsc";
                    ViewBag.GenderSorting = "GenderDsc";
                }


            string str = "";

            using (UsersDbContext db = new UsersDbContext())
            {
                IQueryable<User> users = db.Users;

                if (searchByNameOrGender == "Gender")
                {
                   users = users.Where(usr => usr.Gender.ToLower().StartsWith(searchTerm.ToLower()) || string.IsNullOrEmpty(searchTerm));
                }
                else
                {
                   users = users.Where(usr => (usr.FirstName + " " + usr.LastName).ToLower().StartsWith(searchTerm.ToLower()) || string.IsNullOrEmpty(searchTerm));
                }

                switch(sortBy)
                {
                    case "FirstNameAsc":
                        users = users.OrderBy(usr => usr.FirstName);
                        break;
                    case "FirstNameDsc":
                        users = users.OrderByDescending(usr => usr.FirstName);
                        break;
                    case "LastNameAsc":
                        users = users.OrderBy(usr => usr.LastName);
                        break;
                    case "LastNameDsc":
                        users = users.OrderByDescending(usr => usr.LastName);
                        break;
                    case "GenderAsc":
                        users = users.OrderBy(usr => usr.Gender);
                        break;
                    case "GenderDsc":
                        users = users.OrderByDescending(usr => usr.Gender);
                        break;
                    default:
                        str = str + "Do Nothing";
                        break;
                }

                return View(users.ToList().ToPagedList(page ?? 1, 5));
            }
           
        }

        public JsonResult AutoCompleteSearch(string term)
        {
            using (UsersDbContext db = new UsersDbContext())
            {
                List<string> studentNames = db.Users.Where(std => std.FirstName.ToLower().StartsWith(term.ToLower())).Select(usr => usr.FirstName + " " + usr.LastName).ToList();
                return Json(studentNames, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
