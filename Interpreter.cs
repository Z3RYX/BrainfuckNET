using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace BrainfuckNET
{
    public static class Interpreter
    {
        private static ushort Pointer = 0;
        private static int CodePointer = 0;
        private static ushort[] Tape { get; set; }
        private static string _Code { get; set; }
        private static int[] Bracemap { get; set; }
        private static StringBuilder OutString { get; set; }
        private static string InString { get; set; }

        /// <summary>
        /// The amount of cells on the tape; Defaults to 30000
        /// </summary>
        public static ushort TapeLength = 30_000;

        /// <summary>
        /// Whether to use 8 bit cells or 16 bit cells; Defaults to true
        /// </summary>
        public static bool Use8BitCell = true;

        /// <summary>
        /// Whether or not it should output to StandardOutput; Defaults to true
        /// </summary>
        public static bool ConsoleOutput = true;

        /// <summary>
        /// Executes the Brainfuck code
        /// </summary>
        /// <returns>A tuple containing the steps, time, and output of the code execution</returns>
        public static (int steps, double total_ms, string output) Execute(string code, string input = null, string OutFile = null)
        {
            Tape = new ushort[TapeLength];

            Regex r = new Regex(@"[^<>\+\-\[\],.]");
            _Code = r.Replace(code, "");
            BuildBracemap(_Code);

            OutString = new StringBuilder();
            InString = input;

            int steps = 0;
            DateTime start = DateTime.Now;
            while (CodePointer < _Code.Length) {
                switch (_Code[CodePointer]) {
                    case '+':
                        Plus();
                        break;
                    case '-':
                        Minus();
                        break;
                    case '<':
                        LeftShift();
                        break;
                    case '>':
                        RightShift();
                        break;
                    case '[':
                        LoopStart();
                        break;
                    case ']':
                        LoopEnd();
                        break;
                    case '.':
                        Output();
                        break;
                    case ',':
                        Input();
                        break;
                }
                steps++;
                CodePointer++;
            }

            if (OutFile != null) {
                var w = new StreamWriter(OutFile);
                w.Write(OutString.ToString());
                w.Close();
            }

            return (steps, (DateTime.Now - start).TotalMilliseconds, OutString.ToString());
        }

        private static void BuildBracemap(string code)
        {
            Stack<int> temp = new Stack<int>();
            Bracemap = new int[code.Length];

            for (int i = 0; i < code.Length; i++) {
                if (code[i] == '[') {
                    temp.Push(i);
                }

                if (code[i] == ']') {
                    if (temp.Count == 0) throw new Exception("Invalid Syntax - Missing opening brace");

                    int start = temp.Pop();
                    Bracemap[start] = i;
                    Bracemap[i] = start;
                }
            }

            if (temp.Count > 0) throw new Exception("Invalid Syntax - Missing closing brace");
        }

        private static void Plus()
        {
            Tape[Pointer] = Use8BitCell ? (Tape[Pointer] + 1 > byte.MaxValue ? (ushort)0 : (ushort)(Tape[Pointer] + 1)) : (ushort)(Tape[Pointer] + 1);
        }

        private static void Minus()
        {
            Tape[Pointer] = Use8BitCell ? (Tape[Pointer] == 0 ? byte.MaxValue : (ushort)(Tape[Pointer] - 1)) : (ushort)(Tape[Pointer] - 1);
        }

        private static void RightShift()
        {
            Pointer++;
            if (Pointer == TapeLength) Pointer = 0;
        }

        private static void LeftShift()
        {
            Pointer--;
            if (Pointer == ushort.MaxValue) Pointer = (ushort)(TapeLength - 1);
        }

        private static void LoopStart()
        {
            if (Tape[Pointer] == 0) CodePointer = Bracemap[CodePointer];
        }

        private static void LoopEnd()
        {
            if (Tape[Pointer] != 0) CodePointer = Bracemap[CodePointer];
        }

        private static void Output()
        {
            if (ConsoleOutput) Console.Write((char)Tape[Pointer]);
            OutString.Append(((char)Tape[Pointer]).ToString());
        }

        private static void Input()
        {
            if (InString != null) {
                Tape[Pointer] = InString.Length == 0 ? (byte)0 : (byte)InString[0];
                InString = InString.Length <= 1 ? "" : InString.Substring(1);
            } else {
                Tape[Pointer] = (byte)Console.ReadKey().KeyChar;
            }
        }
    }
}