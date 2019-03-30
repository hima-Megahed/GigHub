using System.Collections.Generic;
using GigHub.Core.Models;

namespace GigHub.Core.Repositories
{
    public interface IAttendanceRepository
    {
        List<Attendance> GetFutureAttendances(string userId);
        Attendance GetAttendance(int gigId, string userId);
        void RemoveAttendance(Attendance attendance);
    }
}