using Microsoft.IdentityModel.Tokens; 
using System.IdentityModel.Tokens.Jwt; 
using System.Security.Claims; 
using System.Text; 

namespace MvcFlowers.Models 
{
    public class AuthOptions 
    {
        // Указывает, кто выдает токен
        public static string Issuer => "TM";

        // Указывает, для кого предназначен токен
        public static string Audience => "APIclients";

        // Указывает срок действия токена в годах
        public static int LifetimeInYears => 1;

        // Определяет ключ для подписи токена, создаваемый из строки
        public static SecurityKey SigningKey => new SymmetricSecurityKey(Encoding.ASCII.GetBytes("superSecretKeyMustBeLoooooong"));

        // Метод для генерации JWT-токена, принимает параметр is_admin для определения роли пользователя
        internal static object GenerateToken(bool is_admin = false)
        {
            
            var now = DateTime.UtcNow;

            // Создаем список утверждений (claims), которые будут включены в токен
            var claims = new List<Claim>
            {
                // Утверждение с именем пользователя
                new Claim(ClaimsIdentity.DefaultNameClaimType, "user"),
                
                // Утверждение с ролью пользователя (admin или guest)
                new Claim(ClaimsIdentity.DefaultRoleClaimType, is_admin ? "admin" : "guest")
            };

            // Создаем объект ClaimsIdentity, который содержит утверждения пользователя
            ClaimsIdentity identity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

            // Создаем JWT-токен
            var jwt = new JwtSecurityToken(
                issuer: Issuer, 
                audience: Audience, 
                notBefore: now, 
                expires: now.AddYears(LifetimeInYears), 
                claims: identity.Claims, 
                signingCredentials: new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256)); 

            // Кодируем токен в строку
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new { token = encodedJwt };
        }
    }
}
