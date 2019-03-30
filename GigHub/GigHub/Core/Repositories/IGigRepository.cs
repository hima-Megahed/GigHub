using System.Collections.Generic;
using GigHub.Core.Models;

namespace GigHub.Core.Repositories
{
    public interface IGigRepository
    {
        List<Gig> GetGigsUserAttending(string userId);
        Gig GetGigWithAttendees(int gigId);
        Gig GetGig(int gigId);
        List<Gig> GetUpcomingGigsByArtist(string userId);
        void AddGig(Gig gig);
    }
}