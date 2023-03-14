using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.Com.ParallelOptions
{
    public class ComParallelOptions : System.Threading.Tasks.ParallelOptions
    {
        private static int maxDegreeOfParallelism => GetMaxDegreeOfParallelism();

        private static int GetMaxDegreeOfParallelism()
            => UnityCore.ResolveOrDefault<int>("AppConfig:MaxDegreeOfParallelism", 3);

        public ComParallelOptions()
        {
            this.MaxDegreeOfParallelism = maxDegreeOfParallelism;
        }
    }
}
