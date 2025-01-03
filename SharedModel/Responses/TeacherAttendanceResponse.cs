using System.Text;

namespace SharedModel.Responses;

public class TeacherAttendanceResponse
{
    public int Id { get; set; }
    public AttendanceStatus AttendanceStatus { get; set; }
    public string? Description { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public string? TeacherPhoto { get; set; }
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public TimeSpan? AtTime { get; set; } = DateTime.Now.TimeOfDay;
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
