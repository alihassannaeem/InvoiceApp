using FluentValidation;
using Invoice.Application.Auth.Login;
using MediatR;

namespace Invoice.Application.Auth.Login
{
    public class LoginRequest : IRequest<LoginResponse>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
    }
}