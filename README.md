# AdaCompilerV2

Goto: https://github.com/mjhaugsdal/AdaCompilerV2

Compiler for a subset of Ada. Written in C#.

Scans and checks .ada files from args[] for syntax errors, giving an appropriate error message.

Ada is a strongly typed language (https://learn.adacore.com/courses/intro-to-ada/chapters/strongly_typed_language.html) 
and does not allow inference (Easier to design the compiler to fail at first error though).

The compiler also creates a .tac (Three-address-code) https://en.wikipedia.org/wiki/Three-address_code to be used by an assembler.
The assembler uses MASM - But I'm not sure about the setup (Instructor used dosbox with some added IO dependencies.)
