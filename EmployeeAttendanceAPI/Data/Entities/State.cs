using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeAPI.Data.Entities
{
    public class State
    {
        public int Id { get; set; }

        [Display(Name = "State Name")]
        public string Name { get; set; }
        [ForeignKey(nameof(CountryId))]
        public int CountryId { get; set; }
        public Country Country { get; set; }


    }
}
