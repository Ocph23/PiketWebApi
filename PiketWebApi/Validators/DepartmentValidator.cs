using FluentValidation;
using PiketWebApi.Data;

namespace PiketWebApi.Validators
{
    public class DepartmentValidator :AbstractValidator<Department>
    {
        public DepartmentValidator()
        {
            RuleFor(x=>x.Name).NotEmpty().WithMessage("Nama jurusan tidak boleh kosong.");
            RuleFor(x=>x.Initial).NotEmpty().WithMessage("Inisial jurusan tidak boleh kosong.");
            RuleFor(x=>x.Description).NotEmpty().WithMessage("Deskripsi jurusan tidak boleh kosong.");
        }
    }
}