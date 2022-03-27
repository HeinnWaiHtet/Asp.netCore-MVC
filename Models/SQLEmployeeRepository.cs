namespace Asp.netCore_MVC.Models
{
    using Microsoft.EntityFrameworkCore;

    public class SQLEmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext context;

        public SQLEmployeeRepository(AppDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Add Employee Data To Database
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Employee Add(Employee employee)
        {
            this.context.Employees.Add(employee);
            this.context.SaveChanges();
            return employee;
        }

        /// <summary>
        /// Delete Employee Data By Request Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Employee Delete(int id)
        {
            var employee = this.context.Employees.Find(id);
            if(employee != null)
            {
                this.context.Employees.Remove(employee);
                this.context.SaveChanges();
            }

            return employee ?? new Employee();
        }

        /// <summary>
        /// Get All Employee Data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Employee> GetAllEmployees()
        {
            return this.context.Employees;
        }

        /// <summary>
        /// Get Employee Data By Request Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Employee GetEmployeeById(int id)
        {
            return this.context.Employees.Find(id) ?? new Employee();
        }

        public Employee Update(Employee employeeChanges)
        {
            var employee = this.context.Employees.Attach(employeeChanges);
            employee.State = EntityState.Modified;
            context.SaveChanges();
            return employeeChanges;
        }
    }
}
