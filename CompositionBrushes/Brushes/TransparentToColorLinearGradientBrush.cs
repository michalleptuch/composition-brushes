using System;
using System.Numerics;

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Toolkit.Uwp.UI.Media;
using Microsoft.Toolkit.Uwp.UI.Media.Helpers;

using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace CompositionBrushes.Brushes
{
  public class TransparentToColorLinearGradientBrush : XamlCompositionBrushBase
  {
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
      "Color", typeof(Color), typeof(TransparentToColorLinearGradientBrush), new PropertyMetadata(Colors.Transparent));

    public static readonly DependencyProperty StartPointProperty = DependencyProperty.Register(
      "StartPoint", typeof(Vector2), typeof(TransparentToColorLinearGradientBrush), new PropertyMetadata(new Vector2(0, 0)));

    public static readonly DependencyProperty EndPointProperty = DependencyProperty.Register(
      "EndPoint", typeof(Vector2), typeof(TransparentToColorLinearGradientBrush), new PropertyMetadata(new Vector2(1, 1)));

    public Color Color
    {
      get => (Color)GetValue(ColorProperty);
      set
      {
        SetValue(ColorProperty, value);
        BuildBrush();
      }
    }

    public Vector2 StartPoint
    {
      get => (Vector2)GetValue(StartPointProperty);
      set
      {
        SetValue(StartPointProperty, value);
        BuildBrush();
      }
    }

    public Vector2 EndPoint
    {
      get => (Vector2)GetValue(EndPointProperty);
      set
      {
        SetValue(EndPointProperty, value);
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

    private async void BuildBrush()
    {
      if (_compositor == null)
      {
        _compositor = Window.Current.Compositor;
      }

      var imageFilePath = "ms-appx:///CompositionBrushes/Assets/TransparencyBackground.png";
      var imageCompositionBrush = await SurfaceLoader.LoadImageAsync(new Uri(imageFilePath), DpiMode.DisplayDpi);

      var borderEffect = new BorderEffect
      {
        ExtendX = CanvasEdgeBehavior.Wrap,
        ExtendY = CanvasEdgeBehavior.Wrap,
        Source = new CompositionEffectSourceParameter("ImageCompositionBrush"),
      };

      var transparentColorBrush = _compositor.CreateLinearGradientBrush();
      transparentColorBrush.ColorStops.Add(_compositor.CreateColorGradientStop(0, Color.FromArgb(0, Color.R, Color.G, Color.B)));
      transparentColorBrush.ColorStops.Add(_compositor.CreateColorGradientStop(1, Color));
      transparentColorBrush.StartPoint = StartPoint;
      transparentColorBrush.EndPoint = EndPoint;

      var compositeEffect = new CompositeEffect
      {
        Sources =
        {
          borderEffect,
          new CompositionEffectSourceParameter("TransparentColorBrush"),
        }
      };

      var effectFactory = _compositor.CreateEffectFactory(compositeEffect);

      var effectBrush = effectFactory.CreateBrush();
      effectBrush.SetSourceParameter("ImageCompositionBrush", imageCompositionBrush);
      effectBrush.SetSourceParameter("TransparentColorBrush", transparentColorBrush);

      CompositionBrush = effectBrush;
    }
  }
}