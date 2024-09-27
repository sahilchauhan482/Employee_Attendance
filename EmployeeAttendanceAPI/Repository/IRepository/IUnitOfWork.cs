namespace EmployeeAPI.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public IEmployeeRepository Employee { get;}
        public IAttendanceRepository Attendance { get;}
        public IUserRepository User { get;}
        public ICountryRepository Country { get;}
        public IStateRepository State { get;}
        public ICityRepository City { get;}
        void save();
    }
}
