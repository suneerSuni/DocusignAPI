using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DocuSignAPI.Model
{
    public class Base64WordDocs
    {
        public List<Base64WordDocument> base64WordDocs;
        public List<FileDetail> fileDetails;
    }
    [DataContract]
    public class Base64WordDocument
    {
        [DataMember]
        public string base64WordDoc { get; set; }
        [DataMember]
        public string docuName { get; set; }
        [DataMember]
        public int docuId { get; set; }
        [DataMember]
        public bool docuVisible { get; set; }
        [DataMember]
        public FileDetail File { get; set; }
    }
    [DataContract]
    public class FileDetail
    {
        [DataMember]
        public string path { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string base64WordDoc { get; set; }
    }
}