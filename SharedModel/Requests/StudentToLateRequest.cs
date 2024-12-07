using SharedModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Requests
{
    public record StudentToLateAndEarlyRequest(int StudentId, TimeSpan AtTime, string Description, StudentAttendanceStatus StudentAttendance);
}
