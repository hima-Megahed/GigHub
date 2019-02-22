using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GigHub.Models;
using Microsoft.AspNet.Identity;

namespace GigHub.Controllers.API
{
    [Authorize]
    public class GigsController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public GigsController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpDelete]
        public IHttpActionResult Cancel(int gigId)
        {
            var userId = User.Identity.GetUserId();
            var gig = _context.Gigs.
                Single(g => g.Id == gigId && g.ArtistId == userId);

            gig.IsCanceled = true;

            _context.SaveChanges();

            return Ok();
        }
    }
}
