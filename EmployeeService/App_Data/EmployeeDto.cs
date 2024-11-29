using System.Collections.Generic;

namespace EmployeeService.Data
{
    public class EmployeeDto
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int? ManagerID { get; set; }

        public List<EmployeeDto> Subordinates { get; set; } = new List<EmployeeDto>();
    }
}
