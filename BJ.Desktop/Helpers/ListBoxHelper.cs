using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace BJ.Desktop.Helpers
{
    public static class ListBoxHelper
    {
        public static readonly DependencyProperty BindableSelectedItemsProperty =
            DependencyProperty.RegisterAttached(
                "BindableSelectedItems",
                typeof(IList),
                typeof(ListBoxHelper),
                new PropertyMetadata(null, OnBindableSelectedItemsChanged));

        public static void SetBindableSelectedItems(DependencyObject element, IList value)
        {
            element.SetValue(BindableSelectedItemsProperty, value);
        }

        public static IList GetBindableSelectedItems(DependencyObject element)
        {
            return (IList)element.GetValue(BindableSelectedItemsProperty);
        }

        private static void OnBindableSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListBox listBox)
            {
                listBox.SelectionChanged -= ListBox_SelectionChanged;

                if (e.NewValue is IList newList)
                {
                    listBox.SelectionChanged += ListBox_SelectionChanged;
                }
            }
        }

        private static void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox)
            {
                var boundList = GetBindableSelectedItems(listBox);
                if (boundList == null) return;

                boundList.Clear();
                foreach (var item in listBox.SelectedItems)
                {
                    boundList.Add(item);
                }
            }
        }
    }
}
