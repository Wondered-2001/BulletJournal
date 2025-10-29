using System.Collections.Generic;

namespace BJ.Core.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation property
        public List<JournalItem> Items { get; set; } = new();
    }
}
