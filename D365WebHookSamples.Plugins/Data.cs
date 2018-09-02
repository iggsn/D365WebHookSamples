using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk;

namespace D365WebHookSamples.Plugins
{
    [DataContract(Name = "Data", Namespace = "https://iggsn.com/schemas/", IsReference = false)]
    [KnownType(typeof(Data))]
    public class Data
    {
        [DataMember]
        public Entity Contact { get; set; }

        [DataMember]
        public bool Success { get; set; }
    }
}
