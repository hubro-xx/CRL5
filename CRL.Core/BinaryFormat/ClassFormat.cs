using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Core.BinaryFormat
{
    public class ClassFormat
    {
        public static byte[] Pack(Type type, object obj)
        {
            //var pro = type.GetProperties().Where(b => b.GetSetMethod() != null).OrderBy(b => b.Name);
            var key = $"BinaryFormatTypePro_{type}";
            var pro = CRL.Core.DelegateCache.Init(key, 9999, () =>
            {
                return type.GetProperties().Where(b => b.GetSetMethod() != null).OrderBy(b => b.Name);
            });
            //var body = new List<byte>();
            var arry = new List<byte[]>();
            var len = 0;
            foreach (var p in pro)
            {
                var d = FieldFormat.Pack(p.PropertyType, p.GetValue(obj));
                //body.AddRange(d);
                arry.Add(d);
                len += d.Length;
            }
            //return body.ToArray();
            return arry.JoinData(len);
        }
        public static object UnPack(Type type, byte[] datas)
        {
            var obj = System.Activator.CreateInstance(type);
            var key = $"BinaryFormatTypePro_{type}";
            var pro = CRL.Core.DelegateCache.Init(key, 9999, () =>
            {
                return type.GetProperties().Where(b => b.GetSetMethod() != null).OrderBy(b => b.Name);
            });

            int dataIndex = 0;
            foreach (var p in pro)
            {
                var value = FieldFormat.UnPack(p.PropertyType, datas, ref dataIndex);
                p.SetValue(obj, value);
            }
            return obj;
        }
    }
}
