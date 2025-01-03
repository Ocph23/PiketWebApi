using System.Text;

namespace SharedModel.Responses;

public record ScheduleResponse(int Id, int SchoolYearId, int Year, int Semester, string DayOfWeek, int TeacherId, string TeacherNumber, string TeacherName, string? Photo = null)
{
    public string TeacherInitial => GetInitial(TeacherName);

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

};
