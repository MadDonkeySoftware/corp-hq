// Copyright (c) MadDonkeySoftware

namespace Manager.Adjudication
{
    using System;
    using Common;
    using Common.Model;
    using Common.Model.JobData;
    using Newtonsoft.Json;

    /// <summary>
    /// The class for monitoring RabbitMQ queues
    /// </summary>
    [Adjudicator(JobTypes.ImportMarketData)]
    public class ImportMarketDataAdjudicator : IAdjudicator
    {
        /// <summary>
        /// Checks to see if the job should skip processing.
        /// </summary>
        /// <param name="job">The job to verify</param>
        /// <returns>True if processing should be skipped, false otherwise.</returns>
        public bool SkipProcessing(JobSpec<string> job)
        {
            return job.Children.Count > 0;
        }

        /// <summary>
        /// Checks to see if the job should be split up.
        /// </summary>
        /// <param name="job">The job to verify</param>
        /// <returns>True if the job should be split up, false otherwise.</returns>
        public bool RequiresSplit(JobSpec<string> job)
        {
            var jobData = JsonConvert.DeserializeObject<MarketDataImport>(job.Data);
            return jobData.MarketTypeIds.Count > 5;
        }

        /// <summary>
        /// Splits the provided job up into smaller child jobs
        /// </summary>
        /// <param name="job">The job to split.</param>
        /// <returns>The results of the split operation.</returns>
        public SplitResult Split(JobSpec<string> job)
        {
            var result = new SplitResult();
            var jobData = JsonConvert.DeserializeObject<MarketDataImport>(job.Data);

            const int step = 5;
            var loopMax = (jobData.MarketTypeIds.Count / step) + 1;
            for (var marker = 0; marker < loopMax; marker++)
            {
                var start = marker * step;
                var count = start + step < jobData.MarketTypeIds.Count ?
                    step :
                    jobData.MarketTypeIds.Count - start;
                var slice = jobData.MarketTypeIds.GetRange(start, count);

                var newJobUuid = Guid.NewGuid().ToString();
                result.ChildJobs.Add(new JobSpec<string>
                {
                    Uuid = newJobUuid,
                    ParentUuid = job.Uuid,
                    MasterUuid = !string.IsNullOrEmpty(job.MasterUuid) ? job.MasterUuid : job.Uuid,
                    Type = job.Type,
                    Data = JsonConvert.SerializeObject(new MarketDataImport() { RegionId = jobData.RegionId, MarketTypeIds = slice }),
                    Status = JobStatuses.New,
                    ExpireAt = DateTime.Now.AddDays(3)
                });
            }

            return result;
        }
    }
}