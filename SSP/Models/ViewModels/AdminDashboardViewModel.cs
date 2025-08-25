using SSP.Models.Domain;
using System.Collections.Generic;

namespace SSP.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public List<AuditLog> AuditLogs { get; set; }
        public List<Student> Students { get; set; }
        
    }
}
