using EmployeeService.Data;
using EmployeeService.Code;

namespace EmployeeService
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeDataService _employeeService;

        public EmployeeService()
        {
            _employeeService = new EmployeeDataService();
        }

        public EmployeeDto GetEmployeeById(int id)
        {

            return _employeeService.GetEmployeeByIDAsync(id).GetAwaiter().GetResult();
        }

      

        public void EnableEmployee(int id, int enable)
        {
            _employeeService.EnableEmployeeAsync(id, enable != 0).GetAwaiter().GetResult(); ;
        }
    }
}