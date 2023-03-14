using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Configuration
{
    public class StorageConfig
    {
        public const string SectionName = "Storage";

        public enum TypeEnum
        {
            OciObjectStorage,
            Smb,
        }

        public string Name { get; set; }

        public string Type { get; set; }

        public OciObjectStorageConfig OciObjectStorage { get; set; }
        public Smb Smb { get; set; }
    }

    public class OciObjectStorageConfig
    {
        public string ConfigFilePath { get; set; }
        public string BucketName { get; set; }
        public string NamespaceName { get; set; }
        public string RootPath { get; set; }

    }

    public class Smb
    {
        public string RootPath { get; set; }
    }
}
