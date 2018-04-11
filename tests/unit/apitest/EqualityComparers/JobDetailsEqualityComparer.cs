// Copyright (c) MadDonkeySoftware

namespace ApiTests.EqualityComparers
{
    using System.Collections.Generic;
    using System.Linq;

    using Api.Model.Response;

    internal class JobDetailsEqualityComparer : IEqualityComparer<JobDetails>
    {
        public bool Equals(JobDetails x, JobDetails y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null | y == null)
            {
                return false;
            }
            else if (x.EndTimestamp == y.EndTimestamp &&
                     x.StartTimestamp == y.StartTimestamp &&
                     x.Status == y.Status &&
                     x.Type == y.Type &&
                     x.Uuid == y.Uuid &&
                     MessagesValidated(x.Messages, y.Messages))
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(JobDetails obj)
        {
            return obj.GetHashCode();
        }

        private static bool MessagesValidated(List<string> x, List<string> y)
        {
            var limit = x.Count;
            if (limit != y.Count)
            {
                return false;
            }

            for (var i = 0; i < limit; i++)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}