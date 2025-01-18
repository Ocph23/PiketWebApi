using System;

namespace SharedModel.Models;
public record PicketReportModel(
    int Id,
    DateOnly Date, 
    Weather Weather,
    TimeSpan? StartAt,
    TimeSpan? EndAt,
    string? CreatedName,
    string? CreatedNumber,
    DateTime CreateAt,
    int TotalStudentLAte,
    int TotalStudenGoHomeEarly
);
