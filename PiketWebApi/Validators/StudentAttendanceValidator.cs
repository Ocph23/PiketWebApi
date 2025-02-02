using FluentValidation;
using FluentValidation.Validators;
using PiketWebApi.Data;

namespace PiketWebApi.Validators
{
    internal class StudentAttendanceValidator : AbstractValidator<StudentAttendance>
    {
        public StudentAttendanceValidator()
        {
            RuleFor(x => x.Student).NotEmpty().WithMessage("Siswa tidak boleh kosong");
            RuleFor(x => x.AttendanceStatus).NotEmpty().WithMessage("Status Kehadiran tidak boleh kosong");
            RuleFor(x => x.TimeIn).NotEmpty().WithMessage("Jam Masuk");
        }
    }
}