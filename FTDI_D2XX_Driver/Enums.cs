using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTDevice
{
    public enum FTStatus : uint
    {
        OK,
        INVALID_HANDLE,
        DEVICE_NOT_FOUND,
        DEVICE_NOT_OPENED,
        IO_ERROR,
        INSUFFICIENT_RESOURCES,
        INVALID_PARAMETER,
        INVALID_BAUD_RATE,
        DEVICE_NOT_OPENED_FOR_ERASE,
        DEVICE_NOT_OPENED_FOR_WRITE,
        FAILED_TO_WRITE_DEVICE,
        EEPROM_READ_FAILED,
        EEPROM_WRITE_FAILED,
        EEPROM_ERASE_FAILED,
        EEPROM_NOT_PRESENT,
        EEPROM_NOT_PROGRAMMED,
        INVALID_ARGS,
        NOT_SUPPORTED,
        OTHER_ERROR,
        DEVICE_LIST_NOT_READY
    }
    [Flags]
    public enum FTOpenExFlags
    {
        OPEN_BY_SERIAL_NUMBER=1,
        OPEN_BY_DESCRIPTION=2,
        OPEN_BY_LOCATION=4,
        OPEN_MASK = 7
    }

    public enum FTDeviceType :uint
    {
        DEVICE_BM,
        DEVICE_AM,
        DEVICE_100AX,
        DEVICE_UNKNOWN,
        DEVICE_2232C,
        DEVICE_232R,
        DEVICE_2232H,
        DEVICE_4232H,
        DEVICE_232H,
        DEVICE_X_SERIES,
        DEVICE_4222H_0,
        DEVICE_4222H_1_2,
        DEVICE_4222H_3,
        DEVICE_4222_PROG,
        DEVICE_900,
        DEVICE_930,
        DEVICE_UMFTPD3A
    }

    [Flags]
    public enum FTListDeviceFlags :uint
    {
        LIST_NUMBER_ONLY=0x80000000,
        LIST_BY_INDEX=0x40000000,
        LIST_ALL=0x20000000
    }

    public enum FTBaudRate
    {
        BAUD_300 = 300,
        BAUD_600 = 600,
        BAUD_1200 = 1200,
        BAUD_2400 = 2400,
        BAUD_4800 = 4800,
        BAUD_9600 = 9600,
        BAUD_14400 = 14400,
        BAUD_19200 = 19200,
        BAUD_38400 = 38400,
        BAUD_57600 = 57600,
        BAUD_115200 = 115200,
        BAUD_230400 = 230400,
        BAUD_460800 = 460800,
        BAUD_921600 = 921600
    }

    public enum BitMode : byte
    {
        RESET = 00,
        ASYNC_BITBANG = 01,
        MPSSE = 02,
        SYNC_BITBANG = 04,
        MCU_HOST = 08,
        FAST_SERIAL = 10,
        CBUS_BITBANG = 20,
        SYNC_FIFO = 40
    }

    [Flags]
    public enum FTFlowControl :UInt16
    {
        NONE = 0x0000,
        RTS_CTS = 0x0100,
        DTR_DSR = 0x0200,
        XON_XOFF = 0x0400
    }

    public enum FTParity: byte
    {
        NONE = 0,
        ODD = 1,
        EVEN = 2,
        MARK = 3,
        SPACE = 4
    }

    public enum FTPurgeBuffer
    {
        RX =1,
        TX =2
    }

    [Flags]
    public enum FTEvent
    {
        RXCHAR = 1,
        MODEM_STATUS = 2,
        LINE_STATUS = 4,
    }
}
