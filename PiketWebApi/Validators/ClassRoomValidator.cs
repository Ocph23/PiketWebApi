using FluentValidation;
using PiketWebApi.Data;
using SharedModel.Requests;

namespace PiketWebApi.Validators
{
    public class ClassRoomValidator:AbstractValidator<ClassRoomRequest>
    {
        public ClassRoomValidator()
        {
            RuleFor(x=> x.Name).NotEmpty().WithMessage("Nama Kelas Tidak Boleh Kosong");
            RuleFor(x=> x.DepartmentId).GreaterThan(0).WithMessage("Jurusan Tidak Boleh Kosong");
            RuleFor(x=> x.HomeRoomTeacherId).GreaterThan(0).WithMessage("Wali Kelas Tidak Boleh Kosong");
            RuleFor(x=> x.ClassRommLeaderId).GreaterThan(0).WithMessage("Ketua Kelas Tidak Boleh Kosong");
        }
    }
}
