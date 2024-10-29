using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Requests
{
    public record ScheduleRequest(int Id, int DayOfWeek, int TeacherId );
    
}
