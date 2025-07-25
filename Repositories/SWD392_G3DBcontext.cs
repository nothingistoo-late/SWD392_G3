﻿using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class SWD392_G3DBcontext : IdentityDbContext<User,Role, Guid>
    {
        public SWD392_G3DBcontext(DbContextOptions<SWD392_G3DBcontext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<StaffSchedule> StaffSchedules { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<CustomerMembership> CustomerMemberships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");


            modelBuilder.Entity<OrderDetail>()
               .HasOne(od => od.Staff)
               .WithMany(s => s.OrderDetails)
               .HasForeignKey(od => od.StaffId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Service)
                .WithMany(s => s.OrderDetails)
                .HasForeignKey(od => od.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // 👈 Giữ lại cascade 1 thằng thôi

            modelBuilder.Entity<Order>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<OrderDetail>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Customer>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Service>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Staff>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Manager>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<StaffSchedule>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Rating>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Membership>().HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
