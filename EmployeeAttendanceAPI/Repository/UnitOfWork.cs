using EmployeeAPI.Data;
using EmployeeAPI.Data.Entities;
using EmployeeAPI.Repository.IRepository;

namespace EmployeeAPI.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _context = dbContext;
            Employee=new EmployeeRepository(_context);
            User = new UserRepository(_context);
            Attendance = new AttendanceRepository(_context);
            Country = new CountryRepository(_context);
            State = new StateRepository(_context);
            City = new CityRepository(_context);
        }
        public IEmployeeRepository Employee { private set; get; }
        public IAttendanceRepository Attendance { private set; get; }
        public IUserRepository User { private set; get; }
        public ICountryRepository Country { private set; get; }
        public IStateRepository  State { private set; get; }
        public ICityRepository City { private set; get; }

        public void save()
        {
            _context.SaveChanges();
        }
    }
}
