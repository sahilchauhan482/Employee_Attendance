using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommon.DTOs
{
    public class AttendanceDTO
    {       
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public decimal EmployeeSalary { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime InTime { get; set; }
        public DateTime OutTime { get; set; }
        public bool Leave { get; set; }
        public bool Absent  { get; set; }
        public string? Duration { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        
    }
}
