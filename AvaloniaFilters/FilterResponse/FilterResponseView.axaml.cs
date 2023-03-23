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
            filterPassTypeCombo.Items = Enum.GetValues(typeof(FilterPassType)).Cast<FilterPassType>();
            filterTypeCombo.SelectedIndex = 0;
            filterPassTypeCombo.SelectedIndex = 1;
            orderCombo.Items = Enumerable.Range(1, 4);
            orderCombo.SelectedIndex = 0;
            CreateFilter();

            filterTypeCombo.SelectionChanged += FilterTypeCombo_SelectionChanged;
            orderCombo.SelectionChanged += OrderCombo_SelectionChanged;
            cutoffFreqSlider.PropertyChanged += CutoffFreqSlider_PropertyChanged;
            cutoffFreqTextBlock.Text = ((int)cutoffFreqSlider.Value).ToString();
            CreateFilter();

            magnitudePlot.HorizontalLines = new LinesDefinition[]
            {
                new LinesDefinition(0, 1, true, Plot.Beige)
            };

            phasePlot.VerticalLines = magnitudePlot.VerticalLines = new LinesDefinition[]
            {
                new LinesDefinition(50, 0, true, Plot.Beige),
                new LinesDefinition(100, 0, true, Plot.Beige),
                new LinesDefinition(250, 0, true, Plot.Beige),
                new LinesDefinition(500, 0, true, Plot.Beige),
                new LinesDefinition(1000, 0, true, Plot.Beige),
                new LinesDefinition(2000, 0, true, Plot.Beige),
                new LinesDefinition(4000, 0, true, Plot.Beige)
            };
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
                IIRFilter? filter = IIRFilter.CreateFilter(
                    (FilterType)filterTypeCombo.SelectedItem,
                    new FilterParameters(order: orderCombo.SelectedIndex + 1,
                        fs: vm.Fs,
                        fc: (int)cutoffFreqSlider.Value),
                    filterPassTypeCombo.SelectedItem is null 
                        ? FilterPassType.None 
                        : (FilterPassType)filterPassTypeCombo.SelectedItem
                );
                    
                vm.SetFilter(filter);
            }
        }
    }
}
