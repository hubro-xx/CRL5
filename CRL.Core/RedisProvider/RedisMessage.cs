using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Core.RedisProvider
{
    /// <summary>
    /// 简单REDIS消息订阅
    /// 由于REDIS消息没有存储，不会订阅到发布之前的消息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RedisMessage<T>
    {

        public static bool created = false;
        protected RedisClient client = new RedisClient();
        public RedisMessage()
        {
            if (!created)
            {
                client.Subscribe<T>(OnSubscribe);
                created = true;
            }
        }
        public void Pubblish(T message)
        {
            client.Pubblish(message);
        }
        public void Pubblish(List<T> messages)
        {
            foreach (var m in messages)
            {
                client.Pubblish(m);
            }

        }
        protected abstract void OnSubscribe(T message);
    }
}
