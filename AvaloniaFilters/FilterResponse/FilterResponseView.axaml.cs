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

            filterTypeCombo.SelectionChanged += FilterTypeCombo_SelectionChanged;
            filterTypeCombo.Items = Enum.GetValues(typeof(FilterType)).Cast<FilterType>();
            filterTypeCombo.SelectedIndex = 0;

            filterPassTypeCombo.SelectionChanged += FilterPassTypeCombo_SelectionChanged;

            orderCombo.SelectionChanged += OrderCombo_SelectionChanged;
            cutoffFreqSlider.PropertyChanged += CutoffFreqSlider_PropertyChanged;
            cutoffFreqTextBlock.Text = ((int)cutoffFreqSlider.Value).ToString();

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

        private void FilterPassTypeCombo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            UpdateCombos();
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
            UpdateCombos();
        }

        void UpdateCombos()
        {
            if (filterTypeCombo.SelectedItem is FilterType ft)
            {
                filterPassTypeCombo.Items = IIRFilter.GetFilterPassTypes(ft);
                if (filterPassTypeCombo.SelectedIndex < 0)
                    filterPassTypeCombo.SelectedIndex = 0;

                if (filterPassTypeCombo.SelectedItem is FilterPassType pt)
                {
                    orderCombo.Items = IIRFilter.GetFilterOrders(ft, pt);
                    if (orderCombo.SelectedIndex < 0)
                        orderCombo.SelectedIndex = 0;
                }
            }
            CreateFilter();
        }

        void CreateFilter()
        {
            if (DataContext is FilterResponseViewModel vm &&
                filterTypeCombo.SelectedItem is FilterType ft &&
                filterPassTypeCombo.SelectedItem is FilterPassType pt &&
                orderCombo.SelectedItem is int order)
            {
                IIRFilter? filter = IIRFilter.CreateFilter(
                    ft,
                    new FilterParameters(order: orderCombo.SelectedIndex + 1)
                    {
                        Fs = vm.Fs,                        
                        Fc = (int)cutoffFreqSlider.Value,
                        BW = bandwidthSlider.Value,
                        Order = order
                    },
                    pt
                );
                    
                vm.SetFilter(filter);
            }
        }
    }
}