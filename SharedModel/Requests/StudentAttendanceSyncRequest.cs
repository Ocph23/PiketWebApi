using SharedModel;
using SharedModel.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Requests
{
    public class StudentAttendanceSyncRequest
    {
        public Guid Id { get; set; }
        public int StudentId { get; set; }
        public AttendanceStatus Status { get; set; }
        public DateTime? TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public string? Description { get; set; }
        public bool IsSynced { get; set; }
    }
}
