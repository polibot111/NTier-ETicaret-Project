using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.WebUI.Models
{
    public class ProductVM //PAVM ile aynı şekilde acılmıstır...Aslında aynı gorevleri yapıyor gibi gözükmektedirler...Fakat PAVM ek olarak alısveriş tarafına pagination işlemlerini de yapacak bir vm sınıfı oldugu icin ProductVM'den ayrılmıstır...Cünkü Admin'de pagination işlemleri zaten var..
    {
        public List<Product> Products { get; set; }
        public Product Product { get; set; }
        public List<Category> Categories { get; set; }

    }
}