using System;
using System.Collections.Generic;

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Toolkit.Uwp.Helpers;

using Windows.Graphics.Effects;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace CompositionBrushes.Factories
{
  internal class ActiveAcrylicBrushFactory
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

      ////Noise image
      var noiseEffectSourceParameter = new CompositionEffectSourceParameter("Noise");

      var imagesurface = LoadedImageSurface.StartLoadFromUri(new Uri("ms-appx:///CompositionBrushes/Assets/NoiseAsset_256X256_PNG.png"));
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

      var hsvTintColor = tintColor.ToHsv();

      var clampedHsvV = Math.Clamp(hsvTintColor.V, minHsvV, maxHsvV);

      var luminosityColor = Microsoft.Toolkit.Uwp.Helpers.ColorHelper.FromHsv(hsvTintColor.H, hsvTintColor.S, clampedHsvV, hsvTintColor.A);

      var minLuminosityOpacity = 0.15;
      var maxLuminosityOpacity = 1.03;
      var luminosityOpacityRangeMax = maxLuminosityOpacity - minLuminosityOpacity;

      var mappedTintOpacity = tintColor.A / 255.0 * luminosityOpacityRangeMax + minLuminosityOpacity;

      return Color.FromArgb((byte)Math.Round(Math.Min(mappedTintOpacity, 1.0) * 255), luminosityColor.R, luminosityColor.G, luminosityColor.B);
    }
  }
}