using System.Text;

namespace SharedModel.Responses;

public class LateAndGoHomeEarlyResponse
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? StudentPhoto { get; set; }
    public AttendanceStatus AttendanceStatus { get; set; }
    public LateAndGoHomeEarlyAttendanceStatus LateAndGoHomeEarlyStatus { get; set; }
    public string? Description { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public string? TeacherPhoto { get; set; }
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public TimeSpan? Time { get; set; } = DateTime.Now.TimeOfDay;
    public int? ClassRoomId { get; set; }
    public string? ClassRoomName { get; set; }
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string? StudentInitial => GetInitial(StudentName);
    public string? TeacherInitial => GetInitial(TeacherName);

    private string? GetInitial(string? name)
    {
        if (string.IsNullOrEmpty(name))
            return string.Empty;

        StringBuilder sb = new StringBuilder();
        foreach (var item in name.Split(" "))
        {
            sb.Append(item.Substring(0, 1).ToUpper());
        }
        return sb.ToString();
    }


}
