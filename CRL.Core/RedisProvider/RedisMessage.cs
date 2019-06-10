using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Core.RedisProvider
{
    /// <summary>
    /// 简单REDIS消息订阅
    /// 存储为list数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RedisMessage<T>
    {
        public virtual int Take
        {
            get
            {
                return 500;
            }
        }
        public virtual int SleepSecond
        {
            get
            {
                return 5;
            }
        }
        public static bool created = false;
        protected RedisClient client = new RedisClient();
        public RedisMessage()
        {
            var hashId = GetHashId();
            if (!created)
            {
                created = true;
                Console.WriteLine("RedisMessage" + typeof(T) + "启动订阅");
                //client.Subscribe<T>(OnSubscribe);
                new ThreadWork().Start(hashId, () =>
                {
                    var all = client.ListRange<T>(hashId, 0, Take - 1);
                    if (all.Count == 0)
                    {
                        return true;
                    }
                    var a = OnSubscribe(all);
                    if (a)
                    {
                        client.ListTrim(hashId, all.Count, -1);
                    }
                    if (rePublish.Count > 0)
                    {
                        Publish(rePublish);
                        rePublish.Clear();
                    }
                    return true;
                }, SleepSecond);
                created = true;
            }
        }
        string GetHashId()
        {
            return string.Format("RedisMessage_{0}",typeof(T).Name);
        }
        public virtual void Publish(T message)
        {
            var hashId = GetHashId();
            client.ListRightPush(hashId, message);
            //client.Pubblish(message);
        }
        public virtual void Publish(List<T> messages)
        {
            var hashId = GetHashId();
            foreach (var m in messages)
            {
                //client.Pubblish(m);
                client.ListRightPush(hashId, m);
            }

        }
        protected abstract bool OnSubscribe(List<T> message);
        List<T> rePublish = new List<T>();
        /// <summary>
        /// 重新加入队列
        /// </summary>
        /// <param name="message"></param>
        protected void RePublish(T message)
        {
            rePublish.Add(message);
        }
    }
}
