
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Text;

namespace CRL.RPC
{
    class ServerHandler : ChannelHandlerAdapter
    {
        public ServerHandler(RPCServer _server)
        {
            server = _server;
        }
        RPCServer server { get; }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = message as IByteBuffer;
            var request = RequestMessage.FromBuffer(buffer);
            ResponseMessage response = server.InvokeResult(request);
            response.MsgId = request.MsgId;
            context.WriteAndFlushAsync(response.ToBuffer());
            //context.CloseAsync();
        }
        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("ExceptionCaught: " + exception);
            context.CloseAsync();
        }
    }
}