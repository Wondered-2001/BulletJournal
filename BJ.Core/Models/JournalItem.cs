using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BJ.Core.Models
{
    public partial class JournalItem : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _content = string.Empty;

        // Navigation properties
        public ObservableCollection<Tag> Tags { get; set; } = new();
        public int? NotebookId { get; set; } // optional
        public Notebook? Notebook { get; set; }
    }
}
