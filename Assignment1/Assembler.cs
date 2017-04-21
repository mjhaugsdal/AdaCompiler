using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Assignment1
{

    
    class Assembler
    {
        bool visual = true;
        private string aPath;
        private string path;
        private rdp rdp;
        private StreamReader tacSr;
        private string tacToken;
        char ch = ' ';
        string token = "";

        public Assembler(string path)
        {
            this.path = path;
            //this.path = path;
            string[] asmPath = path.Split('.');
            this.aPath = asmPath[0] + ".asm";
            
            //Create .asm file

            if (File.Exists(this.aPath))
                File.Delete(this.aPath); //Delete existing asm file

            emit("\t.model small");
            emit("\t.586");
            emit("\t.stack 100h");
            emit("\t.data");
        }

        public Assembler(string path, rdp rdp) : this(path)
        {
            this.rdp = rdp;
        }

        public Assembler(string path, rdp rdp, StreamReader tacSr) : this(path, rdp)
        {
            this.tacSr = tacSr;
        }

        internal void emit(string code)
        {
            if (visual)
               // Console.Write(code +"\n");
            using (StreamWriter asm = new StreamWriter(aPath, true))
            {
                    
                    asm.Write(code+"\n");
            }
        }

        public  void genAssembly()
        {
            
            token = getNextToken();

            while(!tacSr.EndOfStream )
            {
                

            }
            //Read last line





                //tacLine tl = getProc(sr.ReadLine());
                //Need to tokenize

                //Read first line from tac file
               /* while (tl.adr1 != "start" && tl.adr2 != "proc" && tl.adr3 != rdp.mainProc )
                {


                    string line = sr.ReadLine();
                    tl = tokenize(line);
                   // Console.WriteLine(line);
                }*/
                
                //while tac != start proc rdp.mainProc
                //If proc, insert procedure
                //Read next line from TAC file



        }

        /* private tacLine getProc(string v)
         {

         }*/
        public char peekNextChar()
        {
            return (char)tacSr.Peek();
        }
        public char getNextChar()
        {

            ch = (char)tacSr.Read();
            return ch;
        }
        private string getNextToken()
        {
            while (!tacSr.EndOfStream)
            {
                
                char ch = getNextChar();

                if (!Char.IsWhiteSpace(ch))
                {
                    //If peek was successful, get next char
                    ch = getNextChar();
                    processToken(); //Process the token

                    return token;

                }

                if (ch == 10 || Char.IsWhiteSpace(ch))
                {
                    ch = getNextChar();
                }
                else
                {
                    return token;
                }
            }
            return token;
        }

        public void processToken( )
        {
         
            while (tacSr.Peek() > -1) // read the rest of the lexeme
            {
                char c = peekNextChar();
                //idt can be letters, underscore and/or digits
                if (!Char.IsLetterOrDigit(c) && c != 95)
                    break;

                else if (tacSr.Peek() == 32 || tacSr.Peek() == 10)
                {
                    break;
                }
                else
                {
                    ch = getNextChar();
                    token = token+ ch;
                }

            }//end while
        }


        private void insertProcedure()
        {
            //Template:
            /*
             
             procname   PROC
                        push bp
                        mov bp, sp
                        sub sp, SIZE OF LOCALS FROM SYM TABLE

                        ; translated code(TAC)

                        add sp, SIZE OF LOCALS FROM SYM TABLE
                        pop bp
                        ret SIZE OF PARAMETERS FROM SYM TABLE
            procname    ENDP
             */
        }


        /// <summary>
        /// Insert all variables at depth 1 in in the .asm 
        /// </summary>
        public void buildDataSeg()
        {
            for(int i = 0; i<SymTab.tableSize; i++)
            {

                SymTab.entry temp = SymTab.hashTable[i].ElementAt(0);


                while (temp.Next != null)
                {
                    if (temp.depth == 1 )
                    {
                        if (temp.typeOfEntry == SymTab.entryType.varEntry || temp.typeOfEntry == SymTab.entryType.constEntry)
                        {
                            emit(temp.lexeme + "\t" + "dw\t" + "?");
                        }

                    }
                    if (temp.Next != null)
                        temp = temp.Next;
                }
                if (temp.depth == 1)
                {
                    if (temp.typeOfEntry == SymTab.entryType.varEntry || temp.typeOfEntry == SymTab.entryType.constEntry)
                    {
                        emit(temp.lexeme + "\t" + "dw\t" + "?");
                    }
                }
            }
        }//End build dataseg

        internal void addCodeAndIncludes()
        {
            emit("\t.code\t");
            emit("\tinclude io.asm");
        }

        public void generateMain()
        {
            emit("main\tPROC");
            mov("ax", "@data");
            mov("ds", "ax");
            call(startProc());
            mov("ah", "4ch");
            emit("\tint 21h");
            emit("main\tENDP");
        }

        private string startProc()
        {
            string ret = rdp.mainProc;
            return ret;
        }

        private void call(string procName)
        {
            emit("\tcall " + procName);
        }

        private void mov(string r1, string r2)
        {
            emit("\t" + r1 + "," +  r2);
        }


    }//End class assembler
}//end namespace
