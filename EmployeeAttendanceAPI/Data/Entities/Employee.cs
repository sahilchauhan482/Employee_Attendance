using EmployeeCommon.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeAPI.Data.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Gender { get; set; }
        
        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string MobileNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Display(Name = "City")]

        [ForeignKey("CityId")]
        public int CityId { get; set; }

        public City City { get; set; }

        [Required]
        [Display(Name = "Joining Date")]
        [DataType(DataType.Date)]
        public DateTime JoiningDate { get; set; }
        [Display(Name = "Relieving Date")]
        public DateTime? RelievingDate { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }

        [Required]
        [Display(Name = "Date Of Birth")]
        public DateTime Dob { get; set; }
        [Required]
        [Display(Name = "Pan Number")]
        public string PanNumber { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public bool IsActive { get; set; }


    }
}
