using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GigHub.Models;
using GigHub.Repositories;

namespace GigHub.Persistence
{
    public class UnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public GigRepository GigRepository { get; private set; }
        public AttendanceRepository AttendanceRepository { get; private set; }
        public FollowingRepository FollowingRepository { get; private set; }
        public GenreRepository GenreRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            GigRepository = new GigRepository(_context);
            AttendanceRepository = new AttendanceRepository(_context);
            FollowingRepository = new FollowingRepository(_context);
            GenreRepository = new GenreRepository(_context);
        }

        public void Complete()
        {
            _context.SaveChanges();
        }
    }
}