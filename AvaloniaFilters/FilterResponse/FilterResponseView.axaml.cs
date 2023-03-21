using Avalonia.Controls;
using DynamicData.Binding;
using Filters;
using System;
using System.Linq;

namespace AvaloniaFilters
{
    public partial class FilterResponseView : UserControl
    {
        public FilterResponseView()
        {
            InitializeComponent();

            DataContext = new FilterResponseViewModel();

            filterTypeCombo.Items = Enum.GetValues(typeof(FilterType)).Cast<FilterType>();
            filterTypeCombo.SelectedIndex = 0;
            orderCombo.Items = Enumerable.Range(1, 4);
            orderCombo.SelectedIndex = 0;
            CreateFilter();

            filterTypeCombo.SelectionChanged += FilterTypeCombo_SelectionChanged;
            orderCombo.SelectionChanged += OrderCombo_SelectionChanged;
            cutoffFreqSlider.PropertyChanged += CutoffFreqSlider_PropertyChanged;
            cutoffFreqTextBlock.Text = ((int)cutoffFreqSlider.Value).ToString();
            CreateFilter();
        }

        private void CutoffFreqSlider_PropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
        {
            if(e.Property == Slider.ValueProperty)
            {
                cutoffFreqTextBlock.Text = ((int)cutoffFreqSlider.Value).ToString();
                CreateFilter();
            }
        }

        private void OrderCombo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            CreateFilter();
        }

        private void FilterTypeCombo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            CreateFilter();
        }

        void CreateFilter()
        {
            if (DataContext is FilterResponseViewModel vm &&
                filterTypeCombo.SelectedItem is FilterType)
            {
                IIRFilter filter = IIRFilter.CreateLowPass(
                    (FilterType)filterTypeCombo.SelectedItem,
                    orderCombo.SelectedIndex+1, 
                    (int)cutoffFreqSlider.Value, 
                    10000);
                vm.SetFilter(filter);
            }
        }
    }
}
