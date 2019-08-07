using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Core.BinaryFormat
{
    class ListFormat
    {
        public static byte[] Pack(object param)
        {
            var list = (System.Collections.IEnumerable)param;
            var body = new List<byte>();
            var type = param.GetType();
            var innerType = type.GenericTypeArguments[0];
            foreach (var obj in list)
            {
                var data = FieldFormat.Pack(innerType,obj);
                body.AddRange(data);
            }
            return body.ToArray();
        }
        public static object UnPack(Type type, byte[] datas)
        {
            var obj = System.Activator.CreateInstance(type);
            var method = type.GetMethod("Add");
            var innerType = type.GenericTypeArguments[0];
            int dataIndex = 0;
            while (dataIndex < datas.Length)
            {
                var value = FieldFormat.UnPack(innerType, datas,ref dataIndex);
                method.Invoke(obj,new object[] { value});
            }
            return obj;
        }
    }
}
