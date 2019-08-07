using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Core.BinaryFormat
{
    class DicFormat
    {
        public static byte[] Pack(object param)
        {
            var list = (System.Collections.IDictionary)param;
            var body = new List<byte>();
            var type = param.GetType();
            var innerType = type.GenericTypeArguments[0];
            var innerType2 = type.GenericTypeArguments[1];
            foreach (var key in list.Keys)
            {
                var obj = list[key];
                var keyData = FieldFormat.Pack(innerType, key);
                var valueData = FieldFormat.Pack(innerType2,obj);
                body.AddRange(keyData);
                body.AddRange(valueData);
            }
            return body.ToArray();
        }
        public static object UnPack(Type type, byte[] datas)
        {
            var dic = (System.Collections.IDictionary)System.Activator.CreateInstance(type);
            var innerType = type.GenericTypeArguments[0];
            var innerType2 = type.GenericTypeArguments[1];
            int dataIndex = 0;
            while (dataIndex < datas.Length)
            {
                var key = FieldFormat.UnPack(innerType, datas, ref dataIndex);
                var value = FieldFormat.UnPack(innerType2, datas, ref dataIndex);
                if (key == null)
                {
                    continue;
                }
                dic.Add(key,value);
            }
            return dic;
        }
    }
}

