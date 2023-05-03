using System;

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Toolkit.Uwp.UI.Media;
using Microsoft.Toolkit.Uwp.UI.Media.Helpers;

using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace CompositionBrushes.Brushes
{
  public class TransparencyPlaceholderBrush : XamlCompositionBrushBase
  {
    private Compositor _compositor;

    protected override async void OnConnected()
    {
      if (CompositionBrush == null)
      {
        _compositor = Window.Current.Compositor;

        var imageFilePath = "ms-appx:///CompositionBrushes/Assets/TransparencyBackground.png";
        var imageCompositionBrush = await SurfaceLoader.LoadImageAsync(new Uri(imageFilePath), DpiMode.DisplayDpi);

        var borderEffect = new BorderEffect
        {
          ExtendX = CanvasEdgeBehavior.Wrap,
          ExtendY = CanvasEdgeBehavior.Wrap,
          Source = new CompositionEffectSourceParameter("ImageCompositionBrush"),
        };

        var effectFactory = _compositor.CreateEffectFactory(borderEffect);

        var effectBrush = effectFactory.CreateBrush();
        effectBrush.SetSourceParameter("ImageCompositionBrush", imageCompositionBrush);

        CompositionBrush = effectBrush;
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