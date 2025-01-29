using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Responses
{
    public record StudentAttendanceResponse(
        int Id,
        int StudentId,
        string StudentName,
        string ClassName,
        string DepartmentName,
        AttendanceStatus Status,
        DateTime? TimeIn,
        DateTime? TimeOut,
        string? Description,
        DateTime? CreateAt);
}
