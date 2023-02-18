using System;
using System.Collections.Generic;

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

using Windows.Graphics.Effects;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace AlwaysOnAcrylicBrushSample
{
  public class AlwaysOnAcrylicBrushFactory
  {
    private string TintColorColor = "TintColor.Color";
    private string LuminosityColorColor = "LuminosityColor.Color";
    private string FallbackColorColor = "FallbackColor.Color";

    public CompositionEffectBrush CreateActiveAcrylicBrush(
      Color initialTintColor,
      float tintOpacity,
      float? luminosityOpacity,
      Color fallbackColor,
      bool useFallback)
    {
      ////Compositor
      var compositor = Window.Current.Compositor;

      ////Colors
      var tintColor = GetEffectiveTintColor(initialTintColor, tintOpacity);
      var luminosityColor = GetEffectiveLuminosityColor(initialTintColor, tintOpacity, luminosityOpacity);

      ////Factory
      CompositionEffectFactory effectFactory = null;
      var animatedProperties = new List<string>();

      var tintColorEffect = new ColorSourceEffect();
      tintColorEffect.Name = "TintColor";
      tintColorEffect.Color = tintColor;

      animatedProperties.Add(TintColorColor);

      var backdropEffectSourceParameter = new CompositionEffectSourceParameter("Backdrop");

      IGraphicsEffectSource blurredSource;

      blurredSource = backdropEffectSourceParameter;

      var luminosityColorEffect = new ColorSourceEffect();
      luminosityColorEffect.Name = "LuminosityColor";
      luminosityColorEffect.Color = luminosityColor;

      var luminosityBlendEffect = new BlendEffect();
      luminosityBlendEffect.Mode = BlendEffectMode.Color;
      luminosityBlendEffect.Background = blurredSource;
      luminosityBlendEffect.Foreground = luminosityColorEffect;

      var colorBlendEffect = new BlendEffect();
      colorBlendEffect.Mode = BlendEffectMode.Luminosity;
      colorBlendEffect.Background = luminosityBlendEffect;
      colorBlendEffect.Foreground = tintColorEffect;

      ////Noise
      var noiseEffectSourceParameter = new CompositionEffectSourceParameter("Noise");

      var imagesurface = LoadedImageSurface.StartLoadFromUri(new Uri("ms-appx:///Images/NoiseAsset_256X256_PNG.png"));
      var imagebrush = compositor.CreateSurfaceBrush(imagesurface);
      imagebrush.Stretch = CompositionStretch.None;

      animatedProperties.Add(LuminosityColorColor);

      var noiseBorderEffect = new BorderEffect();
      noiseBorderEffect.ExtendX = CanvasEdgeBehavior.Wrap;
      noiseBorderEffect.ExtendY = CanvasEdgeBehavior.Wrap;
      noiseBorderEffect.Source = noiseEffectSourceParameter;

      var blendEffectOuter = new ArithmeticCompositeEffect();
      blendEffectOuter.MultiplyAmount = 0f;
      blendEffectOuter.Source1 = noiseBorderEffect;
      blendEffectOuter.Source1Amount = 0.02f;
      blendEffectOuter.Source2 = colorBlendEffect;
      blendEffectOuter.Source2Amount = 0.98f;

      if (useFallback)
      {
        var fallbackColorEffect = new ColorSourceEffect();
        fallbackColorEffect.Name = "FallbackColor";
        fallbackColorEffect.Color = fallbackColor;

        var fadeInOutEffect = new CrossFadeEffect();
        fadeInOutEffect.Name = "FadeInOut";
        fadeInOutEffect.Source1 = fallbackColorEffect;
        fadeInOutEffect.Source2 = blendEffectOuter;
        fadeInOutEffect.CrossFade = 1.0f;

        animatedProperties.Add(FallbackColorColor);
        animatedProperties.Add("FadeInOut.CrossFade");

        effectFactory = compositor.CreateEffectFactory(fadeInOutEffect, animatedProperties);
      }
      else
      {
        effectFactory = compositor.CreateEffectFactory(blendEffectOuter, animatedProperties);
      }

      ////Worker
      var acrylicBrush = effectFactory.CreateBrush();

      var hostBackdropBrush = compositor.CreateHostBackdropBrush();
      acrylicBrush.SetSourceParameter("Backdrop", hostBackdropBrush);
      acrylicBrush.SetSourceParameter("Noise", imagebrush);

      return acrylicBrush;
    }

    private Color GetEffectiveTintColor(Color tintColor, double tintOpacity)
    {
      tintColor.A = (byte)Math.Round(tintColor.A * tintOpacity);

      return tintColor;
    }

    private Color GetEffectiveLuminosityColor(Color tintColor, double tintOpacity, double? tintLuminosityOpacity)
    {
      tintColor.A = (byte)Math.Round(tintColor.A * tintOpacity);

      return GetLuminosityColor(tintColor, tintLuminosityOpacity);
    }

    private Color GetLuminosityColor(Color tintColor, double? luminosityOpacity)
    {
      if (luminosityOpacity != null)
      {
        return Color.FromArgb((byte)Math.Round((double)luminosityOpacity * 255), tintColor.R, tintColor.G, tintColor.B);
      }

      var minHsvV = 0.125;
      var maxHsvV = 0.965;

      var hsvTintColor = ToHsv(tintColor);

      var clampedHsvV = Math.Clamp(hsvTintColor.V, minHsvV, maxHsvV);

      var luminosityColor = FromHsv(hsvTintColor.H, hsvTintColor.S, clampedHsvV, hsvTintColor.A);

      var minLuminosityOpacity = 0.15;
      var maxLuminosityOpacity = 1.03;
      var luminosityOpacityRangeMax = maxLuminosityOpacity - minLuminosityOpacity;

      var mappedTintOpacity = tintColor.A / 255.0 * luminosityOpacityRangeMax + minLuminosityOpacity;

      return Color.FromArgb((byte)Math.Round(Math.Min(mappedTintOpacity, 1.0) * 255), luminosityColor.R, luminosityColor.G, luminosityColor.B);
    }

    private HsvColor ToHsv(Color color)
    {
      const double toDouble = 1.0 / 255;
      var r = toDouble * color.R;
      var g = toDouble * color.G;
      var b = toDouble * color.B;
      var max = Math.Max(Math.Max(r, g), b);
      var min = Math.Min(Math.Min(r, g), b);
      var chroma = max - min;
      double h1;

      if (chroma == 0)
      {
        h1 = 0;
      }
      else if (max == r)
      {
        h1 = (((g - b) / chroma) + 6) % 6;
      }
      else if (max == g)
      {
        h1 = 2 + ((b - r) / chroma);
      }
      else
      {
        h1 = 4 + ((r - g) / chroma);
      }

      double saturation = chroma == 0 ? 0 : chroma / max;
      HsvColor ret;
      ret.H = 60 * h1;
      ret.S = saturation;
      ret.V = max;
      ret.A = toDouble * color.A;
      return ret;
    }

    private Color FromHsv(double hue, double saturation, double value, double alpha = 1.0)
    {
      if (hue < 0 || hue > 360)
      {
        throw new ArgumentOutOfRangeException(nameof(hue));
      }

      double chroma = value * saturation;
      double h1 = hue / 60;
      double x = chroma * (1 - Math.Abs((h1 % 2) - 1));
      double m = value - chroma;
      double r1, g1, b1;

      if (h1 < 1)
      {
        r1 = chroma;
        g1 = x;
        b1 = 0;
      }
      else if (h1 < 2)
      {
        r1 = x;
        g1 = chroma;
        b1 = 0;
      }
      else if (h1 < 3)
      {
        r1 = 0;
        g1 = chroma;
        b1 = x;
      }
      else if (h1 < 4)
      {
        r1 = 0;
        g1 = x;
        b1 = chroma;
      }
      else if (h1 < 5)
      {
        r1 = x;
        g1 = 0;
        b1 = chroma;
      }
      else
      {
        r1 = chroma;
        g1 = 0;
        b1 = x;
      }

      byte r = (byte)(255 * (r1 + m));
      byte g = (byte)(255 * (g1 + m));
      byte b = (byte)(255 * (b1 + m));
      byte a = (byte)(255 * alpha);

      return Color.FromArgb(a, r, g, b);
    }

    private struct HsvColor
    {
      public double H;

      public double S;

      public double V;

      public double A;
    }
  }
}