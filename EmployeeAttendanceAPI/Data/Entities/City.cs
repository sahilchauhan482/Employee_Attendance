using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeAPI.Data.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey(nameof(StateId))]
        public int StateId { get; set; }
        public State State { get; set; }


    }
}
