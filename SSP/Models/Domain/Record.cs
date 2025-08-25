using System;

namespace SSP.Models.Domain
{
    public class Record
    {
        public int R_Id { get; set; }  // Primary key for Record
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public Guid S_Id { get; set; }
    }
}
