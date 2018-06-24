// Copyright (c) MadDonkeySoftware

namespace ApiTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using System.Linq;

    using Api.Controllers.V1;
    using Api.Model;
    using Api.Model.Data;
    using Api.Model.Response;
    using ApiTests.EqualityComparers;
    using Common.Data;
    using Common.Model;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using Moq;
    using Newtonsoft.Json;
    using RabbitMQ.Client;
    using Xunit;

    /// <summary>
    /// Tests for Job controller actions.
    /// </summary>
    public class JobControllerTest : BaseControllerTest<JobController>
    {
        private readonly Mock<ILogger<JobController>> loggerMock;
        private readonly Mock<IDbFactory> dbFactoryMock;
        private readonly Mock<ISmartConnectionFactory> connectionFactoryMock;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobControllerTest"/> class.
        /// </summary>
        public JobControllerTest()
        {
            this.loggerMock = new Mock<ILogger<JobController>>();
            this.dbFactoryMock = new Mock<IDbFactory>();
            this.connectionFactoryMock = new Mock<ISmartConnectionFactory>();

            this.Controller = new JobController(
                this.loggerMock.Object,
                this.dbFactoryMock.Object,
                this.connectionFactoryMock.Object);
        }

        /// <summary>
        /// Generates the test data for the GetJobStatus tests.
        /// </summary>
        /// <returns>An array of objects for the parameters of the test.</returns>
        public static IEnumerable<object[]> GetJobStatusData()
        {
            // Positive test
            yield return new object[]
            {
                "123",
                new List<JobSpecLite>
                {
                    new JobSpecLite { Uuid = "asdf", Status = "New" },
                    new JobSpecLite { Uuid = "123", Status = "Done" }
                }.AsQueryable(),
                200,
                new JobStatus { Status = "Done" }
            };

            // Negative test
            yield return new object[]
            {
                "123",
                new List<JobSpecLite>
                {
                }.AsQueryable(),
                404,
                null
            };
        }

        /// <summary>
        /// Generates the test data for the GetJobStatus tests.
        /// </summary>
        /// <returns>An array of objects for the parameters of the test.</returns>
        public static IEnumerable<object[]> GetJobDetailsData()
        {
            var testTimestamp = DateTime.UtcNow;
            var expected = new JobDetails
            {
                Uuid = "1",
                Status = "Done",
                Type = "FakeJob",
                StartTimestamp = testTimestamp.AddSeconds(-2),
                EndTimestamp = testTimestamp,
            };
            expected.Messages.Add("Test Message");
            expected.Messages.Add("Test Message 2");

            // Positive test
            yield return new object[]
            {
                "1",
                new List<JobSpec<dynamic>>
                {
                    new JobSpec<dynamic> { Uuid = "2", Status = "New" },
                    new JobSpec<dynamic>
                    {
                        Uuid = "1",
                        Status = "Done",
                        Type = "FakeJob",
                        StartTimestamp = testTimestamp.AddSeconds(-2),
                        EndTimestamp = testTimestamp,
                        Data = "We should never see this."
                    }
                }.AsQueryable(),
                new List<JobMessage>
                {
                    new JobMessage { JobUuid = "1", MasterJobUuid = "1", Level = (ushort)JobMessageLevel.Info, Message = "Test Message 2", Timestamp = testTimestamp },
                    new JobMessage { JobUuid = "1", MasterJobUuid = "1", Level = (ushort)JobMessageLevel.Info, Message = "Test Message", Timestamp = testTimestamp.AddSeconds(-1) }
                }.AsQueryable(),
                200,
                expected
            };

            yield return new object[]
            {
                "5",
                new List<JobSpec<dynamic>>
                {
                    new JobSpec<dynamic> { Uuid = "12", Status = "New" },
                    new JobSpec<dynamic>
                    {
                        Uuid = "5",
                        Status = "Done",
                        Type = "FakeJob",
                        StartTimestamp = testTimestamp.AddSeconds(-2),
                        EndTimestamp = testTimestamp,
                        Data = "We should never see this."
                    }
                }.AsQueryable(),
                new List<JobMessage>
                {
                }.AsQueryable(),
                200,
                new JobDetails
                {
                    Uuid = "5",
                    Status = "Done",
                    Type = "FakeJob",
                    StartTimestamp = testTimestamp.AddSeconds(-2),
                    EndTimestamp = testTimestamp,
                }
            };

            // Negative test
            yield return new object[]
            {
                "100",
                new List<JobSpec<dynamic>>
                {
                }.AsQueryable(),
                null,
                404,
                null
            };
        }

        /// <summary>
        /// Generates the test data for the create new job tests.
        /// </summary>
        /// <returns>An array of objects for the parameters of the test.</returns>
        public static IEnumerable<object[]> PostNewJobData()
        {
            // Negative test
            yield return new object[]
            {
                new EnqueueJob { JobType = "TestJob" },
                400
            };

            // Positive test
            yield return new object[]
            {
                new EnqueueJob { JobType = "ImportMapData" },
                201
            };
        }

        [Theory]
        [MemberData(nameof(GetJobStatusData))]
        public void CorrectJobStatus(string jobUuid, IQueryable<JobSpecLite> queryable, int expectedStatus, JobStatus expectedData)
        {
            // Arrange
            this.dbFactoryMock.Setup(x => x.GetCollectionAsQueryable<JobSpecLite>(CollectionNames.Jobs, null, null, null)).Returns(queryable);

            // Act
            var result = (ObjectResult)this.Controller.GetStatus(jobUuid);
            var data = (ApiResponse<JobStatus>)result.Value;

            // Assert
            Assert.Equal(expectedStatus, result.StatusCode);
            Assert.Equal<JobStatus>(expectedData, data.Result, new JobStatusEqualityComparer());
        }

        [Theory]
        [MemberData(nameof(GetJobDetailsData))]
        public void CorrectJobDetails(
            string jobUuid,
            IQueryable<JobSpec<dynamic>> jobsQueryable,
            IQueryable<JobMessage> messagesQueryable,
            int expectedStatus,
            JobDetails expectedData)
        {
            // Arrange
            this.dbFactoryMock.Setup(x => x.GetCollectionAsQueryable<JobSpec<dynamic>>(CollectionNames.Jobs, null, null, null)).Returns(jobsQueryable);
            this.dbFactoryMock.Setup(x => x.GetCollectionAsQueryable<JobMessage>(CollectionNames.JobMessages, null, null, null)).Returns(messagesQueryable);

            // Act
            var result = (ObjectResult)this.Controller.Get(jobUuid);
            var data = (ApiResponse<JobDetails>)result.Value;

            // Assert
            Assert.Equal(expectedStatus, result.StatusCode);
            Assert.Equal<JobDetails>(expectedData, data.Result, new JobDetailsEqualityComparer());
        }

        [Theory]
        [MemberData(nameof(PostNewJobData))]
        public void CorrectNewJob(EnqueueJob job, int expectedStatus)
        {
            // Arrange
            var mockCollection = new Mock<IMongoCollection<JobSpec<string>>>();

            this.dbFactoryMock.Setup(x => x.GetCollection<JobSpec<string>>(CollectionNames.Jobs, null, null)).Returns(mockCollection.Object);

            // Act
            var result = (ObjectResult)this.Controller.Post(job);
            var data = (ApiResponse<JobCreated>)result.Value;

            // Assert
            Assert.Equal(expectedStatus, result.StatusCode);
        }
    }
}