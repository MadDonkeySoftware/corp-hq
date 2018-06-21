// Copyright (c) MadDonkeySoftware

namespace Runner.Data.Mongo
{
    using System;
    using System.Linq;
    using Common.Data;
    using Common.Model;
    using MongoDB;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Repository responsible for job retrieval and persistance
    /// </summary>
    public class JobRepository : BaseRepository, IJobRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobRepository"/> class.
        /// </summary>
        /// <param name="dbFactory">The dbFactory for this job to use.</param>
        public JobRepository(IDbFactory dbFactory)
            : base(dbFactory)
        {
        }

        /// <summary>
        /// Gets the job data associated with the job.
        /// </summary>
        /// <param name="jobUuid">The unique identifier for the job.</param>
        /// <typeparam name="T">The type to deserialize the job data to.</typeparam>
        /// <returns>A object representing the job data.</returns>
        public T GetJobData<T>(string jobUuid)
        {
            var jobCol = this.DbFactory.GetCollection<JobSpec<string>>(CollectionNames.Jobs);
            var jobData = jobCol.AsQueryable().Where(j => j.Uuid == jobUuid).Select(j => j.Data).FirstOrDefault();

            if (string.IsNullOrEmpty(jobData))
            {
                throw new NullReferenceException("No job data could be found for job.");
            }

            return JsonConvert.DeserializeObject<T>(jobData);
        }

        /// <summary>
        /// Adds a message to the associated job at the specified level.
        /// </summary>
        /// <param name="jobSpec">The job spec.</param>
        /// <param name="level">The message level.</param>
        /// <param name="message">The message.</param>
        public void AddMessageToJob(JobSpecLite jobSpec, JobMessageLevel level, string message)
        {
            JobMessageRepository.InternalAddMessageToJob(this.DbFactory, jobSpec, level, message);
        }

        /// <summary>
        /// Updates the job status and starting timestamp.
        /// </summary>
        /// <param name="jobSpec">The job spec.</param>
        /// <param name="status">The new status for the job.</param>
        /// <param name="timestamp">The new timestamp for the job's start.</param>
        public void UpdateStatusAndStartTimestamp(JobSpecLite jobSpec, string status, DateTime timestamp)
        {
            var jobCol = this.DbFactory.GetCollection<JobSpec<string>>(CollectionNames.Jobs);
            jobCol.UpdateOne(
                Builders<JobSpec<string>>.Filter.Eq(j => j.Uuid, jobSpec.Uuid),
                Builders<JobSpec<string>>.Update.Set(j => j.Status, status).Set(j => j.StartTimestamp, timestamp));
        }

        /// <summary>
        /// Updates the job status and end timestamp.
        /// </summary>
        /// <param name="jobSpec">The job spec.</param>
        /// <param name="status">The new status for the job.</param>
        /// <param name="timestamp">The new timestamp for the job's start.</param>
        public void UpdateStatusAndEndTimestamp(JobSpecLite jobSpec, string status, DateTime timestamp)
        {
            var jobCol = this.DbFactory.GetCollection<JobSpec<string>>(CollectionNames.Jobs);
            jobCol.UpdateOne(
                Builders<JobSpec<string>>.Filter.Eq(j => j.Uuid, jobSpec.Uuid),
                Builders<JobSpec<string>>.Update.Set(j => j.Status, status).Set(j => j.EndTimestamp, timestamp));
        }
    }
}