// Copyright (c) MadDonkeySoftware

namespace ApiTests.EqualityComparers
{
    using System.Collections.Generic;

    using Api.Model.Response;

    internal class JobStatusEqualityComparer : IEqualityComparer<JobStatus>
    {
        public bool Equals(JobStatus x, JobStatus y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null | y == null)
            {
                return false;
            }
            else if (x.Status == y.Status)
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(JobStatus obj)
        {
            return obj.GetHashCode();
        }
    }
}