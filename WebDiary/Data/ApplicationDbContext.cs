﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Providers.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebDiary.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace WebDiary.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Event> Event{ get; set; }


        // user ID from AspNetUser table.
        public int created_user_id { get; set; }

        public DateTime created_on { get; set; }
        // user ID from AspNetUser table.
        public int? modified_user_id { get; set; }

        public DateTime? modified_on { get; set; }

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var AddedEntities = ChangeTracker.Entries()
                .Where(E => E.State == EntityState.Added)
                .ToList();

            AddedEntities.ForEach(E =>
            {
                var prop = E.Metadata.FindProperty("created_on");
                if (prop != null)
                {
                    E.Property("created_on").CurrentValue = DateTime.Now;
                }
            });

            var EditedEntities = ChangeTracker.Entries()
                .Where(E => E.State == EntityState.Modified)
                .ToList();

            EditedEntities.ForEach(E =>
            { 
                var prop = E.Metadata.FindProperty("modified_on");
                if (prop != null)
                {
                    E.Property("modified_on").CurrentValue = DateTime.Now;

                    // to get current user ID
                    var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    E.Property("modified_user_id").CurrentValue = userId;
                }
            });

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }

}