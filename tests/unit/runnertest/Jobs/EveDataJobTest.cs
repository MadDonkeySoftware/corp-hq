// Copyright (c) MadDonkeySoftware

namespace RunnerTests.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using System.Linq;

    using Common.Data;
    using Common.Model;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using Moq;
    using Newtonsoft.Json;
    using RabbitMQ.Client;
    using Runner.Data;
    using Runner.Jobs;
    using Xunit;

    /// <summary>
    /// Tests for Job controller actions.
    /// </summary>
    public class EveDataJobTest
    {
        private readonly string fakeJobUuid;
        private readonly Mock<IJobRepository> jobRepositoryMock;
        private readonly Mock<ISettingRepository> settingRepositoryMock;

        /// <summary>
        /// Initializes a new instance of the <see cref="EveDataJobTest"/> class.
        /// </summary>
        public EveDataJobTest()
        {
            this.fakeJobUuid = string.Empty;
            this.jobRepositoryMock = new Mock<IJobRepository>();
            this.settingRepositoryMock = new Mock<ISettingRepository>();

            this.Job = new JobWrapper(
                this.jobRepositoryMock.Object,
                this.settingRepositoryMock.Object);
        }

        internal JobWrapper Job { get; set; }

        /// <summary>
        /// Generates the test data for the create new job tests.
        /// </summary>
        /// <returns>An array of objects for the parameters of the test.</returns>
        public static IEnumerable<object[]> CreateEndpointData()
        {
            yield return new object[] { "http://base/middle", "part1/part2", "http://base/middle/part1/part2" };
            yield return new object[] { "http://base/middle/", "part1/part2", "http://base/middle/part1/part2" };
            yield return new object[] { "http://base/middle", "/part1/part2", "http://base/middle/part1/part2" };
            yield return new object[] { "http://base/middle/", "/part1/part2", "http://base/middle/part1/part2" };
        }

        [Theory]
        [MemberData(nameof(CreateEndpointData))]
        public void CorrectCreateEndpoint(string eveBase, string part, string expected)
        {
            // Arrange
            this.settingRepositoryMock.Setup(x => x.FetchSetting<string>("eveDataUri")).Returns(eveBase);

            // Act
            var result = (Uri)this.Job.CreateEndpoint(part);

            // Assert
            Assert.Equal(expected, result.AbsoluteUri);
        }

        internal class JobWrapper : EveDataJob
        {
            public JobWrapper(IJobRepository jobRepository, ISettingRepository settingsRepository)
                : base(jobRepository, settingsRepository)
            {
            }

            internal new Uri CreateEndpoint(string part)
            {
                return base.CreateEndpoint(part);
            }

            protected override void Work()
            {
                throw new NotImplementedException();
            }
        }
    }
}