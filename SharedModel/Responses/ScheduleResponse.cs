using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Responses
{
    public record ScheduleResponse(int Id, int SchoolYearId, int Year, int Semester,  string DayOfWeek, int TeacherId, string TeacherNumber, string TeacherName, string? Photo=null);
}
