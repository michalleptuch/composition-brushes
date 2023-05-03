using System.Numerics;

using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Toolkit.Uwp.Helpers;

using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace CompositionBrushes.Brushes
{
  public class SaturationValueDoubleLinearGradientBrush : XamlCompositionBrushBase
  {
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
      "Color", typeof(Color), typeof(SaturationValueDoubleLinearGradientBrush), new PropertyMetadata(Colors.White));

    public Color Color
    {
      get => (Color)GetValue(ColorProperty);
      set
      {
        SetValue(ColorProperty, value);
        BuildBrush();
      }
    }

    private Compositor _compositor;

    protected override void OnConnected()
    {
      BuildBrush();
    }

    protected override void OnDisconnected()
    {
      if (CompositionBrush != null)
      {
        CompositionBrush.Dispose();
        CompositionBrush = null;
      }
    }

    private void BuildBrush()
    {
      if (_compositor == null)
      {
        _compositor = Window.Current.Compositor;
      }

      var hsvColor = Color.ToHsv();

      var colorEffect = new ColorSourceEffect
      {
        Color = Microsoft.Toolkit.Uwp.Helpers.ColorHelper.FromHsv(hsvColor.H, 1, 1),
      };

      var whiteTransparentBrush = _compositor.CreateLinearGradientBrush();
      whiteTransparentBrush.ColorStops.Add(_compositor.CreateColorGradientStop(0, Color.FromArgb(255, 255, 255, 255)));
      whiteTransparentBrush.ColorStops.Add(_compositor.CreateColorGradientStop(1, Color.FromArgb(0, 255, 255, 255)));
      whiteTransparentBrush.StartPoint = new Vector2(0, 0);
      whiteTransparentBrush.EndPoint = new Vector2(1, 0);

      var blackTransparentBrush = _compositor.CreateLinearGradientBrush();
      blackTransparentBrush.ColorStops.Add(_compositor.CreateColorGradientStop(0, Color.FromArgb(0, 0, 0, 0)));
      blackTransparentBrush.ColorStops.Add(_compositor.CreateColorGradientStop(1, Color.FromArgb(255, 0, 0, 0)));
      blackTransparentBrush.StartPoint = new Vector2(0, 0);
      blackTransparentBrush.EndPoint = new Vector2(0, 1);

      var compositeEffect = new CompositeEffect
      {
        Sources =
        {
          colorEffect,
          new CompositionEffectSourceParameter("WhiteTransparentBrush"),
          new CompositionEffectSourceParameter("BlackTransparentBrush"),
        }
      };

      var effectFactory = _compositor.CreateEffectFactory(compositeEffect);

      var effectBrush = effectFactory.CreateBrush();
      effectBrush.SetSourceParameter("WhiteTransparentBrush", whiteTransparentBrush);
      effectBrush.SetSourceParameter("BlackTransparentBrush", blackTransparentBrush);

      CompositionBrush = effectBrush;
    }
  }
}