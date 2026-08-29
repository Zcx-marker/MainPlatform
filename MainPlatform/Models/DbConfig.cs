using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MainPlatform.Models
{
    [DataContract]
    public class DbConfig
    {
        [DataMember]
        public string DbType { get; set; } 
        [DataMember]
        public string Server { get; set; } 
        [DataMember]
        public string Database { get; set; } 
        [DataMember]
        public string DataBasebConnectionString { get; set; } 
    }
}
