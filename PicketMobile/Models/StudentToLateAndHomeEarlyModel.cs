using CommunityToolkit.Mvvm.ComponentModel;
using SharedModel;
using SharedModel.Responses;

namespace PicketMobile.Models;

public class StudentToLateAndHomeEarlyModel : ObservableObject
{
    public int Id { get; set; }

    private StudentResponse? student;

    public StudentResponse? Student
    {
        get { return student; }
        set { SetProperty(ref student , value); }
    }

    private string description;

    public string Description
    {
        get { return description; }
        set { SetProperty(ref description , value); }
    }

    private TimeSpan atTime;

    public TimeSpan AtTime
    {
        get { return atTime; }
        set {SetProperty(ref atTime , value); }
    }


    private TeacherResponse createdBy;

    public TeacherResponse CreatedBy
    {
        get { return createdBy; }
        set { SetProperty(ref createdBy , value); }
    }

    public DateTime CreateAt { get; set; } = DateTime.Now;


    private AttendanceStatus statusKehadiran;

    public AttendanceStatus StatusKehadiran
    {
        get { return statusKehadiran; }
        set { SetProperty(ref statusKehadiran , value); }
    }


    public LateAndGoHomeEarlyAttendanceStatus LateAndGoHomeEarlyStatus { get; set; }



}
