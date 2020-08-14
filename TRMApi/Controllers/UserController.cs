using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TRMApi.Data;
using TRMApi.Models;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMApi.Controllers
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class UserController : ControllerBase
  {
    private readonly ApplicationDbContext _context;
    //private readonly IConfiguration _config;
    private readonly IUserData _userData;
    private readonly ILogger<UserController> _logger;

    public UserManager<IdentityUser> _userManager { get; }

    public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager,
      IConfiguration config, IUserData userData, ILogger<UserController> logger)
    {
      _context = context;
      _userManager = userManager;
      //_config = config;
      _userData = userData;
      _logger = logger;
    }


    [HttpGet]
    public UserModel GetById()
    {
      //UserData data = new UserData(_config);
      //RequestContext.Principal.Identity.GetUserId();
      //var userId = _userManager.Users.First().Id;
      //var userId1 = _userManager.GetUserId(User);

      string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      return _userData.GetUserById(userId).First();
    }


    [Authorize(Roles = "Admin")]
    [HttpGet]
    [Route("Admin/GetAllUsers")]
    public List<ApplicationUserModel> GetAllUsers()
    {
      List<ApplicationUserModel> output = new List<ApplicationUserModel>();

      var users = _context.Users.ToList();
      var userRoles = from ur in _context.UserRoles
                      join r in _context.Roles on ur.RoleId equals r.Id
                      select new { ur.UserId, ur.RoleId, r.Name };

      foreach (var user in users)
      {
        ApplicationUserModel u = new ApplicationUserModel()
        {
          Id = user.Id,
          Email = user.Email
        };

        u.Roles = userRoles.Where(x => x.UserId == u.Id).ToDictionary(key => key.RoleId, val => val.Name);

        //foreach (var r in user.Roles)
        //{
        //  u.Roles.Add(r.RoleId, roles.Where(x => x.Id == r.RoleId).First().Name);
        //}

        output.Add(u);
      }

      return output;
    }

    //LinQ
    //var result =
    //                (from user in users
    //                 select new ApplicationUserModel
    //                 {
    //                   Id = user.Id,
    //                   Email = user.Email,
    //                   Roles =
    //                         (from userRole in user.Roles
    //                          join role in roles on userRole.RoleId equals role.Id
    //                          select new { userRole.RoleId, role.Name }).ToDictionary(o => o.RoleId, o => o.Name)
    //                 }).ToList();


    [Authorize(Roles = "Admin")]
    [HttpGet]
    [Route("Admin/GetAllRoles")]
    public Dictionary<string, string> GetAllRoles()
    {
      var roles = _context.Roles.ToDictionary(x => x.Id, x => x.Name);

      return roles;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Route("Admin/AddRole")]
    public async Task AddRoleAsync(UserRolePairModel pairing)
    {
      string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      var loggerInUser = _userData.GetUserById(loggedInUserId).First();

      var user = await _userManager.FindByIdAsync(pairing.UserId);

      _logger.LogInformation("Admin {Admin} added user {User} to role {Role}", 
        loggedInUserId, user.Id, pairing.RoleName);

      await _userManager.AddToRoleAsync(user, pairing.RoleName);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Route("Admin/RemoveRole")]
    public async Task RemoveRoleAsync(UserRolePairModel pairing)
    {
      string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      var loggerInUser = _userData.GetUserById(loggedInUserId).First();

      var user = await _userManager.FindByIdAsync(pairing.UserId);

      _logger.LogInformation("Admin {Admin} remove user {User} from role {Role}",
       loggedInUserId, user.Id, pairing.RoleName);

      await _userManager.RemoveFromRoleAsync(user, pairing.RoleName);
    }
  }
}
