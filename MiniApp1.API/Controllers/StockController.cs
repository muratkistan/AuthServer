using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiniApp1.API.Controllers
{
    //[Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        //[Authorize(Policy = "AgePolicy")]
        [Authorize(Roles = "admin,manager", Policy = "AnkaraPolicy")]
        [HttpGet]
        public IActionResult GetStock()
        {
            var userName = User.Identity.Name;
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return Ok($"Stock Islemleri => Username: {userName} - UserId: {userIdClaim.Value}");
        }
    }
}