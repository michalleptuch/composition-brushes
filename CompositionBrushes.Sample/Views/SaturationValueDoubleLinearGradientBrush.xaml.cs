using Microsoft.Toolkit.Uwp.Helpers;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace CompositionBrushes.Sample.Views
{
  public sealed partial class SaturationValueDoubleLinearGradientBrush : Page
  {
    public SaturationValueDoubleLinearGradientBrush()
    {
      InitializeComponent();
    }

    private void UpdateColor(object sender, RangeBaseValueChangedEventArgs e)
    {
      BackgroundBrush.Color = ColorHelper.FromHsv(e.NewValue, 1, 1);
    }
  }
}