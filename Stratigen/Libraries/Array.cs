using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratigen.Datatypes;

namespace Stratigen.Libraries
{
    public static class Array
    {
        public static Array2<byte> ToByteArray(this Array2<float> data)
        {
            Array2<byte> result = new Array2<byte>(data.Width, data.Height);
            for (int i = 0; i < data.Count; i++)
            {
                result.Set(i, (byte)(Math.Abs(data.Get(i)) * 255));
            }
            return result;
        }

        public static void ToConsole(this Array2<float> data)
        {
            Console.WriteLine(data.Width + "x" + data.Height + " Array2<float>");
            for (int y = 0; y < data.Height; y++)
            {
                Console.Write("\n");
                for (int x = 0; x < data.Width; x++)
                {
                    Console.Write(data.Get(x, y).ToString() + ' ');
                }
            }
        }

        public static void ToConsole(this Array2<byte> data)
        {
            Console.WriteLine(data.Width + "x" + data.Height + " Array2<byte>");
            for (int y = 0; y < data.Height; y++)
            {
                Console.Write("\n");
                for (int x = 0; x < data.Width; x++)
                {
                    Console.Write(data.Get(x, y).ToString() + ' ');
                }
            }
        }
    }
}
