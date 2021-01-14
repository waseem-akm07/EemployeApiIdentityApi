using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeIdentityApi.Models
{
    public class EmpModel
    {
        public int employeeId { get; set; }
        public string employeeName { get; set; }
        public string employeeAddress { get; set; }
        public string employeeSalary { get; set; }
    }
}