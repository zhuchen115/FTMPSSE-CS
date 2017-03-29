using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTDevice;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            SPIDriver drv = new SPIDriver();
            Console.WriteLine("Channel Found {0}", drv.ChannelNum);
            drv.OpenChannel(0);
            byte[] data = new byte[128];
            data[0] = 0xAA;
            ArrayList.Repeat(data[0], data.Length).CopyTo(data);
            drv.Write(data);

        }
    }
}
