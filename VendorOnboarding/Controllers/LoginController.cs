using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using VendorOnboarding.Models;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using Microsoft.AspNetCore.Cors;

namespace VendorOnboarding.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("VendorBoardingConnection");
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("sp_LoginUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@emailID", model.Email);
                        command.Parameters.AddWithValue("@Password", model.Password);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                                return Unauthorized(new { Message = "Invalid credentials" });

                            var user = new
                            {
                                UserID = reader.GetInt32("UserID"),
                                UserName = reader.GetString("UserName"),
                                Pswrd = reader.GetString("Pswrd")
                            };                                                     

                            var token = GenerateJwtToken(user);
                            return Ok(new AuthResponse
                            {
                                Token = token,
                                User = new UserDto
                                {
                                    UserID = user.UserID,
                                    UserName = user.UserName,
                                    Pswrd = user.Pswrd
                                }
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during login", Error = ex.Message });
            }
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("sp_RegisterUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@email_ID", model.Email);
                        command.Parameters.AddWithValue("@UserName", model.UserName);
                        command.Parameters.AddWithValue("@Pswrd", model.Password); // Hash the password
                        var userIdParam = new SqlParameter("@UserID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(userIdParam);
                        var resultParam = new SqlParameter("@Result", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(resultParam);

                        await command.ExecuteNonQueryAsync();

                        int result = (int)resultParam.Value;
                        if (result == -1)
                            return BadRequest(new { Message = "User already exists" });

                        var user = new
                        {
                            UserName = model.UserName,
                            email_ID = model.Email
                        };

                        return Ok(new AuthResponse
                        {                            
                            User = new UserDto
                            {
                                UserName = user.UserName
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during registration", Error = ex.Message });
            }
        }

        private string GenerateJwtToken(dynamic user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.email_ID),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new Claim(ClaimTypes.Role, user.UserType)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
