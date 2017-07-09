using System;
using System.Linq;
using System.Diagnostics;
using Foundation;
using HealthKit;
using UIKit;
using System.Threading.Tasks;
using OxyPlot.Xamarin.iOS;
using OxyPlot;
using OxyPlot.Series;
using CoreGraphics;
using OxyPlot.Axes;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MySteps
{
    public partial class ViewController : UIViewController
    {
        private HealthStore healthKitStore;
        private NSObject notificationHandle;

        private List<DataItem> stepData = new List<DataItem>();

        public ViewController(IntPtr handle) : base(handle)
        {
            this.healthKitStore = new HealthStore();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.StartDatePicker.ValueChanged += async (s, e) => await GetStepCount();
            this.EndDatePicker.ValueChanged += async (s, e) => await GetStepCount();

            notificationHandle = NSNotificationCenter.DefaultCenter.AddObserver(
                UIApplication.WillEnterForegroundNotification,
                async a => await GetStepCount());


            this.PlotView.Model = new PlotModel()
            {
                IsLegendVisible = false,
                Padding = new OxyThickness(8),
                PlotAreaBorderThickness = new OxyThickness(0),
                Series = {
                    new ColumnSeries()
                    {
                        ItemsSource = this.stepData,
                        ValueField = "Value",
                        LabelFormatString = "{0}"
                    }
                },
                Axes =
                {
                    new CategoryAxis{ ItemsSource = this.stepData, MaximumRange = 7, MinimumRange = 0, LabelField = "Start", StringFormat = "M/dd", TickStyle = TickStyle.None, IsPanEnabled = true, IsZoomEnabled = false },
                    new LinearAxis{ IsAxisVisible = false , IsPanEnabled = false, IsZoomEnabled = false}
                }
            };

            this.PlotView.Model.Axes[0].AxisChanged += (s, e) =>
            {
                var ax = (Axis)s;
                Console.WriteLine($"{e.ChangeType} : {e.DeltaMinimum}, {e.DeltaMinimum}");
                Console.WriteLine($"  {ax.Minimum}, {ax.Maximum}");
                Console.WriteLine($"  Actual={ax.ActualMinimum}, {ax.ActualMaximum}");
                Console.WriteLine($"  Absolute={ax.AbsoluteMinimum}, {ax.AbsoluteMaximum}");
                Console.WriteLine($"  Data={ax.DataMinimum}, {ax.DataMaximum}");
                Console.WriteLine($"  Filter={ax.FilterMinValue}, {ax.FilterMaxValue}");
                Console.WriteLine("");
            };
        }

        public override async void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            var firstDateOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            this.StartDatePicker.SetDate(firstDateOfThisMonth.ToNSDate(), true);
            this.EndDatePicker.SetDate(DateTime.Now.Date.ToNSDate(), true);

            await this.healthKitStore.Initialize();
            await GetStepCount();
        }

        public override void ViewDidUnload()
        {
            base.ViewDidUnload();

            NSNotificationCenter.DefaultCenter.RemoveObserver(notificationHandle);
        }

        private async Task GetStepCount()
        {
            var from = this.StartDatePicker.Date.ToDateTime();
            var to = this.EndDatePicker.Date.ToDateTime().AddDays(1);

            var steps = await this.healthKitStore.GetSteps(from, to);

            this.stepData.Clear();
            this.stepData.AddRange(steps);

            var nowUtc = DateTime.Now.Date.ToUniversalTime();
            var today = (await this.healthKitStore.GetSteps(nowUtc, nowUtc.AddDays(1))).FirstOrDefault();

            InvokeOnMainThread(() =>
            {
                this.PlotView.Model.InvalidatePlot(true);

                foreach (var ax in this.PlotView.Model.Axes.OfType<CategoryAxis>())
                {
                    ax.Reset();
                    ax.MinimumRange = Math.Min(ax.MaximumRange, ax.DataMaximum - ax.DataMinimum);
                    ax.AbsoluteMinimum = ax.DataMinimum;
                    ax.AbsoluteMaximum = ax.DataMaximum;
                    ax.Pan(double.MinValue);
                }

                var total = steps.Sum(s => s.Value);
                this.TotalLabel.Text = total.ToString("N0");
                this.AverageLabel.Text = (total / (to - from).TotalDays).ToString("N0");
                this.TodayLabel.Text = today?.Value.ToString("N0");
            });
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}