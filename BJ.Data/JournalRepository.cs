using BJ.Core.Interfaces;
using BJ.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BJ.Data
{
    public class JournalRepository : IJournalRepository
    {
        private readonly BulletJournalDbContext _context;

        public JournalRepository(BulletJournalDbContext context)
        {
            _context = context;
        }

        public async Task<List<BJ.Core.Models.JournalItem>> GetAllAsync()
        {
            return await _context.JournalItems
                                 .Include(j => j.Tags)
                                 .ToListAsync();
        }

        public async Task AddAsync(BJ.Core.Models.JournalItem item)
        {
            _context.JournalItems.Add(item);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(JournalItem item)
        {
            _context.JournalItems.Update(item);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(JournalItem item)
        {
            _context.JournalItems.Remove(item);
            await _context.SaveChangesAsync();
        }

    }
}
