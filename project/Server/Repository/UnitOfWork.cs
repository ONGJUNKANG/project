﻿using project.Server.Data;
using project.Server.IRepository;
using project.Server.Models;
using project.Shared.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace project.Server.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IGenericRepository<TypeOfVehicle> _typeOfVehicle;
        private IGenericRepository<BookingDetails> _bookingDetails;
        private IGenericRepository<Flights> _flights;
        private IGenericRepository<Card> _card;
        private IGenericRepository<Booking> _booking;

        private UserManager<ApplicationUser> _userManager;

        public UnitOfWork(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IGenericRepository<TypeOfVehicle> TypeOfVehicle
            => _typeOfVehicle ??= new GenericRepository<TypeOfVehicle>(_context);
        public IGenericRepository<BookingDetails> BookingDetails
            => _bookingDetails ??= new GenericRepository<BookingDetails>(_context);
        public IGenericRepository<Flights> Flights
            => _flights ??= new GenericRepository<Flights>(_context);
        public IGenericRepository<Card> Card
            => _card ??= new GenericRepository<Card>(_context);
        public IGenericRepository<Booking> Booking
            => _booking ??= new GenericRepository<Booking>(_context);

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task Save(HttpContext httpContext)
        {
            //To be implemented
            string user = "System";

            var entries = _context.ChangeTracker.Entries()
                .Where(q => q.State == EntityState.Modified ||
                    q.State == EntityState.Added);

            foreach (var entry in entries)
            {
                ((BaseDomainModel)entry.Entity).DateUpdated = DateTime.Now;
                ((BaseDomainModel)entry.Entity).UpdatedBy = user;
                if (entry.State == EntityState.Added)
                {
                    ((BaseDomainModel)entry.Entity).DateCreated = DateTime.Now;
                    ((BaseDomainModel)entry.Entity).CreatedBy = user;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}