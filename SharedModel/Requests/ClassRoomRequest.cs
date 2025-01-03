using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Requests
{
    public record ClassRoomRequest(string Name, int DepartmentId,int Level, int ClassRommLeaderId, int HomeRoomTeacherId);
    public record ClassRoomFromLastClassRequest(int ClassRoomId,int Level, string Name);
}
