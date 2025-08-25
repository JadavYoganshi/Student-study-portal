using SSP.Models.Domain;
using System.Collections.Generic;

namespace SSP.Models.ViewModels
{
    public class StudentRecordViewModel
    {
        public List<Homework> Homeworks { get; set; }
        public List<Todo> Todos { get; set; }
       
    }
}
