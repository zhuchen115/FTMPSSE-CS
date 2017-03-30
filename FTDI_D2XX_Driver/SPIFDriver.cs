using System;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTDevice
{
    /// <summary>
    /// The SPI Driver without using libMPSSE 
    /// </summary>
    /// <remarks>
    /// The initialization procedure is FTDevInit()->FT_MPSSE_Init()->SPI_Init()
    /// </remarks>
    public class SPIFDriver
    {
        FTStatus status;
        IntPtr ftHandle;
        SPIMode mode;

        /// <summary>
        /// Initialize the FTDI device
        /// </summary>
        /// <param name="info">The node info of FTDI device</param>
        /// <returns>The number of FTDI device connected</returns>
        /// <exception cref="FTDIException"> When there are failures in the procedure</exception>
        public uint FTDevInit(out FTDeviceListInfoNode[] info)
        {
            uint devNum = 0;
            status = DllWraper.FT_CreateDeviceInfoList(ref devNum);
            if (status != FTStatus.OK)
                throw new FTDIException(status);
            if (devNum == 0)
            {
                info = null;
                return devNum;
            }
            // Allocate the memory for unmanaged data
            IntPtr ptrInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(FTDeviceListInfoNode)) * (int)devNum);
            // Allocate memory for managed data
            info = new FTDeviceListInfoNode[devNum];
            status = DllWraper.FT_GetDeviceInfoList(ptrInfo, ref devNum);
            if (status != FTStatus.OK)
                throw new FTDIException(status);
            for (int i = 0; i < devNum; i++)
            {
                IntPtr ptrElement = new IntPtr(ptrInfo.ToInt64() + Marshal.SizeOf(typeof(FTDeviceListInfoNode)) * i);
                FTDeviceListInfoNode node = (FTDeviceListInfoNode)Marshal.PtrToStructure(ptrElement, typeof(FTDeviceListInfoNode));
                info[i] = node;
            }
            Marshal.FreeHGlobal(ptrInfo);
            return devNum;
        }

        /// <summary>
        /// Initialize the MPSSE
        /// </summary>
        /// <param name="index">the device index</param>
        /// <exception cref="FTDIException"></exception>
        public void FT_MPSSE_Init(int index = 0)
        {
            status = DllWraper.FT_Open(index, ref ftHandle);
            if (status != FTStatus.OK)
                throw new FTDIException(status, "Cannot Open FTDI device");
            status = DllWraper.FT_ResetDevice(ftHandle);
            if (status != FTStatus.OK)
                throw new FTDIException(status, "Cannot Reset FTDI device");
            status = DllWraper.FT_SetUSBParameters(ftHandle, 10000, 10000);
            if (status != FTStatus.OK)
                throw new FTDIException(status, "Cannot Set USB on FTDI");
            status = DllWraper.FT_SetLatencyTimer(ftHandle, 2);
            if (status != FTStatus.OK)
                throw new FTDIException(status, "Cannot Set  Latency in FTDI");
            status = DllWraper.FT_SetFlowControl(ftHandle, 0x0100, 0x00, 0x00);
            if (status != FTStatus.OK)
                throw new FTDIException(status, "Cannot Set Flow Control in FTDI");
            status = DllWraper.FT_SetBitMode(ftHandle, 0, 0);
            if (status != FTStatus.OK)
                throw new FTDIException(status, "Cannot Reset FTDI Chip");
            status = DllWraper.FT_SetBitMode(ftHandle, 0, 2);
            if (status != FTStatus.OK)
                throw new FTDIException(status, "Cannot Initialize MPSSE on FTDI");
            // Config MPSSE
            byte[] outBuf = new byte[8];
            byte[] inBuf = new byte[8];
            uint szSent = 0,szRead = 0;
            IntPtr inptr = Marshal.AllocHGlobal(inBuf.Length);
            IntPtr outptr = Marshal.AllocHGlobal(outBuf.Length);
            outBuf[0] = 0x84; //Loopback
            Marshal.Copy(outBuf, 0, outptr, 8);
            status = DllWraper.FT_Write(ftHandle, outptr, 1, ref szSent);
            //Check Receive Data
            status = DllWraper.FT_GetQueueStatus(ftHandle, ref szRead);
            if(szRead!=0)
            {
                DllWraper.FT_SetBitMode(ftHandle, 0, 0);
                DllWraper.FT_Close(ftHandle);
                ftHandle = IntPtr.Zero;
                throw new FTDIException("MPSSE Initialization Error, MPSSE receive buffer not zero");
            }
            //Bad Command
            outBuf[0] = 0xAB;
            Marshal.Copy(outBuf, 0, outptr, 8);
            status = DllWraper.FT_Write(ftHandle, outptr, 1, ref szSent);

            do
            {
                status = DllWraper.FT_GetQueueStatus(ftHandle, ref szRead);
            } while (szRead == 0 && status == FTStatus.OK);

            status = DllWraper.FT_Read(ftHandle, inptr, szRead,ref szRead);
            Marshal.Copy(inptr, inBuf, 0,(int)szRead);
            bool echod = false;
            for(int i=0;i<szRead-1;i++)
            {
                if(inBuf[i] == 0xFA&&(inBuf[i+1]==0xAB))
                {
                    echod = true;
                    break;
                }
            }
            if(!echod)
            {
                DllWraper.FT_Close(ftHandle);
                throw new FTDIException("Error in Sync the MPSSE");
            }
            outBuf[0] = 0x85;
            Marshal.Copy(outBuf, 0, outptr, 1);
            status = DllWraper.FT_Write(ftHandle, outptr, 1, ref szSent);

            status = DllWraper.FT_GetQueueStatus(ftHandle, ref szRead);
            if(szRead!=0)
            {
                DllWraper.FT_SetBitMode(ftHandle, 0, 0);
                DllWraper.FT_Close(ftHandle);
                throw new FTDIException("MPSSE Receive Buffer not Empty");
            }
            outBuf[0] = 0x8A; //Disable Clock Divide
            outBuf[1] = 0x8D; //Disable 3 phase data clocking
            outBuf[2] = 0x97; //Disable adaptive clocking
            Marshal.Copy(outBuf, 0, outptr, 3);
            status = DllWraper.FT_Write(ftHandle, outptr, 3, ref szSent);
            // Clean the unmanaged memory
            Marshal.FreeHGlobal(outptr);
            Marshal.FreeHGlobal(inptr);
        }

        /// <summary>
        /// Initialize the spi interface
        /// </summary>
        /// <param name="clkdiv">The clock division of clock</param>
        /// <param name="mode">SPI Operation Mode</param>
        public void SPI_Init(ushort clkdiv = 0x0000,SPIMode mode = SPIMode.MODE0)
        {
            if (ftHandle == IntPtr.Zero)
                throw new InvalidOperationException("The FTDI device is not initialized");
            byte[] outBuffer = new byte[8];
            //byte[] inBuffer = new byte[8];
            uint  szWrite =0;
            IntPtr outptr;
            //inptr = Marshal.AllocHGlobal(8);
            outptr = Marshal.AllocHGlobal(8);
            //SPI Clock Speed
            outBuffer[0] = 0x86;
            outBuffer[1] = (byte)(clkdiv & 0x00ff);
            outBuffer[2] = (byte)((clkdiv >> 8) & 0x00ff);
            outBuffer[3] = 0x80;
            outBuffer[4] = 0x00; // All low
            outBuffer[5] = 0xFB; // OOIOOOOO

            Marshal.Copy(outBuffer, 0, outptr, 6);
            status = DllWraper.FT_Write(ftHandle, outptr, 6, ref szWrite);

            outBuffer[0] = 0x82;
            outBuffer[1] = 0x0f;
            outBuffer[2] = 0x0f;
            Marshal.Copy(outBuffer,0, outptr,3);
            status = DllWraper.FT_Write(ftHandle, outptr, 3, ref szWrite);

            Marshal.FreeHGlobal(outptr);
            this.mode = mode;
        }

        /// <summary>
        /// Send Data to SPI bus
        /// </summary>
        /// <param name="data">The data to be send</param>
        /// <param name="length">The length of data to be send</param>
        /// <returns>The real data sent out</returns>
        /// <remarks>The real data sent out is length +3</remarks>
        public int Write(byte[] data,int length)
        {
            if (length > data.Length)
                throw new ArgumentOutOfRangeException("length", "length is over data Length");
            if (ftHandle == IntPtr.Zero)
                throw new InvalidOperationException("FTDI device not initialized");
            byte OpCode = 0x00;
            switch (mode)
            {
                case SPIMode.MODE0:
                    OpCode = 0x11; //Out Falling Edge
                    break;
                case SPIMode.MODE1:
                    OpCode = 0x10;//Out Rising Edge
                    break;
            }
            byte[] datatosend;
            IntPtr outptr;
            uint szSent = 0,szRealOut = 0;
            int szToSend,lenremain = length;
            int i = 0;
            while (lenremain>0)
            {
                //Separate data in with maxinum 65536 bytes
                szToSend = (lenremain > 65536) ? 65536 : lenremain;
                lenremain -= szToSend;
                datatosend = new byte[szToSend+3];
                datatosend[0] = OpCode;
                // The 2 byte length
                datatosend[1] = (byte)((szToSend-1)&0x00ff);
                datatosend[2] = (byte)(((szToSend - 1)>>8) & 0x00ff);
                Array.Copy(data, 65536 * i, datatosend, 3, szToSend);
                outptr = Marshal.AllocHGlobal(szToSend+2);
                Marshal.Copy(datatosend, 0, outptr, szToSend+3);
                status = DllWraper.FT_Write(ftHandle, outptr, (uint)szToSend+3,ref szSent);
                if (status != FTStatus.OK)
                    throw new FTDIException(status, "send data Error");
                szRealOut += szSent;
                Marshal.FreeHGlobal(outptr);
                i++;
                System.Threading.Thread.Sleep(20);
            }
            return (int)szRealOut;
            
        }

        /// <summary>
        /// Read Data from SPI
        /// </summary>
        /// <param name="data">the read data</param>
        /// <param name="length">The length of data to read</param>
        /// <returns>The real read bytes</returns>
        public int Read(ref byte[] data,int length)
        {
            if (data.Length < length)
                throw new IndexOutOfRangeException("The data doesn't have enough memory");
            if (ftHandle == IntPtr.Zero)
                throw new InvalidOperationException("SPI Driver not initialized");
            byte[] ftcmd = new byte[3];
            IntPtr ptrCmd = Marshal.AllocHGlobal(3);
            IntPtr ptrRead;
            switch (mode)
            {
                case SPIMode.MODE0:
                    ftcmd[0] = 0x20; //In in rising edge
                    break;
                case SPIMode.MODE1:
                    ftcmd[0] = 0x24;// In in falling edge
                    break;
            }
            int lenreamin = length,i=0,szReceiveAll = 0;
            uint szSent = 0, szToRead = 0,szRead = 0;
            while(lenreamin>0)
            {
                szToRead = (lenreamin > 65536) ? 65536 : (uint)lenreamin;
                lenreamin -= (int)szToRead;
                ptrRead = Marshal.AllocHGlobal((int)szToRead);
                ftcmd[1] = (byte)((szToRead-1) & 0x00ff);
                ftcmd[2] = (byte)(((szToRead - 1) >> 8) & 0x00ff);
                Marshal.Copy(ftcmd, 0, ptrCmd, 3);
                status = DllWraper.FT_Write(ftHandle, ptrCmd, 3, ref szSent);
                if (status != FTStatus.OK)
                    throw new FTDIException(status,"Error in sending reading command");
                
                do
                {
                    status = DllWraper.FT_GetQueueStatus(ftHandle, ref szRead);
                    System.Threading.Thread.Sleep(20);
                } while (szRead !=szToRead && status == FTStatus.OK);
                
                status = DllWraper.FT_Read(ftHandle, ptrRead, szToRead, ref szRead);
                if (status != FTStatus.OK)
                    throw new FTDIException(status, "Cannot Read data");

                Marshal.Copy(ptrRead, data, i * 65536, (int)szRead);
                szReceiveAll += (int)szRead;
                Marshal.FreeHGlobal(ptrRead);
            }
            return szReceiveAll;
        }

        public int ReadWrite(byte[] dSend, ref byte[] dRecv, int length)
        {
            if (length > dSend.Length || length > dRecv.Length)
                throw new ArgumentOutOfRangeException("length", "The length is over data length");
            if (ftHandle == IntPtr.Zero)
                throw new InvalidOperationException("SPI device not initialized");
            byte OpCode = 0x00;
            switch (mode)
            {
                case SPIMode.MODE0:
                    OpCode = 0x31; //Out Falling Edge
                    break;
                case SPIMode.MODE1:
                    OpCode = 0x34;//Out Rising Edge
                    break;
            }
            int lenremain = length,i=0,szTransfered = 0;
            uint szToTransfer = 0,szSent =0 ,szRead = 0;
            IntPtr ptrIn, ptrOut;
            byte[] dIn, dOut;
            while(lenremain>0)
            {
                szToTransfer = (lenremain > 65536) ? 65536 : (uint)lenremain;
                lenremain -= (int)szToTransfer;
                dIn = new byte[szToTransfer];
                dOut = new byte[szToTransfer+3];
                dOut[0] = OpCode;
                dOut[1] = (byte)((szToTransfer-1) & 0xff);
                dOut[2] = (byte)(((szToTransfer - 1) >> 8) & 0xff);
                Array.Copy(dSend, i * 65536, dOut, 3, szToTransfer);
                ptrOut = Marshal.AllocHGlobal((int)szToTransfer + 3);
                Marshal.Copy(dOut, 0, ptrOut, dOut.Length);
                status = DllWraper.FT_Write(ftHandle, ptrOut, szToTransfer + 3, ref szSent);
                System.Threading.Thread.Sleep(20);
                do
                {
                    status = DllWraper.FT_GetQueueStatus(ftHandle, ref szRead);
                    System.Threading.Thread.Sleep(20);
                } while (szRead != szToTransfer && status == FTStatus.OK);
                ptrIn = Marshal.AllocHGlobal((int)szToTransfer);
                status = DllWraper.FT_Read(ftHandle, ptrIn,szToTransfer, ref szRead);
                if (status != FTStatus.OK)
                    throw new FTDIException(status, "Cannot Read data");
                Marshal.Copy(ptrIn, dIn, 0, (int)szToTransfer);
                Array.Copy(dIn, 0, dRecv, i * 65536, szToTransfer);
                szTransfered += (int)szRead;
                Marshal.FreeHGlobal(ptrIn);
                Marshal.FreeHGlobal(ptrOut);
                i++;
            }
            return szTransfered;
        }

        /// <summary>
        /// Close the SPI Channel
        /// </summary>
        public void Close()
        {
            if(ftHandle!=IntPtr.Zero)
            {
                DllWraper.FT_SetBitMode(ftHandle, 0, 0);
                DllWraper.FT_Close(ftHandle);
                ftHandle = IntPtr.Zero;
            }
                
        }
    }
}
