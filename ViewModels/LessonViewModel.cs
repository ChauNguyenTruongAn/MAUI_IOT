﻿using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.Defaults;
using LiveChartsCore;
using MAUI_IOT.Models;
using System.Collections.ObjectModel;
using LiveChartsCore.SkiaSharpView;
using Newtonsoft.Json;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using MAUI_IOT.Views;
using Microsoft.Maui.Controls;
using System.Diagnostics;


namespace MAUI_IOT.ViewModels
{
    [QueryProperty(nameof(Lesson), "data")]
    public partial class LessonViewModel : ObservableObject
    {
        [ObservableProperty]
        public Lesson lesson;

        private static readonly SKColor s_gray = new(195, 195, 195);
        private static readonly SKColor s_gray1 = new(160, 160, 160);
        private static readonly SKColor s_gray2 = new(90, 90, 90);
        private static readonly SKColor s_dark3 = new(60, 60, 60);

        private readonly DateTimeAxis _customAxis;

        private readonly ObservableCollection<ObservableValue> xAxis;
        private readonly ObservableCollection<ObservableValue> yAxis;
        private readonly ObservableCollection<ObservableValue> zAxis;
        private readonly IServiceProvider serviceProvider;
        private readonly FullScreenChartViewModel fullScreenChartViewModel;

        public ObservableCollection<ISeries> SeriesX { get; set; }
        public ObservableCollection<ISeries> SeriesY { get; set; }
        public ObservableCollection<ISeries> SeriesZ { get; set; }

        public ObservableCollection<double> a { get; set; }
        public ObservableCollection<double> F { get; set; }
        public ObservableCollection<DateTime> Time { get; set; }
        public ObservableCollection<TimeSpan> Duration { get; set; }

        //dữ liệu khối lượng
        [ObservableProperty]
        private double m;

        ADXL345Sensor ADXL345Sensor { get; set; }

        private CustomAxis aDXL345Axis;

        public CustomAxis ADXL345Axis
        {
            get => aDXL345Axis;
            set
            {
                aDXL345Axis = value;
                OnPropertyChanged(nameof(ADXL345Axis));
            }
        }

        public ObservableCollection<ISeries> Series { get; set; }


        public Axis[] XAxes { get; set; } =
        {
            new Axis
            {
                Name = "X axis",
                NamePaint = new SolidColorPaint(s_gray1),
                TextSize = 18,
                Padding = new Padding(5, 15, 5, 5),
                LabelsPaint = new SolidColorPaint(s_gray),
                SeparatorsPaint = new SolidColorPaint
                {
                    Color = s_gray,
                    StrokeThickness = 1,
                    PathEffect = new DashEffect(new float[] { 3, 3 })
                },
                SubseparatorsPaint = new SolidColorPaint
                {
                    Color = s_gray2,
                    StrokeThickness = 0.5f
                },
                SubseparatorsCount = 9,
                ZeroPaint = new SolidColorPaint
                {
                    Color = s_gray1,
                    StrokeThickness = 2
                },
                TicksPaint = new SolidColorPaint
                {
                    Color = s_gray,
                    StrokeThickness = 1.5f
                },
                SubticksPaint = new SolidColorPaint
                {
                    Color = s_gray,
                    StrokeThickness = 1
                }
            }
        };


        public Axis[] YAxes { get; set; } =
        {
            new Axis
            {
                Name = "Y axis",
                NamePaint = new SolidColorPaint(s_gray1),
                TextSize = 18,
                Padding = new Padding(5, 0, 15, 0),
                LabelsPaint = new SolidColorPaint(s_gray),
                SeparatorsPaint = new SolidColorPaint
                {
                    Color = s_gray,
                    StrokeThickness = 1,
                    PathEffect = new DashEffect(new float[] { 3, 3 })
                },
                SubseparatorsPaint = new SolidColorPaint
                {
                    Color = s_gray2,
                    StrokeThickness = 0.5f
                },
                SubseparatorsCount = 9,
                ZeroPaint = new SolidColorPaint
                {
                    Color = s_gray1,
                    StrokeThickness = 2
                },
                TicksPaint = new SolidColorPaint
                {
                    Color = s_gray,
                    StrokeThickness = 1.5f
                },
                SubticksPaint = new SolidColorPaint
                {
                    Color = s_gray,
                    StrokeThickness = 1
                }
            }
        };

        public LessonViewModel()
        {

        }

