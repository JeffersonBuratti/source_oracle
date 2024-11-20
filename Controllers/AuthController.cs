using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Oracle.ManagedDataAccess.Client;
using source_oracle.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace source_oracle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly string _connectionString = "SuaStringDeConexao";  // Substitua com a sua string de conexão

        // Endpoint para Login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            // Validação do usuário e senha
            if (ValidateUserCredentials(loginModel.Username, loginModel.Password))
            {
                // Gerar o JWT
                var token = GenerateJwtToken(loginModel.Username);
                return Ok(new { token });
            }
            else
            {
                return Unauthorized("Credenciais inválidas.");
            }
        }

        // Método para validar o usuário no banco
        private bool ValidateUserCredentials(string username, string password)
        {
            using (var conn = new OracleConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new OracleCommand("SELECT COUNT(1) FROM USUARIOS WHERE USUARIO = :username AND SENHA = :password", conn))
                {
                    cmd.Parameters.Add(":username", OracleDbType.Varchar2).Value = username;
                    cmd.Parameters.Add(":password", OracleDbType.Varchar2).Value = password;

                    var result = Convert.ToInt32(cmd.ExecuteScalar());
                    return result > 0;
                }
            }
        }

        // Método para gerar o JWT
        private string GenerateJwtToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("SuaChaveSecretaAqui"));  // Substitua pela sua chave secreta
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                issuer: "SeuIssuer",  // Defina o issuer
                audience: "SuaAudience",  // Defina a audiência
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
