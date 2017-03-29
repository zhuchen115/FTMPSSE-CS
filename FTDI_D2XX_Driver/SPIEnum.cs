using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;


namespace FTDevice
{
    /// <summary>
    /// The SPI Transfer Option
    /// Use "|" Operator to link them
    /// </summary>
    [Flags]
    public enum SPITransferOption : uint
    {
        SIZE_IN_BYTES = 0x00000000,
        SIZE_IN_BITS = 0x00000001,
        CHIPSELECT_ENABLE = 0x00000002,
        CHIPSELECT_DISABLE=0x00000004
    }

    /// <summary>
    ///  The SPI Configuration Option
    /// </summary>
    [Flags]
    public enum SPIConfigOption : uint
    {
        MODE_MASK = 0x00000003,
        MODE0 = 0x00000000,
        MODE1 = 0x00000001,
        MODE2 = 0x00000002,
        MODE3 = 0x00000003,
        CS_MASK = 0x0000001C,       /*111 00*/
        CS_DBUS3 = 0x00000000,      /*000 00*/
        CS_DBUS4 = 0x00000004,      /*001 00*/
        CS_DBUS5 = 0x00000008,      /*010 00*/
        CS_DBUS6 = 0x0000000C,      /*011 00*/
        CS_DBUS7 = 0x00000010,      /*100 00*/
        CS_ACTIVELOW = 0x00000020
    }

    /// <summary>
    /// SPI Channel Initialization Configuration
    /// </summary>
    [StructLayout(LayoutKind.Sequential,CharSet = CharSet.Ansi)]
    public struct ChannelConfig 
    {
        /// <summary>
        /// The clock frequency of SPI in Hz
        /// </summary>
        /// <remarks>
        /// The clock rate support by driver range from 0 to 30MHz
        /// </remarks>
        public uint ClockRate;

        /// <summary>
        /// The latency time of spi communication in ms
        /// </summary>
        public byte LatencyTimer;

        /// <summary>
        /// Configuration of SPI<see cref="SPIConfigOption"/> 
        /// </summary>
        public uint configOptions;   /*This member provides a way to enable/disable features
	specific to the protocol that are implemented in the chip
	BIT1-0=CPOL-CPHA:	00 - MODE0 - data captured on rising edge, propagated on falling
 						01 - MODE1 - data captured on falling edge, propagated on rising
 						10 - MODE2 - data captured on falling edge, propagated on rising
 						11 - MODE3 - data captured on rising edge, propagated on falling
	BIT4-BIT2: 000 - A/B/C/D_DBUS3=ChipSelect
			 : 001 - A/B/C/D_DBUS4=ChipSelect
 			 : 010 - A/B/C/D_DBUS5=ChipSelect
 			 : 011 - A/B/C/D_DBUS6=ChipSelect
 			 : 100 - A/B/C/D_DBUS7=ChipSelect
 	BIT5: ChipSelect is active high if this bit is 0
	BIT6 -BIT31		: Reserved
	*/
        /// <summary>
        /// Configuration for FTDI IO port
        /// </summary>
        public uint Pin;/*BIT7   -BIT0:   Initial direction of the pins	*/
                        /*BIT15 -BIT8:   Initial values of the pins		*/
                        /*BIT23 -BIT16: Final direction of the pins		*/
                        /*BIT31 -BIT24: Final values of the pins		*/
        public UInt16 reserved;
    }

    [StructLayout(LayoutKind.Sequential,CharSet =CharSet.Ansi)]
    public struct FTDeviceListInfoNode
    {
        public UInt32 Flags;
        public UInt32 Type;
        public UInt32 ID;
        public UInt32 LocId;
        [MarshalAs(UnmanagedType.LPStr,SizeConst = 16)]
        public string SerialNumber;
        [MarshalAs(UnmanagedType.LPStr,SizeConst = 64)]
        public string Description;
        public IntPtr ftHandle;
    }
}
