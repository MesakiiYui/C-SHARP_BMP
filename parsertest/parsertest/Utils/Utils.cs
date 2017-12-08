using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parsertest.Utils
{
    class Utils
    {
        //byte[] 转 string
        public static string byte2string(byte[] input)
        {
            StringBuilder sb = new StringBuilder(input.Length * 2);
            foreach (byte b in input)
            {
                sb.Append(Byte2Hex(b));
            }

            return sb.ToString();
        }
        private static string Byte2Hex(byte b)
        {
            return b.ToString("X2");
        }
        public static byte[] hexString2Bytes(string hex)
        {
            int len = hex.Length / 2;
            byte[] buffer = new byte[len];

            for (int i = 0; i < len; i++)
            {
                buffer[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return buffer;
        }
        
        //将string反转
        public static string ReverseString(string hex)
        {
            int len = hex.Length / 2;
            string[] olds = new string[len];
            for (int i = 0; i < len; i++)
            {
                olds[i] = hex.Substring(i * 2, 2);
            }
            Array.Reverse(olds);
            return string.Join("", olds);
        }

        //返回时间戳
        public static string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }


    }

}
