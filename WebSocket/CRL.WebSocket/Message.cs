
using CRL.Core.Extension;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace CRL.WebSocket
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
        public string MsgId
        {
            get; set;
        }
        public string Service { get; set; }
        public string Method { get; set; }
        /// <summary>
        /// 按索引
        /// </summary>
        public List<object> Args { get; set; }
        public static RequestMessage FromBuffer(string buffer)
        {
            return buffer.ToObject<RequestMessage>();
        }

    }
    class ResponseMessage : MessageBase
    {
        public bool Success { get; set; }
        public string Data { get; set; }
        public static ResponseMessage CreateError(string msg, string code)
        {
            return new ResponseMessage() { Success = false, Msg = msg, Data = code };
        }
        public object GetData(Type type)
        {
            return Data.ToObject(type);
        }
        public Dictionary<int,object> Outs
        {
            get; set;
        }
        public void SetData(object data)
        {
            Data = data.ToJson();
        }
        public string Msg { get; set; }
        public string MsgId { get;  set; }

        public static ResponseMessage FromBuffer(string buffer)
        {
            return buffer.ToObject<ResponseMessage>();
        }
    }
}
