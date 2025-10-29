using CommunityToolkit.Mvvm.ComponentModel;

namespace BJ.Core.Models
{
    public partial class TagItem : ObservableObject
    {
        public Tag Tag { get; }

        [ObservableProperty]
        private bool _isSelected;

        public TagItem(Tag tag)
        {
            Tag = tag;
        }
    }
}
