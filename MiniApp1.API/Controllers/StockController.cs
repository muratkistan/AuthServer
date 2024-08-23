using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiniApp1.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        //[Authorize(Policy = "AgePolicy")]
        //[Authorize(Roles = "admin,manager", Policy = "IstanbulPolicy")]
        [HttpGet]
        public IActionResult GetStock()
        {
            var userName = User.Identity.Name;
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return Ok($"Stock Islemleri => Username: {userName} - UserId: {userIdClaim.Value}");
        }
    }
}