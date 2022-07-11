
using Invoice.Application.Auth.Jwt.Models;

namespace Invoice.Application.Auth.Login
{
    public class LoginResponse
    {
        public bool Succeeded { get; set; }
        public string UserName { get; set; }
        public JwtTokenResponse TokenResponse { get; set; }        

    }
}