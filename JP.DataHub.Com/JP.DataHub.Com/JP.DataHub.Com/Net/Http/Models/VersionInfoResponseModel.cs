using System;
using System.Collections.Generic;

namespace JP.DataHub.Com.Net.Http.Models
{
    public class VersionInfoResponseModel
    {
        public int currentversion { get; set; }
        public List<DocumentVersion> documentversions { get; set; }
        public string _Upduser_Id { get; set; }
        public DateTime _Upddate { get; set; }
    }

    public class DocumentVersion
    {
        public int version { get; set; }
        public bool is_current { get; set; }
        public string _Reguser_Id { get; set; }
        public DateTime _Regdate { get; set; }
    }
}
