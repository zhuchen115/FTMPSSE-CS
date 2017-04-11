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
            uint devNum = drv.FTDevInit(out devs);
            int devid = 0;
            Console.WriteLine("Number of FTDI Device Found {0}", devNum);
            if (devNum < 0)
                return;
            if (devNum > 1)
            {
                Console.WriteLine("Select One device to Run:");
                for (int i = 0; i < devNum; i++)
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
            Console.WriteLine("\n\n");
            Console.WriteLine("Device Info:");
            Console.WriteLine("ID:{0:X}", devs[devid].ID);
            Console.WriteLine("LocId:{0:X}", devs[devid].LocId);
            Console.WriteLine("Description:{0}", devs[devid].Description);
            Console.WriteLine("Serial Number:{0}", devs[devid].SerialNumber);
            drv.FT_MPSSE_Init(devid);
            Console.Write("Clock Divisor: ");
            ushort clk = UInt16.Parse(Console.ReadLine());
            drv.SPI_Init(clk);
            while (true)
            {
                Console.Write(">");
                string cmd = Console.ReadLine();
                cmd = cmd.Trim();
                if (cmd.StartsWith("write") || cmd == "w")
                    cmd_write(drv, cmd);
                else if (cmd.StartsWith("exit") || cmd=="q")
                {
                    break;
                }
                else if (cmd.StartsWith("read") || cmd == "r")
                {
                    cmd_read(drv, cmd);
                }
                else if(cmd.StartsWith("readwrite")|| cmd=="rw")
                {
                    cmd_readwrite(drv, cmd);
                }
                else
                {
                    Console.WriteLine("Invalid command. Available Command: write (w),read (r), readwrite(rw)");
                }
            }

        }
        static void cmd_write(SPIFDriver drv, string cmd)
        {
            string[] parms = cmd.Split(' ');
            int len = 0;
            if (parms.Count() == 1)
            {
                Console.Write("The number bytes: ");
                try
                {
                    len = Int32.Parse(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
            }
            else
            {
                try
                {
                    len = Int32.Parse(parms[1]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.Write("The number bytes: ");
                    try
                    {
                        len = Int32.Parse(Console.ReadLine());
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine(ex2.Message);
                        return;
                    }
                }
            }
            Console.WriteLine("Input the data to be sent in HEX, separate with \",\"");
            string valstr = Console.ReadLine();
            byte[] bts = strHexToByteArray(valstr);
            if (bts.Count() < len)
            {
                Console.WriteLine("Request no. of bytes {0} larger than real input bytes {1}", len, bts.Count());
                return;
            }
            drv.Write(bts, len);
        }

        static byte[] strHexToByteArray(string str)
        {
            string[] values = str.Split(',');
            if (values.Count() < 1)
                return null;
            byte[] bts = new byte[values.Count()];
            for (int i = 0; i < values.Count(); i++)
            {
                try
                {
                    byte num = Convert.ToByte(values[i].Trim(), 16);
                    bts[i] = num;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Cannot convert {0},at {1} into byte.", values[i], i);
                    Console.WriteLine(ex.Message);
                }

            }
            return bts;

        }

        static void cmd_read(SPIFDriver drv, string cmd)
        {
            string[] parms = cmd.Split(' ');
            int len = 0;
            if (parms.Count() == 1)
            {
                Console.Write("Number of bytes to read: ");
                try
                {
                    len = Int32.Parse(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
            }
            else
            {
                try
                {
                    len = Int32.Parse(parms[1]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.Write("The number bytes: ");
                    try
                    {
                        len = Int32.Parse(Console.ReadLine());
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine(ex2.Message);
                        return;
                    }
                }
            }
            byte[] result = new byte[len+1];
            int numread = drv.Read(ref result, len);
            Console.WriteLine("Number of bytes readed {0}", numread);
            for (int i=0;i<numread;i++)
            {
                if(i==numread-1)
                {
                    Console.Write("{0:X}", result[i]);
                }else
                {
                    Console.Write("{0:X},", result[i]);
                }
            }
            Console.WriteLine();
        }

        static void cmd_readwrite(SPIFDriver drv,string cmd)
        {
            string[] parms = cmd.Split(' ');
            int len = 0;
            if (parms.Count() == 1)
            {
                Console.Write("The number bytes: ");
                try
                {
                    len = Int32.Parse(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
            }
            else
            {
                try
                {
                    len = Int32.Parse(parms[1]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.Write("The number bytes: ");
                    try
                    {
                        len = Int32.Parse(Console.ReadLine());
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine(ex2.Message);
                        return;
                    }
                }
            }
            Console.WriteLine("Input the data to be sent in HEX, separate with \",\"");
            string valstr = Console.ReadLine();
            byte[] bts = strHexToByteArray(valstr);
            if (bts.Count() < len)
            {
                Console.WriteLine("Request no. of bytes {0} larger than real input bytes {1}", len, bts.Count());
                return;
            }
            byte[] drecv = new byte[len];
            int numtf = drv.ReadWrite(bts, ref drecv, len);
            Console.WriteLine("Number of bytes transfered {0}", numtf);
            for (int i = 0; i < numtf; i++)
            {
                if (i == numtf - 1)
                {
                    Console.Write("{0:X}", drecv[i]);
                }
                else
                {
                    Console.Write("{0:X},", drecv[i]);
                }
            }
            Console.WriteLine();
        }
    }
}
