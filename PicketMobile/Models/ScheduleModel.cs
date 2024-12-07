using SharedModel.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicketMobile.Models
{
    public class ScheduleModel
    {
        public string Day { get; set; }
        public ICollection<ScheduleResponse> Members { get; set; } = new List<ScheduleResponse>();

    }

  
}
