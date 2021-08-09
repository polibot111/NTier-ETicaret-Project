using ConsumeDTOS.CDTOS;
using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.WebUI.Models.ShoppingTools
{
    public class OrderVM
    {
        public PaymentDTO PaymentDTO { get; set; }
        public Order Order { get; set; }

       
        
    }
}