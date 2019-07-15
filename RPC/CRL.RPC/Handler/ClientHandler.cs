using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Text;

namespace CRL.RPC
{
    class ClientHandler : ChannelHandlerAdapter
    {
        ResponseWaits waits { get; }
        public ClientHandler(ResponseWaits _waits)
        {
            waits = _waits;
        }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = message as IByteBuffer;
            waits.Set(context.Channel.Id.AsShortText(), buffer);
        }
        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("ExceptionCaught: " + exception);
            context.CloseAsync();
        }
    }
}