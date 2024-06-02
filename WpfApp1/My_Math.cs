using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace gangway_controller
{
    class My_Math
    {
        public static byte[] IntToByte(int a, int b)
        {
            byte[] byteArray1 = BitConverter.GetBytes(a);
            Array.Resize(ref byteArray1, b);
            return byteArray1;//a=300,b=3;则byteArray1={44,1,0};//44是十进制
        }
        public static byte[] ByteToMany(byte[] a, byte[] b)
        {
            return a.Concat(b).ToArray();
        }
        public static double X_ts(double a, double b)
        {
            return 340 * a / b + 20;
        }
        public static double Y_ts(double a, double b)
        {
            return -170 * a / b + 190;
        }
        public static double Y_ts1(double a, double b)
        {
            return -85 * a / b + 105;
        }
        public static byte[] CRCCalc(byte[] data)
        {
            //crc计算赋初始值
            int crc = 0xffff;
            for (int i = 0; i < data.Length; i++)
            {
                crc = crc ^ data[i];
                for (int j = 0; j < 8; j++)
                {
                    int temp;
                    temp = crc & 1;
                    crc = crc >> 1;
                    crc = crc & 0x7fff;
                    if (temp == 1)
                    {
                        crc = crc ^ 0xa001;
                    }
                    crc = crc & 0xffff;
                }
            }
            //CRC寄存器的高低位进行互换
            byte[] crc16 = new byte[2];
            //CRC寄存器的高8位变成低8位，
            crc16[1] = (byte)((crc >> 8) & 0xff);
            //CRC寄存器的低8位变成高8位
            crc16[0] = (byte)(crc & 0xff);
            return crc16;
        }
    }
}
