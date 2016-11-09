using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duncan.PEMS.Install
{
    class Program
    {
        static void Main(string[] args)
        {
            (new Installer(args)).Run();
        }
    }
}
