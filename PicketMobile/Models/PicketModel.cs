using CommunityToolkit.Mvvm.ComponentModel;
using SharedModel;
using SharedModel.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicketMobile.Models
{
    public partial class PicketModel : ObservableObject
    {

        [ObservableProperty]
        int id;

        [ObservableProperty]
        DateOnly date = DateOnly.FromDateTime(DateTime.Now);

        [ObservableProperty]
        Weather weather;

        [ObservableProperty]
        TimeSpan? startAt = new TimeSpan(7, 15, 0);

        [ObservableProperty]
        TimeSpan? endAt;

        [ObservableProperty]
        TeacherResponse? createdBy;

        [ObservableProperty]
        DateTime createAt = DateTime.Now.ToUniversalTime();

        public ICollection<TeacherAttendanceResponse> TeacherAttendance { get; set; } = default;
        public ICollection<LateAndGoHomeEarlyResponse> LateAndComeHomeEarly { get; set; } = default;

    }
}
