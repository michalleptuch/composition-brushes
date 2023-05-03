using System;
using System.Collections.Generic;
using System.Linq;

using CompositionBrushes.Brushes;
using CompositionBrushes.Sample.Models;

using Microsoft.UI.Xaml.Controls;

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CompositionBrushes.Sample
{
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      InitializeComponent();

      if (Environment.OSVersion.Version.Build >= 22000)
      {
        SetValue(BackdropMaterial.ApplyToRootOrPageBackgroundProperty, true);
      }

      Window.Current.SetTitleBar(TitleBarGrid);

      Window.Current.Activated += (s, e) =>
      {
        TitleBarGrid.Opacity = e.WindowActivationState != CoreWindowActivationState.Deactivated ? 1 : 0.5;
      };

      MenuListView.ItemsSource = BuildMenu();
      MenuListView.SelectedIndex = 0;

      MainFrame.Navigate((MenuListView.Items.First() as MenuItem).Id);
    }

    private List<MenuItem> BuildMenu()
    {
      var menuList = new List<MenuItem>();

      menuList.Add(new MenuItem
      {
        Id = typeof(Views.ActiveAcrylicBrush),
        Title = "Active AcrylicBrush",
        BrushName = nameof(ActiveAcrylicBrush),
      });
      menuList.Add(new MenuItem
      {
        Id = typeof(Views.TransparentToColorLinearGradientBrush),
        Title = "Transparent to color gradient",
        BrushName = nameof(TransparentToColorLinearGradientBrush),
      });
      menuList.Add(new MenuItem
      {
        Id = typeof(Views.HueLinearGradientBrush),
        Title = "Hue spectrum gradient",
        BrushName = nameof(HueLinearGradientBrush),
      });
      menuList.Add(new MenuItem
      {
        Id = typeof(Views.TransparencyPlaceholderBrush),
        Title = "Transparency background placeholder",
        BrushName = nameof(TransparencyPlaceholderBrush),
      });
      menuList.Add(new MenuItem
      {
        Id = typeof(Views.SaturationValueDoubleLinearGradientBrush),
        Title = "Saturation-value color picker background",
        BrushName = nameof(SaturationValueDoubleLinearGradientBrush),
      });

      return menuList.OrderBy(x => x.Title).ToList();
    }

    private void Navigate(object sender, ItemClickEventArgs e)
    {
      var menuItem = e.ClickedItem as MenuItem;

      if (MainFrame.CurrentSourcePageType == menuItem.Id)
      {
        return;
      }

      MainFrame.Navigate(menuItem.Id);
    }
  }
}