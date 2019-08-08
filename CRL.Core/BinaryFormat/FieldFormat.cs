using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Core.BinaryFormat
{
    public class FieldFormat
    {
        delegate byte[] toByte(Type type, object obj);
        delegate object fromByte(Type type, byte[] data);
        static Dictionary<Type, Tuple<toByte, fromByte>> methods = new Dictionary<Type, Tuple<toByte, fromByte>>();
        static FieldFormat()
        {
            if (methods.Count != 0)
            {
                return;
            }
            #region methods
            methods.Add(typeof(decimal), new Tuple<toByte, fromByte>((type, param) =>
            {
                Int32[] bits = Decimal.GetBits((decimal)param);
                Byte[] bytes = new Byte[bits.Length * 4];
                for (Int32 i = 0; i < bits.Length; i++)
                {
                    for (Int32 j = 0; j < 4; j++)
                    {
                        bytes[i * 4 + j] = (Byte)(bits[i] >> (j * 8));
                    }
                }
                return bytes;
            }, (type, data) =>
            {
                Int32[] bits = new Int32[data.Length / 4];
                for (Int32 i = 0; i < bits.Length; i++)
                {
                    for (Int32 j = 0; j < 4; j++)
                    {
                        bits[i] |= data[i * 4 + j] << j * 8;
                    }
                }
                return new Decimal(bits);
            }));

            methods.Add(typeof(string),new Tuple<toByte, fromByte>((type,param) =>
            {
                return Encoding.UTF8.GetBytes((string)param);
            },(type,data)=>
            {
                return Encoding.UTF8.GetString(data);
            }));
            methods.Add(typeof(byte), new Tuple<toByte, fromByte>((type, param) =>
            {
                return new byte[] { (byte)param };
            }, (type, data) =>
            {
                return data;
            }));
            methods.Add(typeof(bool), new Tuple<toByte, fromByte>((type, param) =>
            {
                return BitConverter.GetBytes((bool)param);
            }, (type, data) =>
            {
                return BitConverter.ToBoolean(data,0); 
            }));
            methods.Add(typeof(short), new Tuple<toByte, fromByte>((type, param) =>
            {
                return BitConverter.GetBytes((short)param);
            }, (type, data) =>
            {
                return BitConverter.ToInt16(data, 0);
            }));
            methods.Add(typeof(int), new Tuple<toByte, fromByte>((type, param) =>
            {
                return BitConverter.GetBytes((int)param);
            }, (type, data) =>
            {
                return BitConverter.ToInt32(data, 0);
            }));
            methods.Add(typeof(long), new Tuple<toByte, fromByte>((type, param) =>
            {
                return BitConverter.GetBytes((long)param);
            }, (type, data) =>
            {
                return BitConverter.ToInt64(data, 0);
            }));
            methods.Add(typeof(float), new Tuple<toByte, fromByte>((type, param) =>
            {
                return BitConverter.GetBytes((float)param);
            }, (type, data) =>
            {
                return BitConverter.ToSingle(data, 0);
            }));
            methods.Add(typeof(double), new Tuple<toByte, fromByte>((type, param) =>
            {
                return BitConverter.GetBytes((double)param);
            }, (type, data) =>
            {
                return BitConverter.ToDouble(data, 0);
            }));
            methods.Add(typeof(DateTime), new Tuple<toByte, fromByte>((type, param) =>
            {
                var ticks = ((DateTime)param).Ticks;
                return BitConverter.GetBytes(ticks);
            }, (type, data) =>
            {
                var ticks= BitConverter.ToInt64(data, 0);
                return new DateTime(ticks);
            }));
            methods.Add(typeof(byte[]), new Tuple<toByte, fromByte>((type, param) =>
            {
                return (byte[])param;
            }, (type, data) =>
            {
                return data;
            }));
            #endregion
        }
        static int lenSaveLength = 2;
        static Type ReturnType(Type type)
        {
            if (!type.Name.Contains("&"))
            {
                return type;
            }
            return Type.GetType(type.FullName.Replace("&", ""));
        }
        public static byte[] Pack(Type type, object param)
        {
            type = ReturnType(type);
            List<byte> datas = new List<byte>();

            var len = 0;

            byte[] data = null;

            if (param == null)
            {
                len = 0;
            }
            else
            {
                if (type == typeof(object))//object转为string
                {
                    type = typeof(string);
                    param = param + "";
                }
                if (param is Enum)
                {
                    type = Enum.GetUnderlyingType(param.GetType());
                }
                var a = methods.TryGetValue(type, out Tuple<toByte, fromByte> method);
                if (a)
                {
                    data = method.Item1(type, param);
                }
                else
                {
                    if (type.IsGenericType || type.IsArray)
                    {
                        if (type.Name.Contains("Dictionary"))
                        {
                            data = DicFormat.Pack(param);
                        }
                        else if (type.Name.Contains("List`1") || type.IsArray)
                        {
                            data = ListFormat.Pack(param);
                        }
                        else
                        {
                            data = ClassFormat.Pack(type, param);
                        }
                    }
                    else
                    {
                        data = ClassFormat.Pack(type, param);
                    }
                }
      
                if (data != null)
                {
                    len = data.Length;
                }
            }
            var lenData = BitConverter.GetBytes(len).Take(lenSaveLength);
            datas.AddRange(lenData);
            if (len > 0)
            {
                datas.AddRange(data);
            }
            return datas.Count == 0 ? null : datas.ToArray();
        }

        public static object UnPack(Type type, byte[] datas, ref int offset)
        {
            type = ReturnType(type);
            if (type == typeof(object))
            {
                type = typeof(string);
            }
            dynamic obj = null;
            var len = 0;
            var lenData = new byte[4];
            if (datas == null || datas.Length == 0)
            {
                return null;
            }
            Buffer.BlockCopy(datas, offset, lenData, 0, lenSaveLength);

            len = BitConverter.ToInt32(lenData, 0);
            offset += lenSaveLength;
            if (len > 0)
            {
                byte[] data = new byte[len];
                Buffer.BlockCopy(datas, offset, data, 0, len);
                offset += len;

                if (type.BaseType == typeof(Enum))
                {
                     type = Enum.GetUnderlyingType(type);
                }
                var a = methods.TryGetValue(type, out Tuple<toByte, fromByte> method);
                if (a)
                {
                    return method.Item2(type, data);
                }
                if (type.IsGenericType || type.IsArray)
                {
                    if (type.Name.Contains("Dictionary"))
                    {
                        return DicFormat.UnPack(type, data);
                    }
                    else if (type.Name.Contains("List`1") || type.IsArray)
                    {
                        return ListFormat.UnPack(type, data);
                    }
                }
                return ClassFormat.UnPack(type, data);
            }
            return obj;
        }
    }
}
