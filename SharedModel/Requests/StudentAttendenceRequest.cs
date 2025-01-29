using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Requests
{
    public record StudentAttendenceRequest(int Id, int StudentId, AttendanceStatus Status, DateTime? TimeIn, DateTime? TimeOut,  string? Description);
}
