using Microsoft.EntityFrameworkCore;
using EmployeeAPI.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EmployeeAPI.Data
{
    public class ApplicationDbContext: IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> states { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<User> users { get; set; }
    }
}
