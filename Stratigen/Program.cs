using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Stratigen.Libraries;
using Stratigen.Datatypes;
using System.Reflection;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Stratigen
{
    public static class Program
    {
        public static bool StartGame = false;

        [STAThread]
        static void Main()
        {
            using (Menu m = new Menu()) m.Run();
            if (StartGame)
            {
                StartGame = false;
                Play();
            }

        }

        public static void Play()
        {
            using (Kernel k = new Kernel()) k.Run();
        }
    }
}
