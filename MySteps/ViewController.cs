using System;
using System.Linq;
using System.Diagnostics;
using Foundation;
using HealthKit;
using UIKit;
using System.Threading.Tasks;

namespace MySteps
{
    public partial class ViewController : UIViewController
    {
        private HealthStore healthKitStore;

        public ViewController(IntPtr handle) : base(handle)
        {
            this.healthKitStore = new HealthStore();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.StartDatePicker.ValueChanged += async (s, e) => await GetStepCount();
            this.EndDatePicker.ValueChanged += async (s, e) => await GetStepCount();
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

        private async Task GetStepCount()
        {
            var from = this.StartDatePicker.Date.ToDateTime();
            var to = this.EndDatePicker.Date.ToDateTime().AddDays(1);

            var steps = await this.healthKitStore.GetSteps(from, to);

            InvokeOnMainThread(() =>
            {
                this.TotalLabel.Text = steps.ToString("N0");
                this.AverageLabel.Text = (steps / (to - from).TotalDays).ToString("N0");
            });
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}