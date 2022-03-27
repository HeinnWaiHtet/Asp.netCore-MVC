namespace Asp.netCore_MVC.Models
{
    public interface IEmployeeRepository
    {
        public Employee GetEmployeeById(int id);

        IEnumerable<Employee> GetAllEmployees();

        Employee Add(Employee employee);

        Employee Update(Employee employee);

        Employee Delete(int id);
    }
}
