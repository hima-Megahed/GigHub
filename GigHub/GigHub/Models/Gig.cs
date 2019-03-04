using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;

namespace GigHub.Models
{
    public class Gig
    {
        public int Id { get; set; }
        
        public ApplicationUser Artist { get; set; }

        public bool IsCanceled { get; private set; }

        [Required]
        public string ArtistId { get; set; }

        public DateTime DateTime { get; set; }

        [Required]
        [StringLength(255)]
        public string Venue { get; set; }
        
        public Genre Genre { get; set; }

        [Required]
        public byte GenreId { get; set; }

        public ICollection<Attendance> Attendances { get; }

        public Gig()
        {
            Attendances = new Collection<Attendance>();
        }

        public void Cancel()
        {
            IsCanceled = true;

            var notification = Notification.GigCanceled(this);

            foreach (var attendee in Attendances.Select(a => a.Attendee))
            {
                attendee.Notify(notification);
            }
        }

        public void Modify(DateTime viewModelDateTime, string viewModelVenue, byte viewModelGenre)
        {
            var notification = Notification.GigUpdated(this, this.DateTime, this.Venue);

            GenreId = viewModelGenre;
            Venue = viewModelVenue;
            DateTime = viewModelDateTime;

            foreach (var attendee in Attendances.Select(a => a.Attendee))
            {
                attendee.Notify(notification);
            }
        }
    }
}