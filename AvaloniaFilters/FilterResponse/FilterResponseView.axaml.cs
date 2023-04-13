using Avalonia.Controls;
using Avalonia.Input;
using DynamicData.Binding;
using Filters;
using System;
using System.Collections.Generic;
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
            filterTypeCombo.Items = Enum.GetValues(typeof(FilterType)).Cast<FilterType>().ToList();
            filterTypeCombo.SelectedIndex = 0;

            filterPassTypeCombo.SelectionChanged += FilterPassTypeCombo_SelectionChanged;

            orderCombo.SelectionChanged += OrderCombo_SelectionChanged;
            cutoffFreqSlider.PropertyChanged += Slider_PropertyChanged;
            bandwidthSlider.PropertyChanged += Slider_PropertyChanged;
            gainSlider.PropertyChanged += Slider_PropertyChanged;
            qSlider.PropertyChanged += Slider_PropertyChanged;
            rippleFactorSlider.PropertyChanged += Slider_PropertyChanged;
            samplingFreqSlider.PropertyChanged += Slider_PropertyChanged;

            samplingFreqTextBox.PointerWheelChanged += TextBox_PointerWheelChanged;
            cutoffFreqTextBox.PointerWheelChanged += TextBox_PointerWheelChanged;
            bwTextBox.PointerWheelChanged += TextBox_PointerWheelChanged;

            cutoffFreqSlider.PointerWheelChanged += Slider_PointerWheelChanged;
            bandwidthSlider.PointerWheelChanged += Slider_PointerWheelChanged;
            gainSlider.PointerWheelChanged += Slider_PointerWheelChanged;
            qPanel.PointerWheelChanged += Slider_PointerWheelChanged;
            rippleFactorSlider.PointerWheelChanged += Slider_PointerWheelChanged;
            samplingFreqSlider.PointerWheelChanged += Slider_PointerWheelChanged; 

            magnitudePlot.HorizontalLines = new LinesDefinition[]
            {
                new LinesDefinition(0, 1, true, Plot.Beige),
                new LinesDefinition(0, 10, true, Plot.Beige),
                new LinesDefinition(0, 20, true, Plot.Beige),
                new LinesDefinition(0, 50, true, Plot.Beige),
            };
            phasePlot.HorizontalLines = new LinesDefinition[]
            {
                new LinesDefinition(0, (float)Math.PI/4, true, Plot.Beige),
            };
            phasePlot.MinYDisplayRangeEnd = Math.PI/2+0.1;
            phasePlot.MaxYDisplayRangeStart = -Math.PI/2-0.1;

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
                new LinesDefinition(4000, 0, true, Plot.Beige),
                new LinesDefinition(8000, 0, true, Plot.Beige),
                new LinesDefinition(16000, 0, true, Plot.Beige),
                new LinesDefinition(32000, 0, true, Plot.Beige)
            };

            SetMaximumSliderValues();
            UpdatePanelVisibility();
        }

        private void TextBox_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            if (sender == samplingFreqTextBox)
                Slider_PointerWheelChanged(samplingFreqSlider, e);
            else if (sender == cutoffFreqTextBox)
                Slider_PointerWheelChanged(cutoffFreqSlider, e);
            else if (sender == bwTextBox)
                Slider_PointerWheelChanged(bandwidthSlider, e);
        }

        private void Slider_PointerWheelChanged(object? sender, Avalonia.Input.PointerWheelEventArgs e)
        {
            if(sender is Slider slider)
            {
                slider.Value += e.Delta.Y * slider.SmallChange;
                slider.Focus();
            }
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
                if(sender == samplingFreqSlider)
                {
                    SetMaximumSliderValues();
                }

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

        void SetMaximumSliderValues()
        {
            cutoffFreqSlider.Maximum = samplingFreqSlider.Value / 2 - 1;
            bandwidthSlider.Maximum = samplingFreqSlider.Value / 4;
        }

        void UpdatePanelVisibility()
        {
            bwPanel.IsVisible = filterTypeCombo.SelectedItem is FilterType ft6 &&
                filterPassTypeCombo.SelectedItem is FilterPassType pt &&
                (pt == FilterPassType.BandPass || pt == FilterPassType.BandStop || 
                ft6 == FilterType.Equalization || ft6 == FilterType.Notch);
            linearGainPanel.IsVisible = filterTypeCombo.SelectedItem is FilterType ft &&
                (ft == FilterType.Equalization || ft == FilterType.Shelf);
            rippleFactorPanel.IsVisible = filterTypeCombo.SelectedItem is FilterType ft2 &&
                (ft2 == FilterType.ChebyshevTypeI | ft2 == FilterType.ChebyshevTypeII);
            qPanel.IsVisible = filterTypeCombo.SelectedItem is FilterType ft3 &&
                ft3 == FilterType.VariableQ;
        }

        void UpdatePassTypeCombo()
        {
            if (filterTypeCombo.SelectedItem is FilterType ft)
            {
                var selectedPastType = filterPassTypeCombo.SelectedItem;
                List<FilterPassType> list = IIRFilter.GetFilterPassTypes(ft).ToList();
                filterPassTypeCombo.Items = list;
                filterPassTypeCombo.SelectedItem = selectedPastType;

                if (filterPassTypeCombo.SelectedIndex < 0)
                {
                    filterPassTypeCombo.SelectedIndex = 0;
                }

                UpdateOrderCombo();

                CreateFilter();
            }
        }

        void UpdateOrderCombo()
        {
            if (filterTypeCombo.SelectedItem is FilterType ft &&
                filterPassTypeCombo.SelectedItem is FilterPassType pt)
            {
                int? order = orderCombo.SelectedItem as int?;
                orderCombo.Items = IIRFilter.GetFilterOrders(ft, pt);
                orderCombo.SelectedItem = order;
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
