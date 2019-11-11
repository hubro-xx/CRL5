using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CRL.Core.Extension;
namespace ApiProxyTest
{
    class Program
    {

        public class tokenRequest
        {
            public string appId { get; set; }
            public string timestamp { get; set; } = DateTime.Now.ToString("timestamp");
            public string nonce { get; set; }
            public string signMethod { get; set; } = "SHA256";
            public string signature { get; set; }
        }
        public static string sha256(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }

            return builder.ToString();
        }
        static void Main(string[] args)
        {
            var clientConnect = new CRL.Core.ApiProxy.ApiClientConnect("https://api.weixin.qq.com");
            clientConnect.UseBeforRequest((request, members) =>
            {
                //如果需要设置发送头信息
                request.SetHead("token","test");
            });
            //clientConnect.UseXmlContentType();//如果使用XML格式提交解析
            //https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=APPID&secret=APPSECRET
            var client = clientConnect.GetClient<IToken>();
            //如果参数正确,返回token
            var result = client.token("grant_type", "appid", "secret");
            client.test(new Dictionary<string, object>() { { "key", 1 }, { "key2", 12 } });
            Console.WriteLine(result.ToJson());

            //https://api.weixin.qq.com/cgi-bin/user/info?access_token=ACCESS_TOKEN&openid=OPENID&lang=zh_CN

            var client2 = clientConnect.GetClient<IUser>();
            //如果参数正确,返回用户信息
            var result2 = client2.info(result.access_token, "openid");
            Console.WriteLine(result2.ToJson());

            Console.ReadLine();
        }
    }

    /// <summary>
    /// 微信获取token
    /// </summary>
    public interface IToken
    {
        [CRL.Core.ApiProxy.Method(Path = "cgi-bin/token", Method = CRL.Core.ApiProxy.HttpMethod.GET)]
        tokenResponse token(string grant_type, string appid, string secret);
        string test(Dictionary<string,object> args);
    }
    public class tokenResponse
    {
        public string errcode { get; set; }
        public string errmsg { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
    /// <summary>
    /// 微信获取用户信息
    /// </summary>
    public interface IUser
    {
        [CRL.Core.ApiProxy.Method(Path = "cgi-bin/user/info", Method = CRL.Core.ApiProxy.HttpMethod.GET)]
        userInfo info(string access_token, string openid, string lang = "zh_CN");
    }
    public class userInfo
    {
        public string errcode { get; set; }
        public string errmsg { get; set; }

        public string openid { get; set; }
        public string nickname { get; set; }
        public string headimgurl { get; set; }
    }
}
