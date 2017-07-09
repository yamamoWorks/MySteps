using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using HealthKit;
using System.Diagnostics;
using System.Collections.ObjectModel;

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

        public Task<DataItem[]> GetSteps(DateTime from, DateTime to)
        {
            var query = new HKStatisticsCollectionQuery(
                HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount),
                HKQuery.GetPredicateForSamples(from.ToNSDate(), to.ToNSDate(), HKQueryOptions.None),
                HKStatisticsOptions.SeparateBySource | HKStatisticsOptions.CumulativeSum,
                from.ToNSDate(),
                new NSDateComponents { Day = 1 });

            var tcs = new TaskCompletionSource<DataItem[]>();

            query.InitialResultsHandler = (q, result, error) =>
            {
                if (error != null)
                    throw new Exception(error.Description);

                tcs.TrySetResult(
                    result.Statistics.Where(s => s.StartDate.ToDateTime() >= from).Select(s =>
                    {
                        return new DataItem
                        {
                            Start = s.StartDate.ToDateTime().ToLocalTime(),
                            Value = Math.Round(s.SumQuantity().GetDoubleValue(HKUnit.Count))
                        };
                    }).ToArray());
            };

            healthKitStore.Value.ExecuteQuery(query);

            return tcs.Task;
        }
    }
}
