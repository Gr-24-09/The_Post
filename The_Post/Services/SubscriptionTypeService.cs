﻿using Microsoft.EntityFrameworkCore;
using The_Post.Data; 
using The_Post.Models;

namespace The_Post.Services
{
    public class SubscriptionTypeService : ISubscriptionTypeService
    {
        private readonly ApplicationDbContext _db;

        public SubscriptionTypeService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<SubscriptionType>> GetAllSubTypes()
        {
            return await _db.SubscriptionTypes.ToListAsync();
        }

        public async Task<SubscriptionType?> GetByIdAsync(int id)
        {
            return await _db.SubscriptionTypes.FindAsync(id);
        }

        public async Task CreateSubType(SubscriptionType subType)
        {
            _db.SubscriptionTypes.Add(subType);
            await _db.SaveChangesAsync();
        }

        public async Task EditSubType(SubscriptionType subType)
        {
            _db.SubscriptionTypes.Update(subType);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteSubType(int id)
        {
            var subType = await GetByIdAsync(id);
            if (subType == null)
            {
                throw new ArgumentException("SubscriptionType not found", nameof(id));
            }
            _db.SubscriptionTypes.Remove(subType);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await _db.SubscriptionTypes.AnyAsync(st => st.Id == id);
        }
    }
}
