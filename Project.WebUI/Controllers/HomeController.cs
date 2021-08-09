using Project.BLL.DesignPatterns.GenericRepository.ConcRep;
using Project.COMMON.Tools;
using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.WebUI.Controllers
{
    public class HomeController : Controller
    {
        AppUserRepository _apRep;

        public HomeController()
        {
            _apRep = new AppUserRepository();
        }


        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(AppUser appUser)
        {
            AppUser yakalanan = _apRep.FirstOrDefault(x => x.UserName == appUser.UserName);

            string decrypted = DantexCrypt.DeCrypt(yakalanan.Password);

            if (appUser.Password == decrypted && yakalanan != null )
            {

                if (yakalanan.Role == ENTITIES.Enums.AppUserRole.Admin)
                {
                    if (!yakalanan.Active)
                    {
                        return AktifKontrol();
                    }
                    Session["admin"] = yakalanan;
                    return RedirectToAction("CategoryList", "Category", new { area = "Admin" });

                }
                else if (yakalanan.Role == ENTITIES.Enums.AppUserRole.Member)
                {
                    if (!yakalanan.Active)
                    {
                        return AktifKontrol();
                    }
                    Session["member"] = yakalanan;
                    return RedirectToAction("ShoppingList", "Shopping");
                }

                else
                {
                    ViewBag.RolBelirsiz = "Rol belirlenmemiş";
                    return View();
                }
               
                
             

            }

            ViewBag.KullaniciYok = "Kullanıcı bulunamadı";
            return View();




        }

        private ActionResult AktifKontrol()
        {
            ViewBag.AktifDegil = "Lutfen hesabınızı aktif hale getiriniz...Mailinizi kontrol ediniz...";
            return View("Login");
        }
    }
}