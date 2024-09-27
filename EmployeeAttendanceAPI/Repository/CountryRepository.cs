using EmployeeAPI.Data;
using EmployeeAPI.Data.Entities;
using EmployeeAPI.Repository.IRepository;

namespace EmployeeAPI.Repository
{
    public class CountryRepository:Repository<Country>,ICountryRepository
        
    {
        private readonly ApplicationDbContext _context;
        public CountryRepository(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }
    }
}
