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
            cutoffFreqSlider.PropertyChanged += Slider_PropertyChanged;
            bandwidthSlider.PropertyChanged += Slider_PropertyChanged;
            gainSlider.PropertyChanged += Slider_PropertyChanged;
            qSlider.PropertyChanged += Slider_PropertyChanged;

            magnitudePlot.HorizontalLines = new LinesDefinition[]
            {
                new LinesDefinition(0, 1, true, Plot.Beige),
                new LinesDefinition(0, 10, true, Plot.Beige),
                new LinesDefinition(0, 20, true, Plot.Beige),
                new LinesDefinition(0, 50, true, Plot.Beige),
            };
            phasePlot.HorizontalLines = new LinesDefinition[]
            {
                new LinesDefinition(0, 10, true, Plot.Beige),
                new LinesDefinition(0, 25, true, Plot.Beige),
                new LinesDefinition(0, 50, true, Plot.Beige),
            };

            magnitudePlot.MinYDisplayRangeStart = -100;
            magnitudePlot.MinYDisplayRangeEnd = 1;
            phasePlot.XUnit = magnitudePlot.XUnit = "Hz";
            magnitudePlot.YUnit = "dB";

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

            UpdatePanelVisibility();
        }

        private void FilterPassTypeCombo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            UpdatePanelVisibility();
            UpdateOrderCombo();
            CreateFilter();
        }

        private void Slider_PropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
        {
            if(e.Property == Slider.ValueProperty)
            {
                CreateFilter();
            }
        }

        private void OrderCombo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            CreateFilter();
        }

        private void FilterTypeCombo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            UpdatePassTypeCombo();
        }

        void UpdatePanelVisibility()
        {
            bwPanel.IsVisible = filterTypeCombo.SelectedItem is FilterType ft6 &&
                filterPassTypeCombo.SelectedItem is FilterPassType pt &&
                (pt == FilterPassType.BandPass || pt == FilterPassType.BandStop || ft6 == FilterType.Equalization);
            linearGainPanel.IsVisible = filterTypeCombo.SelectedItem is FilterType ft &&
                ft == FilterType.Equalization;
            rippleFactorPanel.IsVisible = filterTypeCombo.SelectedItem is FilterType ft2 &&
                (ft2 == FilterType.ChebychevTypeI | ft2 == FilterType.ChebychevTypeII);
            qPanel.IsVisible = filterTypeCombo.SelectedItem is FilterType ft3 &&
                ft3 == FilterType.VariableQ;
        }

        void UpdatePassTypeCombo()
        {
            if (filterTypeCombo.SelectedItem is FilterType ft)
            {
                filterPassTypeCombo.Items = IIRFilter.GetFilterPassTypes(ft);

                if (filterPassTypeCombo.SelectedIndex < 0)
                    filterPassTypeCombo.SelectedIndex = 0;

                UpdateOrderCombo();

                CreateFilter();
            }
        }

        void UpdateOrderCombo()
        {
            if (filterTypeCombo.SelectedItem is FilterType ft &&
                filterPassTypeCombo.SelectedItem is FilterPassType pt)
            {
                orderCombo.Items = IIRFilter.GetFilterOrders(ft, pt);
                if (orderCombo.SelectedIndex < 0)
                    orderCombo.SelectedIndex = 0;
            }
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
                        Order = order,
                        LinearGain = Math.Pow(10, gainSlider.Value/20),
                        RippleFactor = rippleFactorSlider.Value,
                        Q = qSlider.Value
                    },
                    pt
                );
                    
                vm.SetFilter(filter);
            }
        }
    }
}
