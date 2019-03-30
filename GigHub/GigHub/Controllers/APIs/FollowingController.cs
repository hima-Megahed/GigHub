using System.Linq;
using System.Web.Http;
using GigHub.Core.Dtos;
using GigHub.Core.Models;
using GigHub.Persistence;
using Microsoft.AspNet.Identity;

namespace GigHub.Controllers.APIs
{
    public class FollowingController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public FollowingController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpPost]
        public IHttpActionResult Follow(FollowingDto dto)
        {
            var userId = User.Identity.GetUserId();

            if (_context.Followings.Any(f => f.FollowerId == userId && f.FolloweeId == dto.FolloweeId))
                return BadRequest("Following already exists.");

            var following = new Following
            {
                FollowerId = userId,
                FolloweeId = dto.FolloweeId
            };

            _context.Followings.Add(following);
            _context.SaveChanges();

            return Ok();
        }
    }
}
