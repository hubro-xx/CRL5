using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Reflection;
using System.Collections.Specialized;

namespace CRL.Core.Request
{
    public class RequestHelper
    {
        /// <summary>
        /// 是否为远程服务器,DEBUG用
        /// </summary>
        public static bool IsRemote
        {
            get
            {
                string address = GetServerIp();
                bool a = !address.Contains("192.168.") && !address.Contains("127.0.") && !address.Contains("10.0.");
                return a;
            }
        }
        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP()
        {
            string result = String.Empty;

            result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }

            if (null == result || result == String.Empty || !StringHelper.IsIP(result))
            {
                return "0.0.0.0";
            }

            return result;

        }

        /// <summary>
        /// 判断IP地址是否为内网IP地址
        /// </summary>
        /// <param name="ipAddress">IP地址字符串</param>
        /// <returns></returns>
        public static bool IsInnerIP(String ipAddress)
        {
            bool isInnerIp = false;
            long ipNum = GetIpNum(ipAddress);
            /**
私有IP：A类 10.0.0.0-10.255.255.255
B类 172.16.0.0-172.31.255.255
C类 192.168.0.0-192.168.255.255
当然，还有127这个网段是环回地址 
            **/
            long aBegin = GetIpNum("10.0.0.0");
            long aEnd = GetIpNum("10.255.255.255");
            long bBegin = GetIpNum("172.16.0.0");
            long bEnd = GetIpNum("172.31.255.255");
            long cBegin = GetIpNum("192.168.0.0");
            long cEnd = GetIpNum("192.168.255.255");
            isInnerIp = IsInner(ipNum, aBegin, aEnd) || IsInner(ipNum, bBegin, bEnd) || IsInner(ipNum, cBegin, cEnd) || ipAddress.Equals("127.0.0.1");
            return isInnerIp;
        }
        /// <summary>
        /// 把IP地址转换为Long型数字
        /// </summary>
        /// <param name="ipAddress">IP地址字符串</param>
        /// <returns></returns>
        private static long GetIpNum(String ipAddress)
        {
            String[] ip = ipAddress.Split('.');
            long a = int.Parse(ip[0]);
            long b = int.Parse(ip[1]);
            long c = int.Parse(ip[2]);
            long d = int.Parse(ip[3]);

            long ipNum = a * 256 * 256 * 256 + b * 256 * 256 + c * 256 + d;
            return ipNum;
        }
        /// <summary>
        /// 判断用户IP地址转换为Long型后是否在内网IP地址所在范围
        /// </summary>
        /// <param name="userIp"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static bool IsInner(long userIp, long begin, long end)
        {
            return (userIp >= begin) && (userIp <= end);
        }

        static string serverIp;
		/// <summary>
		/// 获取服务器第一个IP
		/// </summary>
		/// <returns></returns>
		public static string GetServerIp()
		{
            if (string.IsNullOrEmpty(serverIp))
            {
                System.Net.IPAddress[] addressList = Dns.GetHostAddresses(Dns.GetHostName());

                string address = "";
                foreach (System.Net.IPAddress a in addressList)
                {
                    if (a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !a.ToString().Contains("10.0.0"))
                    {
                        if (!IsInnerIP(a.ToString()))
                        {
                            address = a.ToString();
                            break;
                        }
                    }
                }
                serverIp = address;
            }
            return serverIp;
		}
        static string innerIP;
        public static string GetInnerIP()
        {
            if (string.IsNullOrEmpty(innerIP))
            {
                System.Net.IPAddress[] addressList = Dns.GetHostAddresses(Dns.GetHostName());

                string address = "";
                foreach (System.Net.IPAddress a in addressList)
                {
                    if (a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        if (IsInnerIP(a.ToString()))
                        {
                            address = a.ToString();
                            break;
                        }
                    }
                }
                innerIP = address;
            }
            return innerIP;
        }
        static object lockObj = new object();
        static int index = 1;

        /// <summary>
        /// 返回当前http主机名
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentHost()
        {
            string url = HttpContext.Current.Request.Url.ToString();
            return GetCurrentHost(url);
        }
        /// <summary>
        /// 返回当前http主机名
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentHost(string url)
        {
            string[] arry = url.Split('/');
            string host = arry[2];
            string url1 = arry[0] + "//" + host;
            return url1;
        }
        #region 签名机制
        static string timeFormat = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// 生成签名参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parmes"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string GetParameUrl(string key, SortedDictionary<string, string> parmes, Encoding encoding)
        {
            string par = "";
            DateTime time = DateTime.Now;
            parmes.Add("time", time.ToString(timeFormat));

            par = GetParame(parmes, true, encoding);
            string par1 = GetParame(parmes, false, encoding);
            string sign =StringHelper.EncryptMD5(par1 + key);
            par += "&sign=" + sign;
            return par;
        }
        static string GetParame(SortedDictionary<string, string> parmes, bool urlEncode, Encoding encoding)
        {
            string par = "";
            foreach (KeyValuePair<string, string> entry in parmes)
            {
                if (entry.Key != "sign")
                {
                    if (urlEncode)
                    {
                        par += entry.Key.ToLower() + "=" + HttpUtility.UrlEncode(entry.Value, encoding) + "&";
                    }
                    else
                    {
                        par += entry.Key.ToLower() + "=" + entry.Value + "&";
                    }
                }
            }
            par = par.Substring(0, par.Length - 1);
            return par;
        }
        /// <summary>
        /// 验证数据
        /// </summary>
        public static bool VerifyData(string key, SortedDictionary<string, string> parmes, out string msg, Encoding encoding)
        {
            msg = "";
            string time = parmes["time"];
            string sign = parmes["sign"];
            TimeSpan ts = DateTime.Now - Convert.ToDateTime(time);
            if (ts.TotalMinutes > 10)
            {
                msg = "签名超时";
                return false;
            }
            string par = GetParame(parmes, false, encoding);
            string sign1 =StringHelper.EncryptMD5(par + key);
            if (sign != sign1)
            {
                //CRL.Core.EventLog.Log("签名为:" + par,false);
                msg = "签名验证失败";
                return false;
            }
            return true;
        }
        /// <summary>
        /// 从集合中获取指定的参数
        /// </summary>
        /// <param name="cc"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static SortedDictionary<string, string> GetRequestParame(NameValueCollection cc, string pars)
        {
            SortedDictionary<string, string> list = new SortedDictionary<string, string>();
            string[] arry = pars.Split(',');
            foreach (string s in arry)
            {
                if (s.Trim() != "")
                {
                    list.Add(s, cc[s]);
                }
            }
            list.Add("time", cc["time"]);
            list.Add("sign", cc["sign"]);
            return list;
        }
        #endregion

        /// <summary>
        /// 获取工作目录路径,IIS则为网站根目录,程序为程序所在目录
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetFilePath(string file)
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath(file);
            if (string.IsNullOrEmpty(path))
            {
                path = AppDomain.CurrentDomain.BaseDirectory + file.Replace("/",@"\");
            }
            return path;
        }
    }
}
