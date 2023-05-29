using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Mvvm;

namespace PrismCommunitytoolkitApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Prism Application";

        [ObservableProperty]
        private string _currentTime;

        [ObservableProperty]
        private PlotModel _plotModel;
        public MainWindowViewModel()
        {
            InitializePlotModel();
            
        }
        [ObservableProperty]
        private ObservableCollection<DataViewModel> _data = new ObservableCollection<DataViewModel>();
        private void InitializePlotModel()
        {
            PlotModel = new PlotModel();
            PlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom
            });
            PlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left
            });

            var line = new LineSeries()
            {
                ItemsSource = Data,
                DataFieldX = "X",
                DataFieldY = "Y"
            };

            PlotModel.Series.Add(line);
        }
        [RelayCommand]
        private void ButtonClick()
        {
            CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            Data.Add(new DataViewModel()
            {
                X =Data.Count,Y=Data.Count*2/3
            });
            PlotModel.InvalidatePlot(true);
        }
        [RelayCommand]
        private async Task RealTimeOxyPlot()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            await Task.Factory.StartNew(async () =>
            {
                var random = new Random();
                while (true)
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        break;
                    }

                    Data.Add(new DataViewModel()
                    {
                        X = Data.Count,Y = random.Next(0,100)
                    });
                    PlotModel.InvalidatePlot(true);
                   await  Task.Delay(1000);
                }
            },_cancellationTokenSource.Token);
        }
        [RelayCommand]
        private void StopRealtimeOxyPlot()
        {
            _cancellationTokenSource.Cancel();
        }

        private CancellationTokenSource _cancellationTokenSource;
    }

    public partial class DataViewModel:ObservableObject
    {
        [ObservableProperty]
        private int _x;
        [ObservableProperty]
        private int _y;
    }
}
