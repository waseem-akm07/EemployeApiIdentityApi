using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Http.Description;
using EmployeeIdentityApi.Models;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using System.Security.Claims;

namespace EmployeeIdentityApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EmployeesController : ApiController
    {
        private MvcdbEntities db = new MvcdbEntities();

        //Enable CORS       
        public EmployeesController()
        {            
            db.Configuration.ProxyCreationEnabled = false;
        }

        

        // GET: api/Employees  
        /// <summary>
        /// To get all employee
        /// </summary>
        /// <returns> List of employee </returns> 
        /// This resource is for all types of role 
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        [Route("api/employee/getemployees")] 
        public IHttpActionResult GetEmployees()
        {
            var data = (from e in db.Employees
                        join m in db.EmpDep_Mapping on e.EmployeeId equals m.EmpId
                        join d in db.Departments on m.DepId equals d.DepartmentId
                        select new
                        {
                            e.EmployeeId,
                            e.EmployeeName,
                            e.EmployeeAddress,
                            e.EmployeeSalary,
                            d.DepartmentId,
                            d.DepartmentName
                        }).ToList();

            return Ok(data);
        }



        // GET: api/Department  
        /// <summary>
        /// To get all Department
        /// </summary>
        /// <returns> List of Department </returns> 
        /// This resource is for all types of role
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        [Route("api/employee/getdepartments")]
        public IQueryable<Department> GetDepartments()
        {
            
            return db.Departments;
        }


        
        // GET: api/Employees/1
        /// <summary>
        /// To get specific employee detail 
        /// </summary>
        /// <param name="id"> Get employee by id </param>
        /// <returns> Object of employee </returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/employee/getemployee/{id}")]
        [ResponseType(typeof(Employee))]
        public IHttpActionResult GetEmployee(int id)
        {
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }


        //GET:api/Employee
        /// <summary>
        /// Get Multi Department Employees
        /// </summary>
        /// <param name="model"> To pass multi-department id</param>
        /// <returns>Employees of selected Departments</returns>
        [HttpPost]
        [Authorize(Roles ="Admin, User")]
        [Route("api/employee/getemployeebydepartment")]
        public  IHttpActionResult GetEmployeeByDepartment(DepModel model)
        {
            List<DepartmentModel> data = null;
            List<DepartmentModel> result = new List<DepartmentModel>();

             foreach (var dep in model.Id)
             {
                 data = (from e in db.Employees
                            join m in db.EmpDep_Mapping on e.EmployeeId equals m.EmpId
                            join d in db.Departments on m.DepId equals d.DepartmentId
                            where d.DepartmentId == dep
                            select new DepartmentModel
                            {
                                EmployeeId = e.EmployeeId,
                                EmployeeName = e.EmployeeName,
                                EmployeeAddress = e.EmployeeAddress,
                                EmployeeSalary = e.EmployeeSalary,
                                DepartmentId = d.DepartmentId,
                                DepartmentName = d.DepartmentName
                            }).ToList();
                result.AddRange(data);
             }

            return Ok(result);
        }

        // PUT: api/Employees/5
        /// <summary>
        /// Update specific employee 
        /// </summary>
        /// <param name="id"> Fetch employee by id </param>
        /// <param name="employee"> Fetch detail of employee </param>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("api/employee/putemployee/{id}")]
        [ResponseType(typeof(DepartmentModel))]
        public IHttpActionResult PutEmployee(int id, DepartmentModel model)
        {
            var employee = db.Employees.Where(x => x.EmployeeId == id).FirstOrDefault();
            var mapping = db.EmpDep_Mapping.Where(x => x.EmpId == id).FirstOrDefault();

           // employee.EmployeeId = model.EmployeeId;
            employee.EmployeeName = model.EmployeeName;
            employee.EmployeeAddress = model.EmployeeAddress;
            employee.EmployeeSalary = model.EmployeeSalary;
            db.Entry(employee).State = EntityState.Modified;
            db.SaveChanges();
            int employeeId = employee.EmployeeId;
            
            mapping.DepId = model.DepartmentId;
            mapping.EmpId = employeeId;
            db.Entry(mapping).State = EntityState.Modified;
            db.SaveChanges();

            return Ok();
            // return StatusCode(HttpStatusCode.NoContent);
        }



        // POST: api/Employees
        /// <summary>
        /// Create new employee
        /// </summary>
        /// <param name="employees"> Post detail of employee </param>
        /// <returns>  Newly created resource </returns>
        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        [Route("api/employee/postemployee", Name = "PostEmployee")]
        [ResponseType(typeof(List<DepartmentModel>))]
        public IHttpActionResult PostEmployee(List<DepartmentModel> model)
        {
            Employee emp = new Employee();
            EmpDep_Mapping mapping = new EmpDep_Mapping();

            foreach (var employee in model)
            {               
                emp.EmployeeName = employee.EmployeeName;
                emp.EmployeeAddress = employee.EmployeeAddress;
                emp.EmployeeSalary = employee.EmployeeSalary;
                db.Employees.Add(emp);               

                // db.Employees.Add(employee);
                // employee.EmployeeId = employee.EmployeeId;
                mapping.DepId = employee.DepartmentId;
                mapping.EmpId = emp.EmployeeId;
                db.EmpDep_Mapping.Add(mapping);
                db.SaveChanges();
            }


            return Ok();
            //  return CreatedAtRoute("DefaultApi", null, employees);

        }



        // DELETE: api/Employees/2
        /// <summary>
        /// Delete specific employee by id
        /// </summary>
        /// <param name="id"> Fetch employee by id</param>
        /// <returns> Deleted employee </returns>
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("api/employee/deleteemployee/{id}")]
        [ResponseType(typeof(Employee))]
        public IHttpActionResult DeleteEmployee(int id)
        {
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            db.Employees.Remove(employee);
            db.SaveChanges();


            return Ok(employee);
        }


        
         /// <summary>
         /// To Generate Token
         /// </summary>
         /// <param name="Url">Base url for generating token</param>
         /// <param name="UserName">To comapare username</param>
         /// <param name="UserPassword">To compare password</param>
         /// <returns>Token</returns>
        [HttpPost]
        [Route("api/employee/posttoken", Name = "PostToken")]
        // [ResponseType(typeof(TokenModel))]
        public string PostToken(string Url, string UserName, string UserPassword)
        {
           // var role = db.Users.Where(x => x.UserName == UserName).Select(r => r.UserRole).FirstOrDefault();
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("UserName", UserName),
                new KeyValuePair<string, string>("Password", UserPassword),
               // new KeyValuePair<string, string>("role", role)
            };

            var content = new FormUrlEncodedContent(pairs);
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                HttpResponseMessage responce = client.PostAsync(Url + "/Token", content).Result;
                return responce.Content.ReadAsStringAsync().Result;
            }
        }



        /// <summary>
        /// To External Token
        /// </summary>
        /// <param name="Url">Base url for generating token</param>
        /// <param name="UserName">To compare username</param>
        /// <param name="UserPassword">To compare password</param>
        /// <returns>Token</returns>
        [HttpPost]
        [Route("api/employee/externalposttoken", Name = "ExternalPostToken")]
        public string ExternalPostToken(string Url, string UserName, string UserPassword)
        {
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("UserName", UserName),
                new KeyValuePair<string, string>("Password", UserPassword)
            };

            var content = new FormUrlEncodedContent(pairs);
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                HttpResponseMessage responce = client.PostAsync(Url + "/api/Account/ExternalLogin", content).Result;
                return responce.Content.ReadAsStringAsync().Result;
            }
        }
    }
}