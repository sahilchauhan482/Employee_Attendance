using EmployeeAPI.Data;
using EmployeeAPI.Data.Entities;
using EmployeeAPI.Repository.IRepository;

namespace EmployeeAPI.Repository
{
    public class UserRepository:Repository<User>,IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }


    }
}
