using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeIdentityApi.Models
{
    public class DepartmentModel
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int EmployeeId { get; set; } 
        public string EmployeeName { get; set; }
        public string EmployeeAddress { get; set; }
        public string EmployeeSalary { get; set; }
    }
}