using FluentValidation;
using PiketWebApi.Data;
using SharedModel.Requests;

namespace PiketWebApi.Validators
{
    internal class ScheduleValidator : AbstractValidator<ScheduleRequest>
    {
        public ScheduleValidator()
        {
            RuleFor(x => x.TeacherId).NotEmpty().WithMessage("Guru tidak boleh kosong");
            RuleFor(x => x.DayOfWeek).LessThan(0).WithMessage("Hari tidak boleh kosong")
                .GreaterThan(6).WithMessage("Hari tidak boleh kosong");
        }
    }
}