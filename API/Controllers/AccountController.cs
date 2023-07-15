
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SQLitePCL;

namespace API.Controllers
{
    public class AccountController :BaseApiController
    {
        private DataContext _context;
        public AccountController(DataContext context)
        {
            _context = context;
        }
        [HttpPost("egister")]//Post : api/account/register?username=dave&pasvord=pwd

        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            if (await UserExist(registerDto.Username)) return BadRequest ("Username is taken");
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password )),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;

        }
        [HttpPost("login")]

        public async Task<ActionResult<AppUser>>Login(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x =>
               x.UserName == loginDto.Username);

               if (user == null) return Unauthorized("invalid Username");

               using var hmac = new HMACSHA512(user.PasswordSalt);

               var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

               for (int i =0; i<computedHash.Length ; i++ )
               {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("invalid password");

               }
               return user;

        }
        private async Task<bool> UserExist(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}