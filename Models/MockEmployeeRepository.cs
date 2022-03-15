namespace Asp.netCore_MVC.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        #region Properties
        private readonly IList<Employee> _employeeList;
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor For MorkEmployeeRepository
        /// </summary>
        public MockEmployeeRepository()
        {
            _employeeList = new List<Employee>()
            {
                new Employee(){ Id = 1, Name ="Hein Wai Htet",Department = "IT"},
                new Employee(){ Id = 2, Name = "Aye Chan May",Department = "HR"},
                new Employee(){ Id = 3, Name = "HWH",Department = "System Architect"},
                new Employee(){ Id = 4, Name = "ACM", Department = "Chit Thu"}
            };
        }

        /// <summary>
        /// Get Alll Employee Data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Employee> GetAllEmployees() => this._employeeList;

        /// <summary>
        /// Get Employee By Request Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Employee</returns>
        public Employee GetEmployeeById(int id)
        {
            return _employeeList?.FirstOrDefault(emp => emp.Id == id) ?? new Employee();
        }
        #endregion
    }
}
