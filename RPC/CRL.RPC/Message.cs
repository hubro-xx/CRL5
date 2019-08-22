using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRL.RPC
{
    abstract class MessageBase
    {
        public string MsgId
        {
            get;set;
        }
        public IByteBuffer ToBuffer()
        {
            //var data = this.ToByte();
            var data = Core.BinaryFormat.ClassFormat.Pack(GetType(), this);
            return Unpooled.WrappedBuffer(data);
        }
        public string Token
        {
            get;set;
        }
        internal static T FromBuffer<T>(IByteBuffer buffer)
        {
            //var data = buffer.ToString(Encoding.UTF8);
            //return data.ToObject<T>();
            var data = new byte[buffer.MaxCapacity];
            buffer.ReadBytes(data);
            return (T)Core.BinaryFormat.ClassFormat.UnPack(typeof(T), data);
        }
    }
    class RequestMessage : MessageBase
    {
        public string Service { get; set; }
        public string Method { get; set; }
        public Dictionary<string, byte[]> Args { get; set; }
        public static RequestMessage FromBuffer(IByteBuffer buffer)
        {
            return FromBuffer<RequestMessage>(buffer);
        }

    }
    class ResponseMessage : MessageBase
    {
        public bool Success { get; set; }
        public byte[] Data { get; set; }
        public static ResponseMessage CreateError(string msg, string code)
        {
            var response = new ResponseMessage() { Success = false, Msg = msg };
            response.SetData(typeof(string), code);
            return response;
        }
        public Dictionary<string, byte[]> Outs
        {
            get; set;
        }
        public object GetData(Type type)
        {
            int offSet = 0;
            return Core.BinaryFormat.FieldFormat.UnPack(type, Data,ref offSet);
            //return Data.ToObject(type);
        }
        public void SetData(Type type, object data)
        {
            Data = Core.BinaryFormat.FieldFormat.Pack(type, data);
            //Data = data.ToJson();
        }
        public string Msg { get; set; }
        public static ResponseMessage FromBuffer(IByteBuffer buffer)
        {
            return FromBuffer<ResponseMessage>(buffer);
        }
    }
}
