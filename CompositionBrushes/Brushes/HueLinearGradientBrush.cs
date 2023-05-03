using System.Numerics;

using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace CompositionBrushes.Brushes
{
  public class HueLinearGradientBrush : XamlCompositionBrushBase
  {
    private Compositor _compositor;

    protected override void OnConnected()
    {
      if (CompositionBrush == null)
      {
        _compositor = Window.Current.Compositor;

        var hueLinearGradientBrush = _compositor.CreateLinearGradientBrush();
        hueLinearGradientBrush.ColorStops.Add(_compositor.CreateColorGradientStop(0, Colors.Red));
        hueLinearGradientBrush.ColorStops.Add(_compositor.CreateColorGradientStop(0.17f, Colors.Yellow));
        hueLinearGradientBrush.ColorStops.Add(_compositor.CreateColorGradientStop(0.33f, Colors.Lime));
        hueLinearGradientBrush.ColorStops.Add(_compositor.CreateColorGradientStop(0.5f, Colors.Aqua));
        hueLinearGradientBrush.ColorStops.Add(_compositor.CreateColorGradientStop(0.67f, Colors.Blue));
        hueLinearGradientBrush.ColorStops.Add(_compositor.CreateColorGradientStop(0.83f, Colors.Fuchsia));
        hueLinearGradientBrush.ColorStops.Add(_compositor.CreateColorGradientStop(1, Colors.Red));
        hueLinearGradientBrush.StartPoint = new Vector2(0, 0);
        hueLinearGradientBrush.EndPoint = new Vector2(1, 0);

        CompositionBrush = hueLinearGradientBrush;
      }
    }

    protected override void OnDisconnected()
    {
      if (CompositionBrush != null)
      {
        CompositionBrush.Dispose();
        CompositionBrush = null;
      }
    }
  }
}