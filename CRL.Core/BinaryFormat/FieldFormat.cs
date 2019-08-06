using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Core.BinaryFormat
{
    class FieldFormat
    {
        /// <summary>
        /// 4字节长度+body
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static byte[] Pack(object param)
        {
            List<byte> datas = new List<byte>();

            var len = 0;

            byte[] data = null;

            if (param == null)
            {
                len = 0;
            }
            else
            {
                if (param is string)
                {
                    data = Encoding.UTF8.GetBytes((string)param);
                }
                else if (param is byte)
                {
                    data = new byte[] { (byte)param };
                }
                else if (param is bool)
                {
                    data = BitConverter.GetBytes((bool)param);
                }
                else if (param is short)
                {
                    data = BitConverter.GetBytes((short)param);
                }
                else if (param is int)
                {
                    data = BitConverter.GetBytes((int)param);
                }
                else if (param is long)
                {
                    data = BitConverter.GetBytes((long)param);
                }
                else if (param is float)
                {
                    data = BitConverter.GetBytes((float)param);
                }
                else if (param is double)
                {
                    data = BitConverter.GetBytes((double)param);
                }
                else if (param is DateTime)
                {
                    var str = ((DateTime)param).Ticks.ToString();
                    data = Encoding.UTF8.GetBytes(str);
                }
                else if (param is Enum)
                {
                    var enumValType = Enum.GetUnderlyingType(param.GetType());

                    if (enumValType == typeof(byte))
                    {
                        data = new byte[] { (byte)param };
                    }
                    else if (enumValType == typeof(short))
                    {
                        data = BitConverter.GetBytes((Int16)param);
                    }
                    else if (enumValType == typeof(int))
                    {
                        data = BitConverter.GetBytes((Int32)param);
                    }
                    else
                    {
                        data = BitConverter.GetBytes((Int64)param);
                    }
                }
                else if (param is byte[])
                {
                    data = (byte[])param;
                }
                else
                {
                    var type = param.GetType();


                    if (type.IsGenericType || type.IsArray)
                    {
                        if (type.Name.Contains("Dictionary"))
                            data = DicFormat.Pack((System.Collections.IDictionary)param);
                        else if (type.Name.Contains("List`1") || type.IsArray)
                            data = ListFormat.Pack((System.Collections.IEnumerable)param);
                        else
                            data = ClassFormat.Pack( type, param);
                    }
                    else if (type.IsClass)
                    {
                        data = ClassFormat.Pack(type, param);
                    }

                }
                if (data != null)
                    len = data.Length;
            }
            //Console.WriteLine("add:"+ len);
            var lenData = BitConverter.GetBytes(len).Take(lenSaveLength);
            datas.AddRange(lenData);
            if (len > 0)
            {
                datas.AddRange(data);
            }
            return datas.Count == 0 ? null : datas.ToArray();
        }
        static int lenSaveLength = 2;
        public static object UnPack(Type type, byte[] datas, ref int offset)
        {
            dynamic obj = null;
            var len = 0;
            var lenData = new byte[4];

            Buffer.BlockCopy(datas, offset, lenData, 0, lenSaveLength);

            len = BitConverter.ToInt32(lenData, 0);
            offset += lenSaveLength;
            if (len > 0)
            {
                byte[] data = new byte[len];
                Buffer.BlockCopy(datas, offset, data, 0, len);
                offset += len;

                if (type == typeof(string))
                {
                    obj = Encoding.UTF8.GetString(data);
                }
                else if (type == typeof(byte))
                {
                    obj = (data);
                }
                else if (type == typeof(bool))
                {
                    obj = (BitConverter.ToBoolean(data, 0));
                }
                else if (type == typeof(short))
                {
                    obj = (BitConverter.ToInt16(data, 0));
                }
                else if (type == typeof(int))
                {
                    obj = (BitConverter.ToInt32(data, 0));
                }
                else if (type == typeof(long))
                {
                    obj = (BitConverter.ToInt64(data, 0));
                }
                else if (type == typeof(float))
                {
                    obj = (BitConverter.ToSingle(data, 0));
                }
                else if (type == typeof(double))
                {
                    obj = (BitConverter.ToDouble(data, 0));
                }
                else if (type == typeof(decimal))
                {
                    obj = (BitConverter.ToDouble(data, 0));
                }
                else if (type == typeof(DateTime))
                {
                    var dstr = Encoding.UTF8.GetString(data);
                    var ticks = long.Parse(dstr);
                    obj = (new DateTime(ticks));
                }
                else if (type.BaseType == typeof(Enum))
                {
                    var numType = Enum.GetUnderlyingType(type);

                    if (numType == typeof(byte))
                    {
                        obj = Enum.ToObject(type, data[0]);
                    }
                    else if (numType == typeof(short))
                    {
                        obj = Enum.ToObject(type, BitConverter.ToInt16(data, 0));
                    }
                    else if (numType == typeof(int))
                    {
                        obj = Enum.ToObject(type, BitConverter.ToInt32(data, 0));
                    }
                    else
                    {
                        obj = Enum.ToObject(type, BitConverter.ToInt64(data, 0));
                    }
                }
                else if (type == typeof(byte[]))
                {
                    obj = (byte[])data;
                }
                else if (type.IsGenericType)
                {
                    if (type.Name.Contains("Dictionary"))
                    {
                        obj = DicFormat.UnPack(type, data);
                    }
                    else if (type.Name.Contains("List`1") || type.IsArray)
                    {
                        obj = ListFormat.UnPack(type, data);
                    }
                    else
                    {
                        obj = ClassFormat.UnPack(type, data);
                    }
                }
                else if (type.IsClass)
                {
                    obj = ClassFormat.UnPack(type, data);
                }
                else if (type.IsArray)
                {
                    obj = ListFormat.UnPack(type, data);
                }
                else
                {
                    throw new Exception("ParamsSerializeUtil.Deserialize 未定义的类型：" + type.ToString());
                }

            }
            return obj;
        }
    }
}
