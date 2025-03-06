﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Stripe;
using The_Post.Data;
using The_Post.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;


namespace The_Post.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        public SubscriptionService(ApplicationDbContext db, UserManager<User> userManager, IEmailSender emailSender)
        {
            _db = db;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        async Task<Models.Subscription?> ISubscriptionService.AddSubscription(string userId, int subscriptionTypeId)
        {
            // Retrieve the subscription type details
            var subscriptionType = await _db.SubscriptionTypes.FindAsync(subscriptionTypeId);
            if (subscriptionType == null)
            {
                return null;
            }

            // Create a new subscription record
            var subscription = new The_Post.Models.Subscription
            {
                SubscriptionTypeId = subscriptionTypeId,
                HistoricalPrice = subscriptionType.Price,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMonths(1),
                PaymentComplete = true, // Since payment was confirmed via success handler
                UserId = userId
            };

            _db.Subscriptions.Add(subscription);
            await _db.SaveChangesAsync();

            // Assign the "Subscriber" role to the user
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }
            // Add the Subscriber role while keeping the Member role intact.
            var addRoleResult = await _userManager.AddToRoleAsync(user, "Subscriber");
            if (!addRoleResult.Succeeded)
            {
                return null;
            }

            return subscription;
        }

        public async Task SendConfirmationEmailAsync(string userId, int subscriptionTypeId, string baseUrl)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            // Generate a confirmation token
            var token = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "SubscriptionConfirmation");
            var confirmationLink = $"{baseUrl}/Identity/Account/Manage/ConfirmSubscription?userId={userId}&token={Uri.EscapeDataString(token)}&subscriptionTypeId={subscriptionTypeId}";
            

            // Email content
            var subject = "Confirm Your Subscription";
            var htmlMessage = $"<p>Thank you for subscribing! Please confirm your subscription by clicking <a href='{confirmationLink}'>here</a>.</p>";

            // Send email
            await _emailSender.SendEmailAsync(user.Email, subject, htmlMessage);
        }

        // Method to cancel the subscription
        public async Task<bool> CancelSubscriptionAsync(string userId)
        {
            var subscription = await _db.Subscriptions.FirstOrDefaultAsync(s => s.UserId == userId && s.Expires > DateTime.UtcNow);

            if (subscription == null)
            {
                return false;
            }

            subscription.Expires = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && await _userManager.IsInRoleAsync(user, "Subscriber"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Subscriber");
            }

            return true;
        }

        
    }
}
