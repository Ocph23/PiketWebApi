using CommunityToolkit.Mvvm.ComponentModel;
using SharedModel.Models;
using SharedModel;
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
        TimeOnly? startAt = TimeOnly.FromTimeSpan(new TimeSpan(7, 15, 0));

        [ObservableProperty]
        TimeOnly? endAt;

        [ObservableProperty]
        Teacher? createdBy;

        [ObservableProperty]
        DateTime createAt = DateTime.Now.ToUniversalTime();

        [ObservableProperty]
        List<Teacher> teacherAttendance = new();

        [ObservableProperty]
        List<StudentComeHomeEarly> studentsComeHomeEarly = new();

        [ObservableProperty]
        List<StudentToLate> studentsToLate = new();
       

    }
}
