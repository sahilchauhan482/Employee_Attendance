namespace EmployeeCommon.DTOs
{
    public class StateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
        public CountryDto Country { get; set; }
    }
}
