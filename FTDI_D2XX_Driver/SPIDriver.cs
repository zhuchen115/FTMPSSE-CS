using System;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTDevice
{
    
    internal class SPIDllDriver
    {
        [DllImport("libMPSSE.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern FTStatus SPI_GetNumChannels(ref uint numChannels);
        [DllImport("libMPSSE.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern FTStatus SPI_GetChannelInfo(uint index, ref FTDeviceListInfoNode chanInfo);
        [DllImport("libMPSSE.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern FTStatus SPI_OpenChannel(uint index, ref IntPtr handle);
        [DllImport("libMPSSE.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern FTStatus SPI_InitChannel(IntPtr handle, ref ChannelConfig config);
        [DllImport("libMPSSE.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern FTStatus SPI_CloseChannel(IntPtr handle);
        [DllImport("libMPSSE.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern FTStatus SPI_Read(IntPtr handle, ref byte[] buffer, uint sizeToTransfer, ref uint sizeTransfered, uint options);
        [DllImport("libMPSSE.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern FTStatus SPI_Write(IntPtr handle, byte[] buffer, uint sizeToTransfer, ref uint sizeTransfered, uint options);
        [DllImport("libMPSSE.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern FTStatus SPI_ReadWrite(IntPtr handle, byte[] inBuffer, ref byte[] outBuffer, uint sizeToTransfer, ref uint sizeTransferred, uint transferOptions);
        [DllImport("libMPSSE.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern FTStatus SPI_IsBusy(IntPtr handle, ref bool state);
        [DllImport("libMPSSE.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void Init_libMPSSE();
        [DllImport("libMPSSE.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void Cleanup_libMPSSE();
        [DllImport("libMPSSE.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern FTStatus SPI_ChangeCS(IntPtr handle, uint configOptions);
        [DllImport("libMPSSE.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern FTStatus FT_WriteGPIO(IntPtr handle, byte dir, byte value);
        [DllImport("libMPSSE.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern FTStatus FT_ReadGPIO(IntPtr handle, ref byte value);
        [DllImport("libMPSSE.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern FTStatus SPI_ToggleCS(IntPtr handle, bool state);
    }
}
