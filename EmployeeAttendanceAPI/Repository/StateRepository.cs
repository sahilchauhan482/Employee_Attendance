using EmployeeAPI.Data;
using EmployeeAPI.Data.Entities;
using EmployeeAPI.Repository.IRepository;

namespace EmployeeAPI.Repository
{
    public class StateRepository:Repository<State>,IStateRepository
    {
        private readonly ApplicationDbContext _context;
        public StateRepository(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }
    }
}
