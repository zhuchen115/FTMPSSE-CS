/**
 * Copyright [2017] [Chen Zhu, zhuchen115 at gmail.com]
 *
 *Licensed under the Apache License, Version 2.0 (the "License");
 *you may not use this file except in compliance with the License.
 *You may obtain a copy of the License at
 *
 *  http://www.apache.org/licenses/LICENSE-2.0

 *Unless required by applicable law or agreed to in writing, software
 *distributed under the License is distributed on an "AS IS" BASIS,
 *WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *See the License for the specific language governing permissions and
 *limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace FTDevice
{
    [Obsolete("The Class may be not work because of libMPSSE")]
    public class SPIDriver
    {
        FTStatus status = FTStatus.OK;
        ChannelConfig channelconfig;
        uint channels = 0;
        protected IntPtr ftHandle = IntPtr.Zero;

        /// <summary>
        /// Get the numbers of channel
        /// </summary>
        public uint ChannelNum
        {
            get
            {
                status = SPIDllDriver.SPI_GetNumChannels(ref channels);
                if (status != FTStatus.OK)
                    throw new FTDIException(status);
                return channels;
            }
        }

        public SPIDriver()
        {   
            
        }
        /// <summary>
        /// Get the Information of Channel
        /// </summary>
        /// <param name="channel">The index of channel</param>
        /// <exception cref="FTDIException">When Get channel information failed</exception>
        /// <returns>The Device Info Class</returns>
        public DeviceInfo GetChannelInfo(uint channel)
        {
            FTDeviceListInfoNode node = new FTDeviceListInfoNode();
            status = SPIDllDriver.SPI_GetChannelInfo(channel, ref node);
            if (status != FTStatus.OK)
                throw new FTDIException(status);
            return node;
        }

        /// <summary>
        /// Initialize the FTDI device
        /// </summary>
        /// <param name="info">The node info of FTDI device</param>
        /// <returns>The number of FTDI device connected</returns>
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
            for(int i=0;i<devNum;i++)
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
        public void FT_MPSSE_Init(int index=0)
        {
            status = DllWraper.FT_Open(index,ref ftHandle);
            if (status != FTStatus.OK)
                throw new FTDIException(status,"Cannot Open FTDI device");
            status = DllWraper.FT_ResetDevice(ftHandle);
            if (status != FTStatus.OK)
                throw new FTDIException(status, "Cannot Reset FTDI device");
            status = DllWraper.FT_SetUSBParameters(ftHandle, 64000, 64000);
            if (status != FTStatus.OK)
                throw new FTDIException(status, "Cannot Set USB on FTDI");
            status = DllWraper.FT_SetLatencyTimer(ftHandle, 10);
            if (status != FTStatus.OK)
                throw new FTDIException(status, "Cannot Set  Latency in FTDI");
            status = DllWraper.FT_SetBitMode(ftHandle, 0, 0);
            if (status != FTStatus.OK)
                throw new FTDIException(status, "Cannot Reset FTDI Chip");
            status = DllWraper.FT_SetBitMode(ftHandle, 0, 2);
            if (status != FTStatus.OK)
                throw new FTDIException(status, "Cannot Initialize MPSSE on FTDI");
        }



        /// <summary>
        /// Open and initialize the channel
        /// </summary>
        /// <param name="channel">the channel index</param>
        /// <param name="clock">the clock speed in Hz, default 30MHz</param>
        /// <param name="latency">the latency time default 1ms</param>
        /// <param name="options">the SPI Config, Default: MODE0, pin 3 as CS, CS active as low </param>
        /// <param name="pin">the pin configuration,default 0</param>
        /// <exception cref="FTDIException">When failed to the spi channel.</exception>
        /// <returns>1 when succecss</returns>
        public int OpenChannel(uint channel, uint clock = 30000000, byte latency = 1, SPIConfigOption options = SPIConfigOption.MODE0 | SPIConfigOption.CS_DBUS3 | SPIConfigOption.CS_ACTIVELOW,uint pin = 0)
        {
            channelconfig = new ChannelConfig();
            channelconfig.ClockRate = clock;
            channelconfig.LatencyTimer = latency;
            channelconfig.configOptions = (uint)options;
            channelconfig.Pin = pin;

            status = SPIDllDriver.SPI_OpenChannel(channel, ref ftHandle);
            if (status != FTStatus.OK)
                throw new FTDIException(status, "Error in opening spi channel");
            status = SPIDllDriver.SPI_InitChannel(ftHandle,ref channelconfig);
            if (status != FTStatus.OK)
                throw new FTDIException(status, "Error in initializing spi channel");
            return 1;
        }

        public void SPIInit(uint clock = 30000000, byte latency = 1, SPIConfigOption options = SPIConfigOption.MODE0 | SPIConfigOption.CS_DBUS3 | SPIConfigOption.CS_ACTIVELOW, uint pin = 0)
        {
            channelconfig = new ChannelConfig();
            channelconfig.ClockRate = clock;
            channelconfig.LatencyTimer = latency;
            channelconfig.configOptions = (uint)options;
            channelconfig.Pin = pin;
            status = SPIDllDriver.SPI_InitChannel(ftHandle, ref channelconfig);
            if (status != FTStatus.OK)
                throw new FTDIException(status, "Error in initializing spi channel");
            
        }

        /// <summary>
        /// Read From SPI device
        /// </summary>
        /// <param name="length">The length of data to be read</param>
        /// <param name="buffer">The data to be stored</param>
        /// <exception cref="FTDIException">When failed to read from the spi channel.</exception>
        /// <returns>the real transfered length</returns>
        public uint Read(uint length,out byte[] buffer)
        {
            buffer = new byte[length];
            uint sizetransfered = 0;
            if (ftHandle == IntPtr.Zero)
                throw new NullReferenceException("The SPI Interface is not initialized!");
            status = SPIDllDriver.SPI_Read(ftHandle, ref buffer,(uint) length, ref sizetransfered, (uint) (SPITransferOption.SIZE_IN_BYTES | SPITransferOption.CHIPSELECT_ENABLE | SPITransferOption.CHIPSELECT_DISABLE));
            if (status != FTStatus.OK)
                throw new FTDIException(status, "Error is Reading From SPI bus!");
            return sizetransfered;
        }

        /// <summary>
        /// Send data to SPI bus
        /// </summary>
        /// <param name="buffer">The data to be send</param>
        /// <param name="length">The length of data to be send, this number must smaller than the legnth of data</param>
        /// <returns>The real length send out</returns>
        public uint Write(byte[] buffer,uint length = 0)
        {
            uint sizetransfered = 0;
            if (buffer.Length < length)
                throw new ArgumentOutOfRangeException("length", "The length given in arguments is out of data length");
            if (length == 0)
                length = (uint)buffer.Length;
            if(ftHandle ==IntPtr.Zero)
                throw new NullReferenceException("The SPI Interface is not initialized!");
            status = SPIDllDriver.SPI_Write(ftHandle, buffer, length, ref sizetransfered, (uint)(SPITransferOption.SIZE_IN_BYTES | SPITransferOption.CHIPSELECT_ENABLE | SPITransferOption.CHIPSELECT_DISABLE));
            if(status !=FTStatus.OK)
                throw new FTDIException(status, "Error is Send data to SPI bus!");
            return sizetransfered;
        }

        /// <summary>
        /// Read Write data on SPI 
        /// </summary>
        /// <param name="indata">data send to SPI</param>
        /// <param name="outdata">data read from SPI</param>
        /// <param name="length">the lenght to send/receive </param>
        /// <returns>the real numbers of data sent/received </returns>
        public uint ReadWrite(byte[] indata, out byte[] outdata, uint length)
        {
            uint sizetransfered = 0;
            outdata = new byte[length];
            if (indata.Length < length)
                throw new ArgumentOutOfRangeException("length", "The length given in arguments is out of data length");
            if (length == 0)
                length = (uint)indata.Length;
            if (ftHandle == IntPtr.Zero)
                throw new NullReferenceException("The SPI Interface is not initialized!");
            status = SPIDllDriver.SPI_ReadWrite(ftHandle, indata,ref outdata, length, ref sizetransfered, (uint)(SPITransferOption.SIZE_IN_BYTES | SPITransferOption.CHIPSELECT_ENABLE | SPITransferOption.CHIPSELECT_DISABLE));
            if (status != FTStatus.OK)
                throw new FTDIException(status, "Error in ReadWrite data on SPI bus!");
            return sizetransfered;
        }
        /// <summary>
        /// Close the SPI port
        /// </summary>
        public void CloseChannel()
        {
            if (ftHandle == IntPtr.Zero)
                return;
            SPIDllDriver.SPI_CloseChannel(ftHandle);
            ftHandle = IntPtr.Zero;
        }

    }



    public class DeviceInfo
    {
        UInt32 flags;
        UInt32 type;
        UInt32 id;
        UInt32 locid;
        string serialnumber;
        string description;
        
        public UInt32 Flags
        {
            get { return flags; }
        }
        public UInt32 Type
        {
            get { return type; }
        }
        public UInt32 ID
        {
            get { return id; }
        }
        public UInt32 LocId
        {
            get { return locid; }
        }
        public string SerialNumber
        {
            get { return serialnumber; }
        }
        public string Description
        {
            get { return description; }
        }
        // Not allowed to construct this class
        private DeviceInfo()
        {

        }
        /// <summary>
        /// Make the Description API directly turn into object
        /// </summary>
        /// <param name="node">The node info</param>
        public static implicit operator DeviceInfo(FTDeviceListInfoNode node)
        {
            DeviceInfo info = new DeviceInfo();
            info.id = node.ID;
            info.flags = node.Flags;
            info.description = node.Description;
            info.locid = node.LocId;
            info.serialnumber = node.SerialNumber;
            info.type = node.Type;
            return info;
        }

    }
}
