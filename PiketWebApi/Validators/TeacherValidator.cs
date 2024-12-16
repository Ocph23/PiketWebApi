using FluentValidation;
using PiketWebApi.Data;

namespace PiketWebApi.Validators
{
    internal class TeacherValidator :AbstractValidator<Teacher>
    {
        public TeacherValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Nama siswa tidak boleh kosong");
            RuleFor(x => x.RegisterNumber).NotEmpty().WithMessage("Nis tidak boleh kosong");
            RuleFor(x => x.DateOfBorn).NotEmpty().WithMessage("Tanggal lahir tidak boleh kosong");
            RuleFor(x => x.PlaceOfBorn).NotEmpty().WithMessage("Tempat lahir tidak boleh kosong");
            RuleFor(user => user.Email)
            .EmailAddress().WithMessage("Email tidak valid")
            .When(user => !string.IsNullOrEmpty(user.Email));
        }
    }
}