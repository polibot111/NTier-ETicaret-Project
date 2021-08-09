using Project.BLL.DesignPatterns.GenericRepository.ConcRep;
using Project.COMMON.Tools;
using Project.ENTITIES.Models;
using Project.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.WebUI.Controllers
{
    public class RegisterController : Controller
    {

        AppUserRepository _apRep;
        ProfileRepository _pRep;

        public RegisterController()
        {
            _pRep = new ProfileRepository();
            _apRep = new AppUserRepository();
        }


        public ActionResult RegisterNow()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterNow(AppUserVM apvm)
        {

            AppUser appUser = apvm.AppUser;
            UserProfile profile = apvm.Profile;

            appUser.Password = DantexCrypt.Crypt(appUser.Password);

            if (_apRep.Any(x => x.UserName == appUser.UserName))
            {
                ViewBag.ZatenVar = "Kullanıcı ismi daha önce alınmıs";
                return View();
            }
            else if (_apRep.Any(x => x.Email == appUser.Email))
            {
                ViewBag.ZatenVar = "Email zaten kayıtlı";
                return View();
            }


            string gonderilecekMail = "Tebrikler...Hesabınız olusturulmustur...Hesabınızı aktive etmek icin https://localhost:44347/Register/Activation/" + appUser.ActivationCode + " linkine tıklayabilirsiniz.";

            MailSender.Send(appUser.Email, body: gonderilecekMail, subject: "Hesap aktivasyon!");
            _apRep.Add(appUser); 


            if (!string.IsNullOrEmpty(profile.FirstName) || !string.IsNullOrEmpty(profile.LastName) || !string.IsNullOrEmpty(profile.Address))
            {
                profile.ID = appUser.ID;
                _pRep.Add(profile);
            }

            return View("RegisterOk");
        }

        public ActionResult Activation(Guid id)
        {
            AppUser aktifEdilecek = _apRep.FirstOrDefault(x => x.ActivationCode == id);

            if (aktifEdilecek != null)
            {
                aktifEdilecek.Active = true;
                _apRep.Update(aktifEdilecek);

                TempData["HesapAktifmi"] = "Hesabınız Aktif hale getirildi";
                return RedirectToAction("Login", "Home");
            }
            TempData["HesapAktifmi"] = "Aktif edilecek hesap bulunamadı";
            return RedirectToAction("Login", "Home");


           
        }

        public ActionResult RegisterOk()
        {
            return View();
        }

    }
}