namespace SharedModel.Responses;

public record ClassRoomResponse(
    int Id,
    string ClassName,
    int Level,
    int SchoolYearId,
    int Year,
    int DepartmentId,
    string DepartmentName,
    string DepartmentInitial,
    int ClassLeaderId,
    string ClassLeaderName,
    int HomeRoomTeacherId,

    string HomeRoomTeacherName,
    IEnumerable<object> Students
    );
