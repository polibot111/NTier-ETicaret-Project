﻿using Project.DAL.StrategyPattern;
using Project.ENTITIES.Models;
using Project.MAP.Options;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DAL.Context
{
    public class MyContext:DbContext
    {
        public MyContext():base("myConnection")
        {
            Database.SetInitializer(new MyInit()); //MyInit sınıfımız gorevini burada yapacak...
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AppUserMap());
            modelBuilder.Configurations.Add(new CategoryMap());
            modelBuilder.Configurations.Add(new OrderDetailMap());
            modelBuilder.Configurations.Add(new OrderMap());
            modelBuilder.Configurations.Add(new ProductMap());
            modelBuilder.Configurations.Add(new ProfileMap());
            modelBuilder.Configurations.Add(new ShipperMap());


        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Category>  Categories { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<UserProfile>  Profiles { get; set; }
        public DbSet<Shipper> Shippers { get; set; }




    }
}
