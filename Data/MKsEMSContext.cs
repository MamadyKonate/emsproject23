using Microsoft.EntityFrameworkCore;
using emsproject23.Models;
using emsproject23.ViewModels;

namespace emsproject23.Data
{
    public class EMSDbContext : DbContext
    {
        //public DbSet<User> Users { get; set; }
        //public DbSet<Manager> Managers { get; set; }
        //public DbSet<Employee> Employees { get; set; }
        //public DbSet<Credentials> Credentials { get; set; }
        //public DbSet<Contact> Contacts { get; set; }
        //public DbSet<Leave> Leaves { get; set; }
        //public DbSet<LeaveType> LeaveTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@"Data Source=C:\TempDB\EMSDbContext.db");

        public DbSet<User> Users { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Credentials> Credentials { get; set; } 
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Administrator> Administraors { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<LeaveAllowance> LeaveAllowances { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; } 
        public DbSet<UserLogin> UserLogins { get; set; } = default!;
        public DbSet<ResetPass> ResetPasses { get; set; }
    }
}
