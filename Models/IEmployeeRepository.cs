namespace Asp.netCore_MVC.Models
{
    public interface IEmployeeRepository
    {
        /// <summary>
        /// Get Employee Detail By Request Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Employee GetEmployeeById(int id);

        /// <summary>
        /// Get All Employee Data
        /// </summary>
        /// <returns></returns>
        IEnumerable<Employee> GetAllEmployees();

        /// <summary>
        /// Add EMPLOYEE
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        Employee Add(Employee employee);

        /// <summary>
        /// Update Employee Data
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        Employee Update(Employee employee);

        /// <summary>
        /// Delete Employee By Request Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Employee Delete(int id);
    }
}
