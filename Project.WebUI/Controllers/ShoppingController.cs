using Project.BLL.DesignPatterns.GenericRepository.ConcRep;
using System;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.WebUI.Models;
using Project.WebUI.Models.ShoppingTools;
using Project.ENTITIES.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Project.COMMON.Tools;

namespace Project.WebUI.Controllers
{
    public class ShoppingController : Controller
    {
        OrderRepository _oRep;
        ProductRepository _pRep;
        CategoryRepository _cRep;
        OrderDetailRepository _odRep;

        public ShoppingController()
        {
            _oRep = new OrderRepository();
            _pRep = new ProductRepository();
            _cRep = new CategoryRepository();
            _odRep = new OrderDetailRepository();
        }


       
        public ActionResult ShoppingList(int? page, int? categoryID) 
        {

          

            PAVM pavm = new PAVM()
            {
                PagedProducts = categoryID == null ? _pRep.GetActives().ToPagedList(page ?? 1, 9) : _pRep.Where(x => x.CategoryID == categoryID).ToPagedList(page ?? 1, 9),
                Categories = _cRep.GetActives()
            };

            if (categoryID != null) TempData["catID"] = categoryID;


            return View(pavm);
        }


        public ActionResult AddToCart(int id)
        {
            Cart c = Session["scart"] == null ? new Cart() : Session["scart"] as Cart;

            Product eklenecekUrun = _pRep.Find(id);

            CartItem ci = new CartItem
            {
                ID = eklenecekUrun.ID,
                Name = eklenecekUrun.ProductName,
                Price = eklenecekUrun.UnitPrice,
                ImagePath = eklenecekUrun.ImagePath
            };

            c.SepeteEkle(ci);
            Session["scart"] = c;
            return RedirectToAction("ShoppingList");
        }







        public ActionResult CartPage()
        {
            if (Session["scart"] != null)
            {
                CartPageVM cpvm = new CartPageVM();
                Cart c = Session["scart"] as Cart;

                cpvm.Cart = c;
                return View(cpvm);
            }

            TempData["sepetBos"] = "Sepetinizde ürün bulunmamaktadır";
            return RedirectToAction("ShoppingList");
        }

        public ActionResult DeleteFromCart(int id)
        {
            if (Session["scart"] != null)
            {
                Cart c = Session["scart"] as Cart;

                c.SepettenSil(id);
                if (c.Sepetim.Count == 0)
                {
                    Session.Remove("scart");
                    TempData["sepetBos"] = "Sepetinizde ürün bulunmamaktadır";
                    return RedirectToAction("ShoppingList");
                }
                return RedirectToAction("CartPage");
            }

            return RedirectToAction("ShoppingList");
        }

        public ActionResult SiparisOnayla()
        {
            AppUser mevcutKullanici;

            if (Session["member"] != null)
            {
                mevcutKullanici = Session["member"] as AppUser;
            }
            else
            {
                TempData["anonim"] = "Kullanıcı üye degil";
            }
            return View();

        }

        //https://localhost:44396/api/Payment/ReceivePayment

        [HttpPost]
        public ActionResult SiparisiOnayla(OrderVM ovm)
        {
            bool result;

            Cart sepet = Session["scart"] as Cart;

            ovm.Order.TotalPrice = ovm.PaymentDTO.ShoppingPrice = sepet.TotalPrice.Value;



            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44396/api/");

                Task<HttpResponseMessage> postTask = client.PostAsJsonAsync("Payment/ReceivePayment", ovm.PaymentDTO);

                HttpResponseMessage sonuc;

                try
                {
                    sonuc = postTask.Result;
                }
                catch (Exception ex)
                {

                    TempData["baglantiRed"] = "Banka baglantıyı reddetti";
                    return RedirectToAction("ShoppingList");
                }


                if (sonuc.IsSuccessStatusCode) result = true;

                else result = false;

                if (result)
                {
                    if (Session["member"] != null)
                    {
                        AppUser kullanici = Session["member"] as AppUser;
                        ovm.Order.AppUserID = kullanici.ID;
                        ovm.Order.UserName = kullanici.UserName;
                    }
                    else
                    {
                        ovm.Order.AppUserID = null;
                        ovm.Order.UserName = TempData["anonim"].ToString();
                    }

                    _oRep.Add(ovm.Order); 

                    foreach (CartItem item in sepet.Sepetim)
                    {
                        OrderDetail od = new OrderDetail();
                        od.OrderID = ovm.Order.ID;
                        od.ProductID = item.ID;
                        od.TotalPrice = item.SubTotal;
                        od.Quantity = item.Amount;
                        _odRep.Add(od);


                        Product stokDus = _pRep.Find(item.ID);
                        stokDus.UnitsInStock -= item.Amount;
                        _pRep.Update(stokDus);

                    }

                    TempData["odeme"] = "Siparişiniz bize ulasmıstır. Tesekkür ederiz";
                    MailSender.Send(ovm.Order.Email, body: $"Siparişiniz basarıyla alındı...{ovm.Order.TotalPrice}");
                    return RedirectToAction("ShoppingList");



                }

                else
                {
                    TempData["sorun"] = "Odeme ile ilgili bir sorun olustu. Lutfen bankanızla iletişmize geciniz";
                    return RedirectToAction("ShoppingList");
                }



            }









            return View();
        }



    }
}