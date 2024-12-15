using FluentValidation;
using FluentValidation.Validators;
using PiketWebApi.Data;

namespace PiketWebApi.Validators
{
    internal class SchoolYearValidator : AbstractValidator<SchoolYear>
    {
        public SchoolYearValidator()
        {
            RuleFor(x => x.Year).NotNull().WithMessage("Tahun tidak boleh kosong")
                .GreaterThan(DateTime.Now.Year - 1)
                .WithMessage($"Tahun tidak boleh lebih besar dari {DateTime.Now.Year - 1}");
            RuleFor(x => x.Semester).LessThanOrEqualTo(2).WithMessage("Semester hanya boleh 1  atau 2");
        }
    }
}