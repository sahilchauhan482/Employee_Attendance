using EmployeeAPI.Data;
using EmployeeAPI.Data.Entities;
using EmployeeAPI.Repository.IRepository;

namespace EmployeeAPI.Repository
{
    public class CityRepository:Repository<City>,ICityRepository
    {
        private readonly ApplicationDbContext _Context;
        public CityRepository(ApplicationDbContext context):base(context) 
        {
            _Context = context;
        }
    }
}
