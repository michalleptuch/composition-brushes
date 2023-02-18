using System;

using Windows.System.Power;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace AlwaysOnAcrylicBrushSample
{
  public class AlwaysOnAcrylicBrush : XamlCompositionBrushBase
  {
    public static readonly DependencyProperty TintColorProperty =
      DependencyProperty.Register("TintColor", typeof(Color), typeof(AlwaysOnAcrylicBrush), new PropertyMetadata(Color.FromArgb(204, 255, 255, 255)));

    public static readonly DependencyProperty TintOpacityProperty =
      DependencyProperty.Register("TintOpacity", typeof(double), typeof(AlwaysOnAcrylicBrush), new PropertyMetadata(1.0));

    public static readonly DependencyProperty TintLuminosityOpacityProperty =
      DependencyProperty.Register("TintLuminosityOpacity", typeof(double?), typeof(AlwaysOnAcrylicBrush), new PropertyMetadata(null));

    public static new readonly DependencyProperty FallbackColorProperty =
      DependencyProperty.Register("FallbackColor", typeof(Color), typeof(AlwaysOnAcrylicBrush), new PropertyMetadata(Color.FromArgb(204, 255, 255, 255)));

    private readonly AlwaysOnAcrylicBrushFactory _factory;

    private AccessibilitySettings _accessibilitySettings;
    private UISettings _uiSettings;
    private bool _fastEffects;
    private bool _energySaver;

    private bool _isConnected;

    public AlwaysOnAcrylicBrush()
    {
      _factory = new AlwaysOnAcrylicBrushFactory();

      _accessibilitySettings = new AccessibilitySettings();
      _uiSettings = new UISettings();
      _fastEffects = CompositionCapabilities.GetForCurrentView().AreEffectsFast();
      _energySaver = PowerManager.EnergySaverStatus == EnergySaverStatus.On;

      _isConnected = false;
    }

    public Color TintColor
    {
      get
      {
        return (Color)GetValue(TintColorProperty);
      }
      set
      {
        SetValue(TintColorProperty, value);
        UpdateBrush();
      }
    }

    public double TintOpacity
    {
      get
      {
        return (double)GetValue(TintOpacityProperty);
      }
      set
      {
        SetValue(TintOpacityProperty, value);
        UpdateBrush();
      }
    }

    public double? TintLuminosityOpacity
    {
      get
      {
        return (double?)GetValue(TintLuminosityOpacityProperty);
      }
      set
      {
        SetValue(TintLuminosityOpacityProperty, value);
        UpdateBrush();
      }
    }

    public new Color FallbackColor
    {
      get
      {
        return (Color)GetValue(FallbackColorProperty);
      }
      set
      {
        SetValue(FallbackColorProperty, value);
        UpdateBrush();
      }
    }

    protected override void OnConnected()
    {
      base.OnConnected();

      _isConnected = true;

      UpdateBrush();

      _accessibilitySettings.HighContrastChanged += OnHighContrastChanged;
      _uiSettings.ColorValuesChanged += OnColorValuesChanged;
      PowerManager.EnergySaverStatusChanged += OnEnergySaverStatusChanged;
      CompositionCapabilities.GetForCurrentView().Changed += OnChanged;
    }

    protected override void OnDisconnected()
    {
      base.OnDisconnected();

      _isConnected = false;

      _accessibilitySettings.HighContrastChanged -= OnHighContrastChanged;
      _uiSettings.ColorValuesChanged -= OnColorValuesChanged;
      PowerManager.EnergySaverStatusChanged -= OnEnergySaverStatusChanged;
      CompositionCapabilities.GetForCurrentView().Changed -= OnChanged;

      if (CompositionBrush != null)
      {
        CompositionBrush.Dispose();
        CompositionBrush = null;
      }
    }

    private void UpdateBrush()
    {
      if (!_isConnected)
      {
        return;
      }

      _energySaver = PowerManager.EnergySaverStatus == EnergySaverStatus.On;

      var useSolidColorFallback =
        _accessibilitySettings.HighContrast ||
        !_uiSettings.AdvancedEffectsEnabled ||
        !_fastEffects ||
        _energySaver;

      CompositionBrush = _factory.CreateActiveAcrylicBrush(TintColor, (float)TintOpacity, (float?)TintLuminosityOpacity, FallbackColor, useSolidColorFallback);
    }

    private async void OnHighContrastChanged(AccessibilitySettings sender, object args)
    {
      await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        UpdateBrush();
      });
    }

    private async void OnColorValuesChanged(UISettings sender, object args)
    {
      await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        UpdateBrush();
      });
    }

    private async void OnChanged(CompositionCapabilities sender, object args)
    {
      await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        _fastEffects = sender.AreEffectsFast();
        UpdateBrush();
      });
    }

    private async void OnEnergySaverStatusChanged(object sender, object e)
    {
      await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        _energySaver = PowerManager.EnergySaverStatus == EnergySaverStatus.On;
        UpdateBrush();
      });
    }
  }
}