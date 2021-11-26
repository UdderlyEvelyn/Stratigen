using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratigen2D
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (Kernel k = new Kernel()) k.Run();
        }
    }
}
