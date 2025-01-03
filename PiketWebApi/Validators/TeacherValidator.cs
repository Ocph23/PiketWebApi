using FluentValidation;
using PiketWebApi.Data;

namespace PiketWebApi.Validators
{
    internal class TeacherValidator :AbstractValidator<Teacher>
    {
        public TeacherValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Nama siswa tidak boleh kosong");
            RuleFor(x => x.RegisterNumber).NotEmpty().WithMessage("NIP/Nomor Induk guru tidak boleh kosong");
            RuleFor(x => x.DateOfBorn).NotEmpty().WithMessage("Tanggal lahir tidak boleh kosong");
            RuleFor(x => x.PlaceOfBorn).NotEmpty().WithMessage("Tempat lahir tidak boleh kosong");
            RuleFor(user => user.Email).NotEmpty().WithMessage("Email tidak boleh kosong")
            .EmailAddress().WithMessage("Email tidak valid");
        }
    }
}