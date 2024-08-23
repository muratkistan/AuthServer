using AuthServer.Core.Dtos;
using FluentValidation;

namespace AuthServer.API.Validation
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Kullanıcı adı boş olamaz");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Parola boş olamaz");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email boş olamaz").EmailAddress().WithMessage("Geçerli bir email adresi giriniz");
        }
    }
}