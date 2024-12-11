using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Responses
{
    public record ClassRoomResponse(
        int Id,
        string ClassName,
        int SchoolYearId,
        int Year,
        int DepartmentId,
        string DepartmentName,
        string DepartmentInitial,
        int ClassLeaderId,
        string ClassLeaderName,
        int HomeRoomTeacherId,
        string HomeRoomTeacherName,
        IEnumerable<object> Students
        );
}
