using System.ComponentModel.DataAnnotations;

namespace EmployeeCommon.DTOs
{
    public class EmployeeDto
    {
        public EmployeeDto()
        {
            IsActive = true;           
        }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string? Gender { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string MobileNumber { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        public int CityId { get; set; }        
        public string? CityName { get; set; }        
        public int StateId { get; set; }        
        public string? StateName { get; set; }        
        public int CountryId { get; set; }        
        public string? CountryName { get; set;}

        [Required]
        [Display(Name = "Joining Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime JoiningDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? RelievingDate { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }
        [Display(Name = "Date Of Birth")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Dob { get; set; }
        [Display(Name = "Pan Number")]
       
        public string PanNumber { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public bool IsActive { get; set; }

       
    }
}
