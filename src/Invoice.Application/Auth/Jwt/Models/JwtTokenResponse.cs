using System;
using System.Collections.Generic;
using System.Text;

namespace Invoice.Application.Auth.Jwt.Models
{
    public class JwtTokenResponse
    {
        public string Id { get; set; }
        public string AuthToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
