using Microsoft.Azure.WebJobs;

namespace SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers
{

    public static class JobHostExtensions
    {
        public static async Task<IJobHost> Purge(this IJobHost jobs)
        {
            await jobs.CallAsync(nameof(PurgeFunction));
            return jobs;
        }

        public static async Task<IJobHost> Terminate(this IJobHost jobs)
        {
            await jobs.CallAsync(nameof(TerminateFunction));
            return jobs;
        }

        public static async Task<IJobHost> WaitFor(this IJobHost jobs, string orchestration, TimeSpan? timeout = null)
        {
            await jobs.CallAsync(nameof(WaitForFunction), new Dictionary<string, object>
            {
                ["timeout"] = timeout,
                ["name"] = orchestration
            });

            return jobs;
        }

        public static async Task<IJobHost> ThrowIfFailed(this Task<IJobHost> task)
        {
            var jobs = await task;
            await jobs.CallAsync(nameof(ThrowIfFailedFunction));

            return jobs;
        }
    }
}
