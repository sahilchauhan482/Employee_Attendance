using EmployeeAPI.Data;
using EmployeeAPI.Data.Entities;
using EmployeeAPI.Repository.IRepository;

namespace EmployeeAPI.Repository
{
    public class AttendanceRepository:Repository<Attendance>,IAttendanceRepository
    {
        private readonly ApplicationDbContext _context;
        public AttendanceRepository(ApplicationDbContext  context):base(context)
        {
            _context = context;

        }
    }
}
