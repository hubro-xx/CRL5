
using System;
using System.Collections.Generic;
using System.Text;

namespace CRL.DynamicWebApi
{
    abstract class MessageBase
    {
        public string ToBuffer()
        {
            return this.ToJson();
        }
        public string Token
        {
            get;set;
        }
    }
    class RequestMessage : MessageBase
    {
        public string Service { get; set; }
        public string Method { get; set; }
        public Dictionary<string, object> Args { get; set; }
        public static RequestMessage FromBuffer(string buffer)
        {
            return buffer.ToObject<RequestMessage>();
        }

    }
    class ResponseMessage : MessageBase
    {
        public bool Success { get; set; }
        public string Data { get; set; }
        public object GetData(Type type)
        {
            return Data.ToObject(type);
        }
        public Dictionary<string, object> Outs
        {
            get; set;
        }
        public void SetData(object data)
        {
            Data = data.ToJson();
        }
        public string Msg { get; set; }
        public static ResponseMessage FromBuffer(string buffer)
        {
            return buffer.ToObject<ResponseMessage>();
        }
    }
}
