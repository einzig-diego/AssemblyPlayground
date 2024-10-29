using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace x86AssemblyPlayground.Core
{
    public class CPU
    {
        public int AX { get; set; }
        public int BX { get; set; }
        public int CX { get; set; }
        public int DX { get; set; }
        public int IP { get; set; } // Instruction Pointer
        public bool ZeroFlag { get; set; }
        public bool GreaterFlag { get; set; }
        public bool LessFlag { get; set; }

        public Dictionary<int, string> Memory { get; set; }
        public Dictionary<string, int> Labels { get; set; }
        private Dictionary<int, Action> InterruptVectorTable;
        private Dictionary<int, byte> DataMemory;

        public CPU()
        {
            AX = BX = CX = DX = IP = 0;
            ZeroFlag = GreaterFlag = LessFlag = false;
            Memory = new Dictionary<int, string>();
            Labels = new Dictionary<string, int>();
            DataMemory = new Dictionary<int, byte>();
            InterruptVectorTable = new Dictionary<int, Action>
        {
            { 0x10, HandleInt10 },
            { 0x16, HandleInt16 }
        };
        }

        public void Execute(string instruction)
        {
            if (string.IsNullOrWhiteSpace(instruction))
            {
                return;
            }

            var parts = instruction.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return;
            }

            var opcode = parts[0].ToUpper();

            switch (opcode)
            {
                case "MOV":
                    Mov(parts[1], parts[2]);
                    break;
                case "ADD":
                    Add(parts[1], parts[2]);
                    break;
                case "SUB":
                    Sub(parts[1], parts[2]);
                    break;
                case "INC":
                    Inc(parts[1]);
                    break;
                case "DEC":
                    Dec(parts[1]);
                    break;
                case "JMP":
                    Jmp(parts[1]);
                    break;
                case "CMP":
                    Cmp(parts[1], parts[2]);
                    break;
                case "JE":
                    Je(parts[1]);
                    break;
                case "JNE":
                    Jne(parts[1]);
                    break;
                case "JG":
                    Jg(parts[1]);
                    break;
                case "JL":
                    Jl(parts[1]);
                    break;
                case "INT":
                    Int(parts[1]);
                    break;
                case "DB":
                    DefineString(parts[1], instruction.Substring(instruction.IndexOf(parts[1]) + parts[1].Length).Trim());
                    break;
                default:
                    Console.WriteLine($"Unknown instruction: {instruction}");
                    break;
            }
        }

        private void Mov(string dest, string src)
        {
            if (dest.StartsWith("[") && dest.EndsWith("]"))
            {
                int address = GetValue(dest.Trim('[', ']'));
                DataMemory[address] = (byte)GetValue(src);
            }
            else if (src.StartsWith("[") && src.EndsWith("]"))
            {
                int address = GetValue(src.Trim('[', ']'));
                SetValue(dest, DataMemory[address]);
            }
            else
            {
                int value = GetValue(src);
                SetValue(dest, value);
            }
        }

        private void Add(string dest, string src)
        {
            int value = GetValue(dest) + GetValue(src);
            SetValue(dest, value);
        }

        private void Sub(string dest, string src)
        {
            int value = GetValue(dest) - GetValue(src);
            SetValue(dest, value);
        }

        private void Inc(string operand)
        {
            int value = GetValue(operand) + 1;
            SetValue(operand, value);
        }

        private void Dec(string operand)
        {
            int value = GetValue(operand) - 1;
            SetValue(operand, value);
        }

        private void Jmp(string label)
        {
            if (Labels.ContainsKey(label))
            {
                IP = Labels[label];
            }
            else
            {
                Console.WriteLine($"Unknown label: {label}");
            }
        }

        private void Cmp(string op1, string op2)
        {
            int val1 = GetValue(op1);
            int val2 = GetValue(op2);
            ZeroFlag = val1 == val2;
            GreaterFlag = val1 > val2;
            LessFlag = val1 < val2;
        }

        private void Je(string label)
        {
            if (ZeroFlag)
            {
                Jmp(label);
            }
        }

        private void Jne(string label)
        {
            if (!ZeroFlag)
            {
                Jmp(label);
            }
        }

        private void Jg(string label)
        {
            if (GreaterFlag)
            {
                Jmp(label);
            }
        }

        private void Jl(string label)
        {
            if (LessFlag)
            {
                Jmp(label);
            }
        }

        private void Int(string interrupt)
        {
            int intNum = Convert.ToInt32(interrupt, 16);
            if (InterruptVectorTable.ContainsKey(intNum))
            {
                InterruptVectorTable[intNum]();
            }
            else
            {
                Console.WriteLine($"Unknown interrupt: {interrupt}");
            }
        }

        private void HandleInt10()
        {
            // Simulate basic video services
            int ah = (AX >> 8) & 0xFF; // AH register
            int al = AX & 0xFF; // AL register

            switch (ah)
            {
                case 0x0E: // Teletype output
                    Console.Write((char)al);
                    break;
                default:
                    Console.WriteLine($"Unhandled INT 0x10 function: AH={ah:X2}");
                    break;
            }
        }

        private void HandleInt16()
        {
            // Simulate keyboard input
            char input = Console.ReadKey().KeyChar;
            AX = (AX & 0xFF00) | input; // Store input in AL
        }

        private int GetValue(string operand)
        {
            switch (operand)
            {
                case "AX": return AX;
                case "BX": return BX;
                case "CX": return CX;
                case "DX": return DX;
                case "AH": return (AX >> 8) & 0xFF;
                case "AL": return AX & 0xFF;
                case "BH": return (BX >> 8) & 0xFF;
                case "BL": return BX & 0xFF;
                case "CH": return (CX >> 8) & 0xFF;
                case "CL": return CX & 0xFF;
                case "DH": return (DX >> 8) & 0xFF;
                case "DL": return DX & 0xFF;
                default:
                    if (operand.StartsWith("0x"))
                    {
                        return Convert.ToInt32(operand, 16);
                    }
                    if (Labels.ContainsKey(operand))
                    {
                        return Labels[operand];
                    }
                    return int.Parse(operand);
            }
        }

        private void SetValue(string operand, int value)
        {
            switch (operand)
            {
                case "AX": AX = value; break;
                case "BX": BX = value; break;
                case "CX": CX = value; break;
                case "DX": DX = value; break;
                case "AH": AX = (AX & 0x00FF) | (value << 8); break;
                case "AL": AX = (AX & 0xFF00) | (value & 0xFF); break;
                case "BH": BX = (BX & 0x00FF) | (value << 8); break;
                case "BL": BX = (BX & 0xFF00) | (value & 0xFF); break;
                case "CH": CX = (CX & 0x00FF) | (value << 8); break;
                case "CL": CX = (CX & 0xFF00) | (value & 0xFF); break;
                case "DH": DX = (DX & 0x00FF) | (value << 8); break;
                case "DL": DX = (DX & 0xFF00) | (value & 0xFF); break;
                default: throw new ArgumentException($"Unknown register: {operand}");
            }
        }
        private void DefineString(string label, string value)
        {
            // Remove the leading and trailing quotes from the string value
            value = value.Substring(value.IndexOf("\"")).Trim('"');

            Labels[label] = IP;
            for (int i = 0; i < value.Length; i++)
            {
                DataMemory[IP + i] = (byte)value[i];
            }
            DataMemory[IP + value.Length] = 0;
        }
    }
}
