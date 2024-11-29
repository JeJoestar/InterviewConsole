using EmployeeService.Data;
using System.Threading.Tasks;

namespace EmployeeService.Code
{
    public interface IEmployeeDataService
    {
        Task<EmployeeDto> GetEmployeeByIDAsync(int employeeId);

        Task EnableEmployeeAsync(int employeeId, bool newEnableValue);
    }
}