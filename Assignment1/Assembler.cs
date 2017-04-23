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
        private string code = null;
        char ch = ' ';
        string token = "";
        private StreamWriter asmSw;
        private StreamReader tacSr;
        private SymTab st;
        bool ax = false;

        public Assembler(string path)
        {
            this.path = path;
            
            string[] asmPath = path.Split('.');
            aPath = asmPath[0] + ".asm";

            //Create .asm file
            if (File.Exists(aPath))
                File.Delete(aPath); //Delete existing asm file

            emit("\t.model small");
            emit("\t.586");
            emit("\t.stack 100h");
            emit("\t.data");
        }

        public Assembler(string path, rdp rdp) : this(path)
        {
            this.rdp = rdp;
            
        }

        public Assembler(string path, rdp rdp , StreamReader tacSr) : this(path, rdp)
        {
            this.tacSr = tacSr;
        }

        public Assembler(string path, rdp rdp, StreamReader tacSr, SymTab st) : this(path, rdp, tacSr)
        {
            this.st = st;
        }

        private void emit(string code)
        {
            //if (visual)
            // Console.Write(code +"\n");
            // using (StreamWriter sw = new StreamWriter(path, true))

            using (StreamWriter asmSw = new StreamWriter(aPath, true))
                asmSw.Write(code + "\n");

            code = "";
            

        }

        public  void genAssembly()
        {

            while(!tacSr.EndOfStream)
            insertProcedure();

            /* while (!tacSr.EndOfStream )
            {

                token = getNextToken();
            }*/
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


            token = "";
            while (!tacSr.EndOfStream)
            {
                
                char ch = peekNextChar();

                if (!Char.IsWhiteSpace(ch))
                {
                    //If peek was successful, get next char
                    ch = getNextChar();
                    token = processToken(); //Process the token
                    Console.WriteLine(token);
                    return token;

                }

                if (ch == 10 || Char.IsWhiteSpace(ch))
                {
                    ch = getNextChar();
                }
                else
                {
                    Console.WriteLine(token);
                    return token;
                }
            }
            return token;
        }
        private void match()
        {
                
            //Console.WriteLine("MATCHED " + desiredToken + " AND " + token.token);
            token = getNextToken();     
        }
        public string processToken( )
        {

            token = token + ch;

            while (tacSr.Peek() > -1) // read the rest of the lexeme
            {
                char c = peekNextChar();
                //idt can be letters, underscore and/or digits

                if (tacSr.Peek() == 32 || tacSr.Peek() == 10)
                {
                    break;
                }
                else
                {
                    ch = getNextChar();
                    token = token+ ch;
                }

            }//end while

            return token;
        }


        private void insertProcedure()
        {
            
            
            token = getNextToken(); // proc
            token = getNextToken(); // proc name
            Console.WriteLine("LEXEME " + token);
            SymTab.entry ptr = st.lookUp(token);
            Console.WriteLine("LEXEME "+ptr.lexeme);
            emit(token + "\t PROC");
            emit("\tpush bp ");  //push bp
            mov("bp", "sp"); // mov bp, sp
            SymTab.entry.function fptr = ptr as SymTab.entry.function;
            emit("\tsub sp, " + fptr.sizeOfLocal.ToString()); // sub sp, size of locals from symtab

            //TRANSLATED CODE
            emit("");
            transCode();


            //Last part of template
            emit("\tadd sp, " + fptr.sizeOfLocal.ToString());
            emit("\tpop bp");
            emit("\tret " + fptr.sizeOfParams);

           // token = getNextToken();
            //token = getNextToken(); // proc
            token = getNextToken(); // proc name
            emit(token + "\t endp");

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


        private void transCode()
        {
            L();
        }

        private bool R()
        {
            token = getNextToken(); // get R

            switch(token[0])
            {
                //BP
                case ('_'):
                    //addCode();
                    break;
                case ('@'):
                    break;
                default:
                    if(char.IsDigit(token[0]))
                    {
                        mov("ax", token);
                  
                        L();
                        return true;
                    }

                    break;
            }

            return false;
        }

        private bool L()
        {
            token = getNextToken(); // get L 
            
            switch (token[0])
            {
                //BP
                case ('_'):
                    //addCode();
                    token = getNextToken(); // get = 
                    ax = R();
                    if(ax == true)
                    {
                        ax = false;
                        token = token.Substring(1, token.Length-1);
                        //token = getNextToken();
                        mov("[" + token + "]", "ax");
                    }

                    break;

                case ('@'):
                    break;

                default:

                    switch (token)
                    {
                        case ("wri"):
                            break;
                        case ("ws"):
                            break;
                        case ("wrln"):
                            break;
                        case ("rdi"):
                            break;
                        case ("rds"):
                            break;
                        case ("call"):
                            break;


                        default:
                            Console.WriteLine("Second switch -- variable?");
                            break;
                    }
                    break;
            }
            return false;
        }

        private void addCode()
        {
            //Trim underscore if underscore

        }
    }//End class assembler
}//end namespace
