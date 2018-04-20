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
    /// Base Controller Test
    /// </summary>
    /// <typeparam name="T">The type that the controller is.</typeparam>
    public abstract class BaseControllerTest<T> : IDisposable
        where T : Controller
    {
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Gets or sets the controller being tested.
        /// </summary>
        /// <returns>The controller being tested</returns>
        protected T Controller { get; set; }

        /// <summary>
        /// Disposes of resources owned by this class
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of resources owned by this class
        /// </summary>
        /// <param name="disposing">True to clean up managed resources, False to clean up managed and unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.Controller?.Dispose();
                }

                this.disposedValue = true;
            }
        }
    }
}