using BJ.Core.Interfaces;
using BJ.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Threading.Tasks;
//using System.Windows.Input;

namespace BJ.Core.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IJournalRepository _repo;

        public ObservableCollection<JournalItem> Items { get; } = new();
        public ObservableCollection<Tag> EditingTags { get; } = new();
        public ObservableCollection<TagItem> AllTagItems { get; } = new();
        public ObservableCollection<Tag> SelectedFilterTags { get; } = new();


        // Async commands
        public IAsyncRelayCommand LoadItemsCommand { get; }
        public IAsyncRelayCommand AddItemCommand { get; }
        public IAsyncRelayCommand SaveItemCommand { get; }
        public IAsyncRelayCommand DeleteItemCommand { get; }

        public IAsyncRelayCommand AddTagCommand { get; }
        public IAsyncRelayCommand RemoveTagCommand { get; }

        private JournalItem? _selectedItem;
        public JournalItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    // Copy to editing item
                    EditingItem = value != null
                        ? new JournalItem
                        {
                            Id = value.Id,
                            Title = value.Title,
                            Content = value.Content,
                            Tags = new ObservableCollection<Tag>(value.Tags)
                        }
                        : null;

                    // Sync tags collection
                    SyncEditingTags();

                    SaveItemCommand.NotifyCanExecuteChanged();
                    DeleteItemCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private JournalItem? _editingItem;
        public JournalItem? EditingItem
        {
            get => _editingItem;
            set
            {
                if (SetProperty(ref _editingItem, value))
                {
                    SyncEditingTags();
                }
            }
        }

        private Tag? _selectedTag;
        public Tag? SelectedTag
        {
            get => _selectedTag;
            set
            {
                if (SetProperty(ref _selectedTag, value))
                {
                    RemoveTagCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public MainViewModel(IJournalRepository repo)
        {
            _repo = repo;

            LoadItemsCommand = new AsyncRelayCommand(LoadItemsAsync);
            AddItemCommand = new AsyncRelayCommand(AddItemAsync);
            SaveItemCommand = new AsyncRelayCommand(SaveItemAsync, () => SelectedItem != null && EditingItem != null);
            DeleteItemCommand = new AsyncRelayCommand(DeleteItemAsync, () => SelectedItem != null);

            AddTagCommand = new AsyncRelayCommand(AddTagAsync, () => EditingItem != null);
            RemoveTagCommand = new AsyncRelayCommand(RemoveTagAsync, () => SelectedTag != null);

            _ = LoadItemsAsync();
        }

        private void SyncEditingTags()
        {
            EditingTags.Clear();
            if (EditingItem != null)
            {
                foreach (var t in EditingItem.Tags)
                    EditingTags.Add(t);
            }
        }

        private async Task LoadItemsAsync()
        {
            var all = await _repo.GetAllAsync();
            Items.Clear();
            foreach (var item in all)
                Items.Add(item);

            if (Items.Count > 0)
                SelectedItem = Items[0];

            SyncAllTags();
        }

        private async Task AddItemAsync()
        {
            var newItem = new JournalItem { Title = "New Item", Content = "" };
            await _repo.AddAsync(newItem);
            Items.Add(newItem);
            SelectedItem = newItem;
        }

        private async Task SaveItemAsync()
        {
            if (SelectedItem == null || EditingItem == null) return;
            SyncAllTags();

            // Copy edited fields back to SelectedItem
            SelectedItem.Title = EditingItem.Title;
            SelectedItem.Content = EditingItem.Content;

            // Replace tags with a fresh copy from EditingItem
            SelectedItem.Tags.Clear();
            foreach (var t in EditingItem.Tags)
                SelectedItem.Tags.Add(t);

            // Persist to repository
            await _repo.UpdateAsync(SelectedItem);

            // Refresh EditingItem to reflect saved state
            EditingItem = new JournalItem
            {
                Id = SelectedItem.Id,
                Title = SelectedItem.Title,
                Content = SelectedItem.Content,
                Tags = new ObservableCollection<Tag>(SelectedItem.Tags)
            };

            // Refresh Items collection to notify UI
            var index = Items.IndexOf(SelectedItem);
            if (index >= 0)
            {
                // Replace item in collection to trigger UI update
                Items[index] = SelectedItem;
            }

            // Re-sync tags for the editor
            SyncEditingTags();
        }


        private async Task DeleteItemAsync()
        {
            if (SelectedItem == null) return;

            await _repo.DeleteAsync(SelectedItem);
            Items.Remove(SelectedItem);
            SelectedItem = null;
            EditingItem = null;
            EditingTags.Clear();
        }

        private Task AddTagAsync()
        {
            if (EditingItem == null) return Task.CompletedTask;

            var newTag = new Tag { Name = "New Tag" };
            EditingItem.Tags.Add(newTag);
            EditingTags.Add(newTag);
            SelectedTag = newTag;

            // Ensure the new tag appears in the global tag list for filtering
            if (!AllTagItems.Any(ti => ti.Tag.Name == newTag.Name))
            {
                var tagItem = new TagItem(newTag);
                tagItem.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(TagItem.IsSelected))
                    {
                        if (tagItem.IsSelected && !SelectedFilterTags.Contains(newTag))
                            SelectedFilterTags.Add(newTag);
                        else if (!tagItem.IsSelected && SelectedFilterTags.Contains(newTag))
                            SelectedFilterTags.Remove(newTag);

                        ApplyTagFilter();
                    }
                };
                AllTagItems.Add(tagItem);
            }

            return Task.CompletedTask;
        }

        private Task RemoveTagAsync()
        {
            if (EditingItem == null || SelectedTag == null) return Task.CompletedTask;

            EditingItem.Tags.Remove(SelectedTag);
            EditingTags.Remove(SelectedTag);
            SelectedTag = null;

            return Task.CompletedTask;
        }
   
        private void SyncAllTags()
        {
            AllTagItems.Clear();

            // Collect all unique tags from journal items
            var tagSet = Items
                .SelectMany(i => i.Tags)
                .GroupBy(t => t.Name)
                .Select(g => g.First())
                .ToList();

            foreach (var tag in tagSet)
            {
                var tagItem = new TagItem(tag);
                tagItem.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(TagItem.IsSelected))
                    {
                        if (tagItem.IsSelected && !SelectedFilterTags.Contains(tag))
                            SelectedFilterTags.Add(tag);
                        else if (!tagItem.IsSelected && SelectedFilterTags.Contains(tag))
                            SelectedFilterTags.Remove(tag);

                        ApplyTagFilter();
                    }
                };
                AllTagItems.Add(tagItem);
            }
        }

        private void ApplyTagFilter()
        {
            if (SelectedFilterTags.Count == 0)
            {
                // No filter -> show all items
                _ = LoadItemsAsync();
                return;
            }

            var filtered = Items
                .Where(i => i.Tags.Any(t => SelectedFilterTags.Any(f => f.Name == t.Name)))
                .ToList();

            Items.Clear();
            foreach (var item in filtered)
                Items.Add(item);
        }
    }
}
