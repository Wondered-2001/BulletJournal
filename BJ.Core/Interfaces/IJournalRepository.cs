using System.Collections.Generic;
using System.Threading.Tasks;
using BJ.Core.Models;

namespace BJ.Core.Interfaces
{
    public interface IJournalRepository
    {
        Task<List<BJ.Core.Models.JournalItem>> GetAllAsync();
        Task AddAsync(BJ.Core.Models.JournalItem item);
        Task UpdateAsync(JournalItem item);
        Task DeleteAsync(JournalItem item);

    }
}
