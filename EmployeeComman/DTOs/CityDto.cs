namespace EmployeeCommon.DTOs
{
    public class CityDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StateId { get; set; }
        public StateDto state { get; set; }
    }
}
