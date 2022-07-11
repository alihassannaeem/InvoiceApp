using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Invoice.Application.Auth.Identity;
using Invoice.Application.Auth.Jwt;
using Invoice.Application.Auth.Login;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Invoice.Application.Auth.Login
{
    public class LoginCommand : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly IJwtFactory _jwtFactory;
        private readonly UserManager<AppUser> _userManager;

        public LoginCommand(UserManager<AppUser> userManager, IJwtFactory jwtFactory)
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
        }

        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                return new LoginResponse
                {
                    Succeeded = false
                };

            var jwt = await _jwtFactory.GenerateTokenForUser(user);            

            return new LoginResponse
            {
                UserName = "Admin",
                Succeeded = true,
                TokenResponse = jwt
            };
        }
    }
}