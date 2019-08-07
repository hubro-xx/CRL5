using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Core.BinaryFormat
{
    public class ClassFormat
    {
        public static byte[] Pack(Type type, object obj)
        {
            var pro = type.GetProperties().Where(b => b.GetSetMethod() != null).OrderBy(b => b.Name);
            var body = new List<byte>();
            foreach (var p in pro)
            {
                var d = FieldFormat.Pack(p.PropertyType, p.GetValue(obj));
                body.AddRange(d);
            }
            return body.ToArray();
        }
        public static object UnPack(Type type, byte[] datas)
        {
            var obj = System.Activator.CreateInstance(type);
            var pro = type.GetProperties().Where(b => b.GetSetMethod() != null).OrderBy(b => b.Name);
            int dataIndex =0;
            foreach (var p in pro)
            {
                var value = FieldFormat.UnPack(p.PropertyType, datas, ref dataIndex);
                p.SetValue(obj, value);
            }
            return obj;
        }
    }
}
