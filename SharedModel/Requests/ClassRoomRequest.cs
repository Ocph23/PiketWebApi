using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Requests
{
    public record ClassRoomRequest(string Name, int DepartmentId, int ClassRommLeaderId, int HomeRoomTeacherId);
}
