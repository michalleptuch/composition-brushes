using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;

namespace CompositionBrushes.Sample.Views
{
  public sealed partial class TransparentToColorLinearGradientBrush : Page, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    private float _startPointX;
    private float _startPointY;
    private float _endPointX;
    private float _endPointY;

    public TransparentToColorLinearGradientBrush()
    {
      _startPointX = 0;
      _startPointY = 0;
      _endPointX = 1;
      _endPointY = 1;

      InitializeComponent();
    }

    private Vector2 StartPoint
    {
      get => new Vector2(_startPointX, _startPointY);
    }

    private Vector2 EndPoint
    {
      get => new Vector2 (_endPointX, _endPointY);
    }

    private float StartPointX
    {
      get => _startPointX;
      set
      {
        _startPointX = value;
        OnPropertyChanged(nameof(StartPoint));
      }
    }

    private float StartPointY
    {
      get => _startPointY;
      set
      {
        _startPointY = value;
        OnPropertyChanged(nameof(StartPoint));
      }
    }

    private float EndPointX
    {
      get => _endPointX;
      set
      {
        _endPointX = value;
        OnPropertyChanged(nameof(EndPoint));
      }
    }

    private float EndPointY
    {
      get => _endPointY;
      set
      {
        _endPointY = value;
        OnPropertyChanged(nameof(EndPoint));
      }
    }

    private void OnPropertyChanged([CallerMemberName] string name = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
  }
}