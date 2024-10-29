# AssemblyPlayground
An x86 CPU simulator written in C# .NET 4.8.

## Features
- **Registers**: AX, BX, CX, DX, IP (Instruction Pointer)
- **Flags**: ZeroFlag, GreaterFlag, LessFlag
- **Memory**: Simulated memory for storing instructions and data
- **Instruction Set**: Supports MOV, ADD, SUB, INC, DEC, JMP, CMP, JE, JNE, JG, JL, INT, and DB instructions
- **Interrupt Handling**: Basic support for INT 0x10 (video services) and INT 0x16 (keyboard input)

## Instructions
- MOV dest, src: Move data from src to dest
- ADD dest, src: Add src to dest
- SUB dest, src: Subtract src from dest
- INC operand: Increment operand by 1
- DEC operand: Decrement operand by 1
- JMP label: Jump to label
- CMP op1, op2: Compare op1 and op2
- JE label: Jump to label if ZeroFlag is set
- JNE label: Jump to label if ZeroFlag is not set
- JG label: Jump to label if GreaterFlag is set
- JL label: Jump to label if LessFlag is set
- INT interrupt: Trigger an interrupt
- DB label, “string”: Define a string in memory

## Example assembly script:
```asm
DB inputBuffer, "  "  ; Buffer to store user input (2 characters + null terminator)
MOV CX, inputBuffer

input:
MOV AH, 0x16  ; Wait for key press
INT 0x16
MOV [CX], AL  ; Store first character
INC CX
MOV AH, 0x16  ; Wait for key press
INT 0x16
MOV [CX], AL  ; Store second character
INC CX

MOV CX, inputBuffer
MOV AL, [CX]
CMP AL, 0x68  ; Compare with 'h'
JNE input  ; If not 'h', wait for input again
INC CX
MOV AL, [CX]
CMP AL, 0x69  ; Compare with 'i'
JNE input  ; If not 'i', wait for input again

DB message, "hello!"
MOV BX, message
print:
MOV AL, [BX]
CMP AL, 0
JE end
MOV AH, 0x0E
INT 0x10
INC BX
JMP print
end:
RUN
```

**Runner:**
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using x86AssemblyPlayground.Core;

namespace x86AssemblyPlayground
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            AssemblyPlayground playground = new AssemblyPlayground();
            playground.Run();
            Console.ReadKey();
        }
    }
}
```


### Licensed under Creative Commons Zero
