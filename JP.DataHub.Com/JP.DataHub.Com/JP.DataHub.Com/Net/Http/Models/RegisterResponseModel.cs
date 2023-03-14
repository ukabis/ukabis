using System;
using System.Linq;

namespace JP.DataHub.Com.Net.Http.Models
{
    public class RegisterResponseModel
    {
        public string RepositoryKey => id?.Split('~')?.LastOrDefault();
        public int Version => int.TryParse(id?.Split('~')?.Reverse().Skip(1).FirstOrDefault(), out var version) ? version : 1;

        public string id { get; set; }
    }
}
