using Project.ENTITIES.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.ENTITIES.Models
{
    public class AppUser:BaseEntity
    {
        public AppUser()
        {
            Role = AppUserRole.Member;
            ActivationCode = Guid.NewGuid();
        }

        public string UserName { get; set; }
        public string Password { get; set; }

        //Guid sizin unique(benzeri olmayan) bir kod üretmenizi saglayan bir yapıdır...
        public Guid ActivationCode { get; set; }
        public bool Active { get; set; }
        public string Email { get; set; }
        public AppUserRole Role { get; set; }

        //Relational Properties

        public virtual UserProfile Profile { get; set; }
        public virtual List<Order> Orders { get; set; }




    }
}
