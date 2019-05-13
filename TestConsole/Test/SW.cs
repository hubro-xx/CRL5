/**
* CRL 快速开发框架 V5
* Copyright (c) 2019 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL5
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    public class SW
    {
        public static long Do(Action act)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();

            act();

            sw.Stop();

            return sw.ElapsedMilliseconds;
        }

        public static T Do<T>(Func<T> fn)
        {
            T t;
            Stopwatch sw = new Stopwatch();

            sw.Start();

            t = fn();

            sw.Stop();

            Console.WriteLine(sw.ElapsedMilliseconds);

            return t;
        }
    }
}
