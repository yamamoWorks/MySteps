using System;
using System.Linq;
using System.Diagnostics;
using Foundation;
using HealthKit;
using UIKit;

namespace MySteps
{
    public partial class ViewController : UIViewController
    {
        private HKHealthStore healthKitStore;

        public ViewController(IntPtr handle) : base(handle)
        {
            this.healthKitStore = new HKHealthStore();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.StartDatePicker.ValueChanged += (s, e) => GetStepCount();
            this.EndDatePicker.ValueChanged += (s, e) => GetStepCount();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            var firstDateOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            this.StartDatePicker.SetDate(firstDateOfThisMonth.ToNSDate(), true);
            this.EndDatePicker.SetDate(DateTime.Now.Date.ToNSDate(), true);
            GetStepCount();
        }
        
        private void GetStepCount()
        {
            var query = new HKStatisticsCollectionQuery(
                HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount),
                HKQuery.GetPredicateForSamples(this.StartDatePicker.Date, this.EndDatePicker.Date.AddSeconds(60 * 60 * 24), HKQueryOptions.None),
                HKStatisticsOptions.SeparateBySource | HKStatisticsOptions.CumulativeSum,
                this.StartDatePicker.Date,
                new NSDateComponents { Day = 1 });

            query.InitialResultsHandler = (q, result, error) =>
            {
                InvokeOnMainThread(() =>
                {
                    var total = result.Statistics.Sum(s => Math.Round(s.SumQuantity().GetDoubleValue(HKUnit.Count)));
                    this.TotalLabel.Text = total.ToString("N0");
                    var span = result.Statistics.LastOrDefault()?.EndDate.ToDateTime().Date - this.StartDatePicker.Date.ToDateTime().Date;
                    this.AvarageLabel.Text = (total / span?.TotalDays)?.ToString("N0");
                });
            };

            this.healthKitStore.ExecuteQuery(query);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
} 