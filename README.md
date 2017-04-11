# FT4342H SPI CSharp
The CSharp Driver for FTDI 42xx Chip
The driver is using [MPSSE opcode](http://www.ftdichip.com/Support/Documents/AppNotes/AN_108_Command_Processor_for_MPSSE_and_MCU_Host_Bus_Emulation_Modes.pdf) from FTDI

## Command Window Operation
1. Select the device, the device must support MPSSE [Detail](http://www.ftdichip.com/Support/Documents/AppNotes/AN_135_MPSSE_Basics.pdf).
2. Choose the clock divisor, this will change the clock frequency of the SPI
3. Command supported
  + write or w: write number of bytes to SPI bus
  + read or r: read number of bytes from SPI bus
  + readwrite or rw : Read and Write on SPI bus
4. The bytes should input in hexadecimal without 0x and separated with ","
