using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using GigHub.Models;

namespace GigHub.Repositories
{
    public class GigRepository
    {
        private readonly ApplicationDbContext _context;

        public GigRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Gig> GetGigsUserAttending(string userId)
        {
            return _context.Attendances
                .Where(a => a.AttendeeId == userId)
                .Select(a => a.Gig)
                .Include(g => g.Artist)
                .Include(g => g.Genre)
                .ToList();
        }

        public Gig GetGigWithAttendees(int gigId)
        {
            return _context.Gigs
                 .Include(g => g.Attendances.Select(a => a.Attendee))
                 .SingleOrDefault(g => g.Id == gigId);
        }

        public Gig GetGig(int gigId)
        {
            return _context.Gigs
                .Include(g => g.Artist)
                .Include(g => g.Genre)
                .SingleOrDefault(g => g.Id == gigId);
        }

        public List<Gig> GetUpcomingGigsByArtist(string userId)
        {
            return _context.Gigs.
                Where(a => a.ArtistId == userId && a.DateTime > DateTime.Now && a.IsCanceled == false)
                .Include(a => a.Genre)
                .ToList();
        }

        public void AddGig(Gig gig)
        {
            _context.Gigs.Add(gig);
        }
    }
}