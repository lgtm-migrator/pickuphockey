﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace pickuphockey.Models
{
    public enum PaymentPreference
    {
        Unknown,
        Cash,
        PayPal,
        Check
    }

    public enum TeamAssignment
    {
        Unassigned,
        Light,
        Dark
    }

    public enum NotificationPreference
    {
        [Display(Name = @"None")]
        None,
        [Display(Name = @"All")]
        All,
        [Display(Name = @"Only My Buy/Sells")]
        OnlyMyBuySell
    }

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName("Payment Preference")]
        public PaymentPreference PaymentPreference { get; set; }

        [DisplayName("Team Assignment")]
        public TeamAssignment TeamAssignment { get; set; }

        [DisplayName("Notification Preference")]
        public NotificationPreference NotificationPreference { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public void AddActivity(int sessionId, string activity)
        {
            var userid = Thread.CurrentPrincipal.Identity.GetUserId();
            
            var activitylog = ActivityLogs.Create();

            activitylog.UserId = userid;
            activitylog.SessionId = sessionId;
            activitylog.Activity = activity;
            activitylog.CreateDateTime = DateTime.UtcNow;

            Entry(activitylog).State = EntityState.Added;
            SaveChanges();
        }

        public DbSet<Session> Sessions { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<BuySell> BuySell { get; set; }
    }
}