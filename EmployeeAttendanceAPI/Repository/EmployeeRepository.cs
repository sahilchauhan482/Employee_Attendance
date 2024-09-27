using EmployeeAPI.Data;
using EmployeeAPI.Data.Entities;
using EmployeeAPI.Repository.IRepository;

namespace EmployeeAPI.Repository
{
    public class EmployeeRepository:Repository<Employee>,IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;
        public EmployeeRepository(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }

    }
}
