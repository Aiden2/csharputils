﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpPspEmulator.Core;
using CSharpPspEmulator.Core.Cpu;
using CSharpPspEmulator.Core.Cpu.Assembler;

namespace CSharpPspEmulator
{
	class Program
	{
		static void Main(string[] args)
		{
            var Assembler = new Assembler();

            /*
			var Registers = new RegistersCpu();
			var Memory = new Memory();
			var Cpu = new Cpu();
			var CpuState = new CpuState(Registers, Cpu, Memory);
			//var Cpu = new Cpu();
			CpuState.InstructionData = 0x20;
			Cpu.Execute(CpuState);
            */
            Console.ReadKey();
        }
	}
}