        public LessonViewModel(IServiceProvider serviceProvider, FullScreenChartViewModel fullScreenChartViewModel)
        {
            xAxis = new ObservableCollection<ObservableValue>
            {
                // new ObservableValue(5),
                //new(10),
                //new(15),
                //new(20),
                //new(25),
                //new(30),
            };
            yAxis = new ObservableCollection<ObservableValue>();
            zAxis = new ObservableCollection<ObservableValue>();
            
            Series = new ObservableCollection<ISeries>
            {
                new LineSeries<ObservableValue>
                {
                    Values = xAxis,
                    Fill = null,
                    LineSmoothness = 0,
                    GeometrySize = 0,
                },
                new LineSeries<ObservableValue>
                {
                    Values = yAxis,
                    Fill = null,
                    LineSmoothness = 0,
                    GeometrySize = 0,

                },
                new LineSeries<ObservableValue>
                {
                    Values = zAxis,
                    Fill = null,
                    LineSmoothness = 0,
                    GeometrySize = 0,

                },
            };


            SeriesX = new ObservableCollection<ISeries>
            {
                new LineSeries<ObservableValue>
                {
                    Values = xAxis,
                    Fill = null,
                    LineSmoothness = 0,
                    Stroke = new SolidColorPaint(SKColors.White),
                    GeometrySize = 2
                }
            };

            SeriesY = new ObservableCollection<ISeries>
            {
                new LineSeries<ObservableValue>
                {
                    Values = yAxis,
                    Fill = null,
                    LineSmoothness = 0,
                    Stroke = new SolidColorPaint(SKColors.White),
                    GeometrySize = 0
                }
            };

            SeriesZ = new ObservableCollection<ISeries>
            {
                new LineSeries<ObservableValue>
                {
                    Values = zAxis,
                    Fill = null,
                    LineSmoothness = 0,
                    Stroke = new SolidColorPaint(SKColors.White),
                    GeometrySize = 0
                }
            };


            aDXL345Axis = new Models.CustomAxis();
            ADXL345Sensor = new ADXL345Sensor();
            ADXL345Sensor.PropertyChanged += ADXL345Sensor_PropertyChanged;
            this.serviceProvider = serviceProvider;
            this.fullScreenChartViewModel = fullScreenChartViewModel;

            //Khởi tạo a
            a = new ObservableCollection<double>();
            F = new ObservableCollection<double>();
            Time = new ObservableCollection<DateTime>();
            Duration = new ObservableCollection<TimeSpan>();
        }

        private async void ADXL345Sensor_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ADXL345Sensor.ReceivedData))
            {
                ADXL345Axis = JsonConvert.DeserializeObject<Models.CustomAxis>(ADXL345Sensor.ReceivedData);
                AddItem(ADXL345Axis.x, ADXL345Axis.y, ADXL345Axis.z);
                caculateA(ADXL345Axis.x, ADXL345Axis.y, ADXL345Axis.z, ADXL345Axis.TimeStamp);
                RemoveItem();
            }
        }

        public void AddItem(float x, float y, float z)
        {
            xAxis.Add(new ObservableValue(x));
            yAxis.Add(new ObservableValue(y));
            zAxis.Add(new ObservableValue(z));
        }

        public void RemoveItem()
        {
            if (xAxis.Count >= 100)
            {
                if (xAxis.Count == 0) return;
                xAxis.RemoveAt(0);

                if (yAxis.Count == 0) return;
                yAxis.RemoveAt(0);

                if (zAxis.Count == 0) return;
                zAxis.RemoveAt(0);
            }
        }

        [RelayCommand]
        async Task Start()
        {
            await ADXL345Sensor.ConnectAsync(new Uri("ws://113.161.84.132:8800/api/adxl345"));
        }

        [RelayCommand]
        async Task Stop()
        {
            await ADXL345Sensor.CloseAsync();
            Debug.Write(a.Count + " " + F.Count + " " + Time.Count);
            await DataBinding();
        }

        [RelayCommand]
        void Save()
        {

        }

        public DrawMarginFrame Frame { get; set; } = new()
        {
            Fill = new SolidColorPaint(s_dark3),
            Stroke = new SolidColorPaint
            {
                Color = s_gray,
                StrokeThickness = 1
            }
        };

        [RelayCommand]
        public async Task Zoom(ObservableCollection<ISeries> series)
        {
            Dictionary<string, object> paramaters = new Dictionary<string, object>
            {
                {"data",series },
            };
            await Shell.Current.GoToAsync(nameof(FullScreenChartView), paramaters);
        }

        [ObservableProperty]
        private double avgF;

        public async Task caculateA(double x, double y, double z, DateTime time)
        {
            await Task.Run(() =>
            {
                double result = Math.Sqrt(x * x + y * y + z * z);
                MainThread.BeginInvokeOnMainThread(() => { a.Add(result); F.Add(result * m); Time.Add(time);});
            }); 
        }


        //dữ liệu cho bảng
        private async Task DataBinding()
        {
            if (a.Count == 0 || Time.Count == 0 || a.Count != F.Count || a.Count != Time.Count)
            {
                Debug.WriteLine("Dữ liệu không hợp lệ.");
                return;
            }

            int n = a.Count;
            if (n <= 1) return;

            TimeSpan diff = Time[n - 1] - Time[0];

            // số mẫu lấy trong một 100ms
            int duration = 15;

            
            //Lưu các index để xóa
            List<int> listRemove = new List<int>();
            //Tạo biến hứng giá trị
            ObservableCollection<double> _a = new ObservableCollection<double>();
            ObservableCollection<double> _F = new ObservableCollection<double>();
            ObservableCollection<DateTime> _time = new ObservableCollection<DateTime>();
            for (int i = 0; i < n; i++)
            {
                if (i % duration == 0)
                {
                    _a.Add(a[i]);
                    _F.Add(F[i]);
                    _time.Add(Time[i]);
                }
            }
            a = _a;
            F = _F;
            Time = _time;
            

            for(int i = 0; i < Time.Count; i++)
                Duration.Add(Time[i] - Time[0]);


            avgF = F.Any() ? F.Average() : 0;

            //Debug.WriteLine($"{a.Count} {F.Count} {Time.Count} {m} {diff.TotalMilliseconds}");
        }
    }
}