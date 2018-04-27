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
    using Runner.Jobs;
    using Xunit;

    /// <summary>
    /// Tests for Job controller actions.
    /// </summary>
    public class EveDataJobTest
    {
        private readonly string fakeJobUuid;
        private readonly Mock<IDbFactory> dbFactoryMock;

        /// <summary>
        /// Initializes a new instance of the <see cref="EveDataJobTest"/> class.
        /// </summary>
        public EveDataJobTest()
        {
            this.fakeJobUuid = string.Empty;
            this.dbFactoryMock = new Mock<IDbFactory>();

            this.Job = new JobWrapper(
                this.fakeJobUuid,
                this.dbFactoryMock.Object);
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
            var settings = new List<Common.Model.Setting<string>>
            {
                new Common.Model.Setting<string>
                {
                    Key = "eveDataUri",
                    Value = eveBase
                }
            };

            this.dbFactoryMock.Setup(x => x.GetCollectionAsQueryable<Common.Model.Setting<string>>("corp-hq", CollectionNames.Settings, null, null)).Returns(settings.AsQueryable());

            // Act
            var result = (Uri)this.Job.CreateEndpoint(part);

            // Assert
            Assert.Equal(expected, result.AbsoluteUri);
        }

        internal class JobWrapper : EveDataJob
        {
            public JobWrapper(string jobUuid, IDbFactory dbFactory)
                : base(jobUuid, dbFactory)
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