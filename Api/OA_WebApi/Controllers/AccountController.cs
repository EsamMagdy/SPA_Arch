using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OA_Data;
using OA_Repo;
using OA_Service.DTOs;
using OA_Service.Interface;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly RoleManager<AppRole> _roleManager;

        // private readonly IEmailSender _emailSender;

        public AccountController(UserManager<AppUser> userManager,
                                RoleManager<AppRole> roleManager,
                                SignInManager<AppUser> signInManager,
                                ITokenService tokenService,
                                IMapper mapper,
                                ApplicationContext context

            )
        {
            _roleManager = roleManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _context = context;

        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(AppUserDto userDto)
        {
           if (!ModelState.IsValid) return BadRequest();

           //check Email
           var res = await CheckUser(userDto.Email);
           if (res) return Ok("username is already taken");

           var user = _mapper.Map<AppUserDto, AppUser>(userDto);

           var result = await _userManager.CreateAsync(user, userDto.Password);

           if (!result.Succeeded) return BadRequest(result.Errors);

           return new UserDto
           {
               Username = user.UserName,
               Email = user.Email,
               Token = await _tokenService.CreateToken(user),
               //IsActive = user.IsActive
           };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto) // password Esam*123456
        {
            var user = await _userManager.Users
                                    .Where(x => x.Email == loginDto.Email)
                                    .FirstOrDefaultAsync();

            if (user == null) return Unauthorized("من فضلك تاكد من البريد الالكترونى");

            var result = await _signInManager
                          .CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded) return Unauthorized("من فضلك تاكد من الرقم السرى");

            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                Email = user.Email
            };
        }

        private async Task<bool> CheckUser(string Email)
        {
            var user = await _context.Users.Where(x => x.UserName == Email).ToListAsync();
            return user.Count() > 0;
        }
    }
}