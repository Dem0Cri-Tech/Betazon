using Betazon.Migrations;
using Betazon.Models;
using Hashing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{

    IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("C:\\Users\\daleo\\source\\repos\\Betazon\\Betazon\\appsettings.json", false, true);
    IConfigurationRoot root;

    private readonly AdventureWorksLt2019Context _context;
    private readonly Encrypt  _encrypt;


    public AuthController(AdventureWorksLt2019Context context)
    {
        _encrypt = new Encrypt();
        _context = context;
        root = builder.Build();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] TokenModel user)
    {
        
        //var value; This will be the substitute once implemented
       
        if (user is null)
        {
            return BadRequest("Invalid client request");
        }
        if(await VerifyCredentials(user)) //if (user.Email == "pippo" && user.Password == "baudo") This will be the substitute once implemented
            {
            Customer customer = await FindUser(user);
            Credentials cred = await FindRole(customer);

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(root.GetSection("SecretKey")["Key"]));// GetBytes("SyU4@SK!3@zrT4%&")
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(ClaimTypes.Role, cred.Role.ToString()),
                    new Claim(ClaimTypes.Surname,customer.LastName),
                    new Claim(ClaimTypes.Name,customer.FirstName)
                };

            var tokeOptions = new JwtSecurityToken(
                issuer: "https://localhost:7157",
                audience: "https://localhost:7157",
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: signinCredentials
            ); ;
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            // we're gonna move the jwt from local to cookie httponly
            //HttpContext.Response.Cookies.Append("access_token", tokenString, new CookieOptions { HttpOnly = true });
            
            return Ok(new TokenResponse { Token = tokenString });
         
        }
        
        
        return Unauthorized();
    }

    private async Task<bool>  VerifyCredentials(TokenModel user)// VerifyCredentials(TokenModel user,out string value)TO IMPLEMENT ONCE A CUSTOMER IS CREATED
    {
       
            try
            {
                var customer = await _context.Customers
                   .AsNoTracking()
                   .Where(customer => customer.EmailAddress == user.Email)
                   .FirstOrDefaultAsync();

                if (customer == null)
                    return false;


                if (_encrypt.Hash(user.Password, customer.PasswordSalt) == customer.PasswordHash)
                    return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        
        
        return false;

    }

    private async Task<Customer> FindUser(TokenModel user)// TO FIND THE USER ROLE CONNECTED TO THE "USER" WHICH IS A RECORD IN THE CUSTOMER TABLE
    {
        
            try
            {
                return await _context.Customers
                   .AsNoTracking()
                   .Where(customer => customer.EmailAddress == user.Email)
                   .Include(credentials => credentials.Credentials)
                   .FirstOrDefaultAsync();

               
            }
            catch(Exception ex)
            {
                return null;
            }
        

        return null;

    }

    private async Task<Credentials> FindRole(Customer customer)// TO FIND THE USER ROLE CONNECTED TO THE "USER" WHICH IS A RECORD IN THE CUSTOMER TABLE
    {

        try
        {
            return await _context.Credentials
               .AsNoTracking()
               .Where(credentials => credentials.CustomerId == customer.CustomerId)
               .FirstOrDefaultAsync();


        }
        catch (Exception ex)
        {
            return null;
        }


        return null;

    }
}