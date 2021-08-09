using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus.DataSets;
using Project.COMMON.Tools;
using Project.DAL.Context;
using Project.ENTITIES.Models;

namespace Project.DAL.StrategyPattern
{

    //Bogus Kütüphanesi bize hazır Data sunacaktır...

    //MyInit kullanılırken Migrate yapmak saglıklı degildir...Veritabanınızı olusturmanın tek yolu Migration degildir. Siz düzgün bir CodeFirst yazarsanız Projenizdeki ilgili veritabanına gelecek tek bir istek bile DataBase'in olusması icin yeterlidir...



    public class MyInit : CreateDatabaseIfNotExists<MyContext> //Eger DataBase yoksa calısacak bir strateji 
    {

        //DataBase'e tam olusurken veri ekleyebilmek adına Seed metodunu override etmeliyiz...

        protected override void Seed(MyContext context)
        {
            #region Admin
            AppUser au = new AppUser();
            au.UserName = "cgr";
            au.Password = DantexCrypt.Crypt("123");
            au.Email = "cagri.yolyapar@gmail.com";

            au.Role = ENTITIES.Enums.AppUserRole.Admin;

            context.AppUsers.Add(au);
            context.SaveChanges(); //dikkat ederseniz au nesnesinin ID'si buradan sonra olusuyor...

            UserProfile up = new UserProfile();
            up.ID = au.ID;
            up.FirstName = "Çağrı";
            up.LastName = "Yolyapar";
            up.Address = "Barbaros mah, Degirmen Apt. Denizmen sok.";

            #endregion



            for (int i = 0; i < 10; i++)
            {

                AppUser ap = new AppUser();
                ap.UserName = new Internet("tr").UserName();
                ap.Password = new Internet("tr").Password();
                ap.Email = new Internet("tr").Email();

                context.AppUsers.Add(ap);

            }

            context.SaveChanges();

            for (int i = 2; i < 12; i++)
            {
                UserProfile upMember = new UserProfile();
                upMember.ID = i;//birebir ilişki oldugundan üst tarafta olusturulan AppUser'ların ID'leri ile buraları eşleşmeli.. O yüzden döngünün iterasyonunu 2'den baslattık...
                upMember.FirstName = new Name("tr").FirstName();
                upMember.LastName = new Name("tr").LastName();
                upMember.Address = new Address("tr").Locale;
                context.Profiles.Add(upMember);
            }

            context.SaveChanges();



            for (int i = 0; i < 10; i++)
            {

                Category c = new Category();
                c.CategoryName = new Commerce("tr").Categories(1)[0];
                c.Description = new Lorem("tr").Sentence(10);
             
                for (int j = 0; j < 30; j++)
                {
                    Product p = new Product();
                    p.ProductName = new Commerce("tr").ProductName();
                    p.UnitPrice = Convert.ToDecimal(new Commerce("tr").Price());
                    p.UnitsInStock = 100;
                    p.ImagePath = new Images().Nightlife();

                    c.Products.Add(p);

                }

                context.Categories.Add(c);
                context.SaveChanges();



            }


        }
    }
}
