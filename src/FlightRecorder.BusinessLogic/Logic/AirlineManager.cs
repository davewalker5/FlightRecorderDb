﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlightRecorder.BusinessLogic.Extensions;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightRecorder.BusinessLogic.Logic
{
    internal class AirlineManager : IAirlineManager
    {
        private FlightRecorderDbContext _context;

        internal AirlineManager(FlightRecorderDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Airline Get(Expression<Func<Airline, bool>> predicate)
            => List(predicate, 1, 1).FirstOrDefault();

        /// <summary>
        /// Get the first airline matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Airline> GetAsync(Expression<Func<Airline, bool>> predicate)
        {
            List<Airline> airlines = await _context.Airlines
                                                   .Where(predicate)
                                                   .ToListAsync();
            return airlines.FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Airline> List(Expression<Func<Airline, bool>> predicate, int pageNumber, int pageSize)
        {
            IEnumerable<Airline> results;
            if (predicate == null)
            {
                results = _context.Airlines;
                results = results.Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize);
            }
            else
            {
                results = _context.Airlines
                                  .Where(predicate)
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize); ;
            }

            return results;
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IAsyncEnumerable<Airline> ListAsync(Expression<Func<Airline, bool>> predicate, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Airline> results;
            if (predicate == null)
            {
                results = _context.Airlines;
                results = results.Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .AsAsyncEnumerable();
            }
            else
            {
                results = _context.Airlines.Where(predicate).AsAsyncEnumerable();
            }

            return results;
        }

        /// <summary>
        /// Add a named airline, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Airline Add(string name)
        {
            name = name.CleanString();
            Airline airline = Get(a => a.Name == name);

            if (airline == null)
            {
                airline = new Airline { Name = name };
                _context.Airlines.Add(airline);
                _context.SaveChanges();
            }

            return airline;
        }

        /// <summary>
        /// Add a named airline, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Airline> AddAsync(string name)
        {
            name = name.CleanString();
            Airline airline = await GetAsync(a => a.Name == name);

            if (airline == null)
            {
                airline = new Airline { Name = name };
                await _context.Airlines.AddAsync(airline);
                await _context.SaveChangesAsync();
            }

            return airline;
        }
    }
}
