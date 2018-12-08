using System;
using System.Collections.Generic;
using System.Text;
using IdentityCustomizationDemo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityCustomizationDemo.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);
            builder
                .Entity<UserAddress>(entityBuilder => {
                    entityBuilder.ToTable("UserAddresses");
                    entityBuilder
                        .HasOne<ApplicationUser>()
                        .WithMany(user => user.Addresses)
                        .HasForeignKey(userAddress => userAddress.UserId)
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
