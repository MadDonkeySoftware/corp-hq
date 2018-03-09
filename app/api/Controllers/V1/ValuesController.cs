// Copyright (c) MadDonkeySoftware

namespace Api.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Api.Data;
    using Api.Model;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MongoDB.Bson;
    using MongoDB.Driver;

    /// <summary>
    /// This class will be deleted
    /// </summary>
    [V1Route]
    public class ValuesController : Controller
    {
        private readonly ILogger logger;
        private readonly IDbFactory dbFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValuesController"/> class.
        /// </summary>
        /// <param name="logger">The logger in which to use.</param>
        /// <param name="dbFactory">The factory for DB connections.</param>
        public ValuesController(ILogger<ValuesController> logger, IDbFactory dbFactory)
        {
            this.logger = logger;
            this.dbFactory = dbFactory;
        }

        /// <summary>
        /// GET api/values
        /// </summary>
        /// <returns>A list of values</returns>
        [HttpGet]
        public IEnumerable<dynamic> Get()
        {
            this.logger.LogDebug(1000, "Returning list of values");

            // TODO: This is all throw away code. It just demonstrates the link between the code and mongodb.
            var col = this.dbFactory.GetCollection<Sample>("corp-hq", "values");

            if (col.Count(new BsonDocument()) == 0)
            {
                this.SeedValueData(col);
            }

            var data = new List<dynamic>();
            var documents = col.Find(x => true);
            foreach (var document in documents.ToList())
            {
                data.Add(new
                {
                    key = document.Key,
                    Value = document.Value
                });
            }

            return data;
        }

        /// <summary>
        /// GET api/values/5
        /// </summary>
        /// <param name="key">An arbitrary key.</param>
        /// <returns>A single value</returns>
        [HttpGet("{key}")]
        public string Get(string key)
        {
            var col = this.dbFactory.GetCollection<Sample>("corp-hq", "values");
            this.logger.LogDebug(1000, "Returning specific value");
            var cultureInfo = System.Globalization.CultureInfo.InvariantCulture;
            return string.Format(cultureInfo, "Value: {0}", key);
        }

        /// <summary>
        /// POST api/values
        /// </summary>
        /// <param name="value">An arbitrary value.</param>
        [HttpPost]
        public void Post([FromBody]Sample value)
        {
            this.logger.LogDebug(1001, "Adding to list of values");
            var col = this.dbFactory.GetCollection<Sample>("corp-hq", "values");
            col.InsertOne(new Sample
            {
                Key = Guid.NewGuid().ToString(),
                Value = value.Value
            });
        }

        /// <summary>
        /// PUT api/values/5
        /// </summary>
        /// <param name="id">An arbitrary id.</param>
        /// <param name="value">An arbitrary value.</param>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
            var cultureInfo = System.Globalization.CultureInfo.InvariantCulture;
            this.logger.LogDebug(1000, string.Format(cultureInfo, "Put called with id: {0}, {1}", id, value));
        }

        /// <summary>
        /// DELETE api/values/5
        /// </summary>
        /// <param name="id">An arbitrary id.</param>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var cultureInfo = System.Globalization.CultureInfo.InvariantCulture;
            this.logger.LogDebug(1000, "Delete called with id: " + id);
        }

        private void SeedValueData(IMongoCollection<Sample> col)
        {
            var items = new List<Sample>();
            for (var i = 1; i < 3; i++)
            {
                items.Add(new Sample
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = "value" + i
                });
            }

            col.InsertMany(items);
        }
    }
}