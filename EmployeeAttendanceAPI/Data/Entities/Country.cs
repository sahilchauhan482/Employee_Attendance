using System.ComponentModel.DataAnnotations;

namespace EmployeeAPI.Data.Entities
{
    public class Country
    {
        public int Id { get; set; }
        [Display(Name = "Country Name")]
        public string Name { get; set; }
    }
}
