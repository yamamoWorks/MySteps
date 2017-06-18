using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using HealthKit;

namespace MySteps
{
    class HealthStore
    {
        private static Lazy<HKHealthStore> healthKitStore = new Lazy<HKHealthStore>(() => new HKHealthStore());

        public HealthStore()
        {
        }

        public Task Initialize()
        {
            var typesToWrite = new NSSet();
            var typesToRead = new NSSet(new[] { HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount) });
            return healthKitStore.Value.RequestAuthorizationToShareAsync(typesToWrite, typesToRead);
        }

        public Task<(double Total, TimeSpan Span)> GetSteps(DateTime from, DateTime to)
        {
            var query = new HKStatisticsCollectionQuery(
                HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount),
                HKQuery.GetPredicateForSamples(from.ToNSDate(), to.AddDays(1).ToNSDate(), HKQueryOptions.None),
                HKStatisticsOptions.SeparateBySource | HKStatisticsOptions.CumulativeSum,
                from.ToNSDate(),
                new NSDateComponents { Day = 1 });

            var tcs = new TaskCompletionSource<(double, TimeSpan)>();

            query.InitialResultsHandler = (q, result, error) =>
            {
                if (error != null)
                    throw new Exception(error.Description);

                var total = result.Statistics.Sum(s => Math.Round(s.SumQuantity().GetDoubleValue(HKUnit.Count)));
                var span = (result.Statistics.LastOrDefault()?.EndDate.ToDateTime().Date - from.Date) ?? TimeSpan.Zero;
                tcs.TrySetResult((total, span));
            };

            healthKitStore.Value.ExecuteQuery(query);

            return tcs.Task;
        }
    }
}
