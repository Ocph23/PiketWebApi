using FluentValidation;
using FluentValidation.Validators;
using PiketWebApi.Data;
using SharedModel.Requests;
using System.ComponentModel.DataAnnotations;

namespace PiketWebApi.Validators
{
    public class PaginateRequestValidator : AbstractValidator<PaginationRequest>
    {

        public PaginateRequestValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0).WithMessage("Tentukan halaman");
            RuleFor(x => x.PageSize).GreaterThan(0).WithMessage("Tentukan jumlah baris per halaman");
        }
    }
}