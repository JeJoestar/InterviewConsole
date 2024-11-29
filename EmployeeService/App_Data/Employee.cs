using System.Collections.Generic;
using System.Linq;

namespace EmployeeService.Data
{
    public class Employee
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int? ManagerID { get; set; }

        public bool Enable { get; set; }

        public List<Employee> Subordinates { get; set; } = new List<Employee>();

        public EmployeeDto ToDto()
        {
            return new EmployeeDto
            {
                ID = ID,
                ManagerID = ManagerID,
                Name = Name,
                Subordinates = Subordinates.Select(x => x.ToDto()).ToList(),
            };
        }
    }
}
