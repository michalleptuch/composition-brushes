using System;

using CompositionBrushes.Sample.Views;

using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CompositionBrushes.Sample
{
  sealed partial class App : Application
  {
    private readonly UISettings _uiSettings;

    public App()
    {
      InitializeComponent();

      _uiSettings = new UISettings();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
      var rootFrame = Window.Current.Content as Frame;

      var view = ApplicationView.GetForCurrentView();
      view.SetPreferredMinSize(new Size(500, 500));

      var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
      coreTitleBar.ExtendViewIntoTitleBar = true;

      _uiSettings.ColorValuesChanged += async (_, __) =>
      {
        await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        {
          SetThemeForTitleBar();
        });
      };

      SetThemeForTitleBar();

      if (rootFrame == null)
      {
        rootFrame = new Frame();

        rootFrame.NavigationFailed += OnNavigationFailed;

        Window.Current.Content = rootFrame;
      }

      if (e.PrelaunchActivated == false)
      {
        if (rootFrame.Content == null)
        {
          rootFrame.Navigate(typeof(MainPage), e.Arguments);
        }

        Window.Current.Activate();
      }
    }

    private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
      throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }

    private void SetThemeForTitleBar()
    {
      var foregroundColor = _uiSettings.GetColorValue(UIColorType.Foreground);
      var isLightTheme = foregroundColor == Color.FromArgb(255, 0, 0, 0);

      var titleBar = ApplicationView.GetForCurrentView().TitleBar;

      var grayColor = Color.FromArgb(255, 128, 128, 128);

      var whiteForegroundColor = Color.FromArgb(255, 255, 255, 255);
      var whitePressedForegroundColor = Color.FromArgb(255, 192, 192, 192);
      var whiteHoverBackgroundColor = Color.FromArgb(24, 255, 255, 255);
      var whitePressedBackgroundColor = Color.FromArgb(16, 255, 255, 255);

      var blackForegroundColor = Color.FromArgb(255, 0, 0, 0);
      var blackPressedForegroundColor = Color.FromArgb(255, 96, 96, 96);
      var blackHoverBackgroundColor = Color.FromArgb(24, 0, 0, 0);
      var blackPressedBackgroundColor = Color.FromArgb(16, 0, 0, 0);

      //// Foreground
      titleBar.ButtonForegroundColor = isLightTheme ? blackForegroundColor : whiteForegroundColor;
      titleBar.ButtonHoverForegroundColor = isLightTheme ? blackForegroundColor : whiteForegroundColor;
      titleBar.ButtonPressedForegroundColor = isLightTheme ? blackPressedForegroundColor : whitePressedForegroundColor;
      titleBar.ButtonInactiveForegroundColor = grayColor;
      titleBar.InactiveForegroundColor = grayColor;
      titleBar.ForegroundColor = isLightTheme ? blackForegroundColor : whiteForegroundColor;

      //// Background
      titleBar.BackgroundColor = Colors.Transparent;
      titleBar.ButtonBackgroundColor = Colors.Transparent;
      titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
      titleBar.ButtonHoverBackgroundColor = isLightTheme ? blackHoverBackgroundColor : whiteHoverBackgroundColor;
      titleBar.ButtonPressedBackgroundColor = isLightTheme ? blackPressedBackgroundColor : whitePressedBackgroundColor;
      titleBar.InactiveBackgroundColor = Colors.Transparent;
    }
  }
}