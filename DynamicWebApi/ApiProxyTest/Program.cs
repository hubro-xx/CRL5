using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Core.Extension;
namespace ApiProxyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var clientConnect = new CRL.Core.ApiProxy.ApiClientConnect("https://api.weixin.qq.com").UseApiprefix("");
            clientConnect.UseBeforRequest((request, members) =>
            {
                //如果需要设置发送头信息
                request.SetHead("token","test");
            });
            //clientConnect.UseXmlContentType();//如果使用XML格式提交解析
            //https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=APPID&secret=APPSECRET
            var client = clientConnect.GetClient<weixinToken>();
            //如果参数正确,返回token
            var result = client.token("grant_type", "appid", "secret");
            Console.WriteLine(result.ToJson());

            //https://api.weixin.qq.com/cgi-bin/user/info?access_token=ACCESS_TOKEN&openid=OPENID&lang=zh_CN

            var client2 = clientConnect.GetClient<user>();
            //如果参数正确,返回用户信息
            var result2 = client2.info(result.access_token, "openid");
            Console.WriteLine(result2.ToJson());

            Console.ReadLine();
        }
    }
    /// <summary>
    /// 微信获取token
    /// </summary>
    [CRL.Core.ApiProxy.Control(Name = "cgi-bin")]
    public interface weixinToken
    {
        [CRL.Core.ApiProxy.HttpGet]
        tokenResponse token(string grant_type, string appid, string secret);
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
    [CRL.Core.ApiProxy.Control(Name = "cgi-bin/user")]
    public interface user
    {
        [CRL.Core.ApiProxy.HttpGet]
        userInfo info(string access_token,string openid,string lang= "zh_CN");
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
