﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModelClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ApplicationUser> applicationUser { get; set; }
        public DbSet<Inventory> inventories { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<PImages> images { get; set; }
        public DbSet<UserCart> userCarts { get; set; }
        public DbSet<UserOrderHeader> orderHeaders { get; set; }
        public DbSet<OrderDetails> orderDetails { get; set; }   
    }
}
