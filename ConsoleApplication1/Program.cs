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

            SPIFDriver drv = new SPIFDriver();
            FTDeviceListInfoNode[] devs;
            uint devNum=drv.FTDevInit(out devs);
            int devid = 0;
            Console.WriteLine("Number of FTDI Device Found {0}", devNum);
            if (devNum < 0)
                return;
            if(devNum>1)
            {
                Console.WriteLine("Select One device to Run:");
                for(int i= 0;i<devNum;i++)
                {
                    Console.WriteLine("{0}: ID={1:X} Name={2}", i, devs[i].ID, devs[i].Description);
                    Console.WriteLine("Device Info:");
                    Console.WriteLine("ID:{0:X}", devs[i].ID);
                    Console.WriteLine("LocId:{0:X}", devs[i].LocId);
                    Console.WriteLine("Description:{0}", devs[i].Description);
                    Console.WriteLine("Serial Number:{0}", devs[i].SerialNumber);
                }
                devid = Int32.Parse(Console.ReadLine());
            }
            Console.WriteLine("Device Info:");
            Console.WriteLine("ID:{0:X}", devs[devid].ID);
            Console.WriteLine("LocId:{0:X}", devs[devid].LocId);
            Console.WriteLine("Description:{0}", devs[devid].Description);
            Console.WriteLine("Serial Number:{0}", devs[devid].SerialNumber);
            drv.FT_MPSSE_Init(devid);
            drv.SPI_Init();

            byte[] data = new byte[128];
            data[0] = 0xCC;
            ArrayList.Repeat(data[0], data.Length).CopyTo(data);
            int szSent = drv.Write(data,data.Length);
            Console.WriteLine("Size Sent:{0}", szSent);
            drv.Close();
            Console.Read();
        }
    }
}
