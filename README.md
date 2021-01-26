# BrainfuckNET
An easy-to-use library that allows you to interpret Brainfuck code. By default it interprets by the original language specification, but has a few settings that can be changed.

## Use Example (for version 1.2.1)
```cs
using BrainfuckNET;

// Running code with default settings
Interpreter.Execute("+[.+]");

// Running code with an input string, if not supplied, the library will take input from the console#s standard input
Interpreter.Execute(",[.,]", input: "Hello World!"); // If the input runs out of characters, it will interpret the next input as NULL

// Running code and saving the output to a file (file will be created and overwritten if it exists)
Interpreter.Execute("+[.+]", OutFile: "output.txt");

// Execute() returns a tuple of three values
(int Steps, double TimeInMilliseconds, string Output) = Interpreter.Execute("+[.+]");

// These values can be changed before executing
Interpreter.TapeLength = 30_000; // Amount of cells on the tape as a ushort
Interpreter.Use8BitCell = true // Whether the cells are 8 bits or 16 bits in size
Interpreter.ConsoleOutput = true // Whether or not the resulting output should be written to the console
```
