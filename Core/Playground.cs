using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace x86AssemblyPlayground.Core
{
    public class AssemblyPlayground
    {
        private CPU cpu;

        public AssemblyPlayground()
        {
            cpu = new CPU();
        }

        public void Run()
        {
            Console.WriteLine("Enter assembly instructions (type 'RUN' to execute):");

            string line;
            int address = 0;
            List<string> instructions = new List<string>();

            while ((line = Console.ReadLine()) != null)
            {
                if(line.IndexOf(';')!=-1)
                    line = line.Substring(0, line.IndexOf(';'));
                if (line.ToUpper() == "RUN")
                {
                    break;
                }

                instructions.Add(line);
            }

            PreprocessLabels(instructions);
            LoadInstructions(instructions);
            PrintSeparator();
            ExecuteProgram();
        }

        private void PreprocessLabels(List<string> instructions)
        {
            for (int i = 0; i < instructions.Count; i++)
            {
                string line = instructions[i];
                if (line.Contains(":"))
                {
                    var parts = line.Split(':');
                    cpu.Labels[parts[0].Trim()] = i;
                    instructions[i] = parts.Length > 1 ? parts[1].Trim() : string.Empty;
                }
            }
        }

        private void LoadInstructions(List<string> instructions)
        {
            for (int i = 0; i < instructions.Count; i++)
            {
                cpu.Memory[i] = instructions[i];
            }
        }

        private void ExecuteProgram()
        {
            while (cpu.Memory.ContainsKey(cpu.IP))
            {
                string instruction = cpu.Memory[cpu.IP];
                cpu.Execute(instruction);
                cpu.IP++;
            }

            PrintRegisters();
        }
        private void PrintRegisters()
        {
            PrintSeparator();
            Console.WriteLine($"AX: {cpu.AX:X4} (AH: {cpu.AX >> 8:X2}, AL: {cpu.AX & 0xFF:X2})");
            Console.WriteLine($"BX: {cpu.BX:X4} (BH: {cpu.BX >> 8:X2}, BL: {cpu.BX & 0xFF:X2})");
            Console.WriteLine($"CX: {cpu.CX:X4} (CH: {cpu.CX >> 8:X2}, CL: {cpu.CX & 0xFF:X2})");
            Console.WriteLine($"DX: {cpu.DX:X4} (DH: {cpu.DX >> 8:X2}, DL: {cpu.DX & 0xFF:X2})");
            Console.WriteLine($"IP: {cpu.IP:X4}");
            Console.WriteLine($"ZeroFlag: {cpu.ZeroFlag}");
            Console.WriteLine($"GreaterFlag: {cpu.GreaterFlag}");
            Console.WriteLine($"LessFlag: {cpu.LessFlag}");
        }

        private void PrintSeparator()
        {
            Console.WriteLine("\n"+new string('-', 40));
        }
    }
}
