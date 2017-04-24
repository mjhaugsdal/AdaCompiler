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

        private StreamReader tacSr;
        private SymTab st;


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
            {
                insertProcedure();

                if(token == rdp.mainProc)
                {
                    //GOTO END
                    generateMain();
                    break;
                }
            }
            



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
                   // Console.WriteLine(token);
                    return token;

                }

                if (ch == 10 || Char.IsWhiteSpace(ch))
                {
                    ch = getNextChar();
                }
                else
                {
                    //Console.WriteLine(token);
                    return token;
                }
            }
            return token;
        }

        public string processToken( )
        {

            token = token + ch;
            if (tacSr.Peek() == 43 || tacSr.Peek() == 42)
            {
                return token;
            }


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

            SymTab.entry ptr = st.lookUp(token.Trim(' '));

            emit(token + "\t PROC");
            emit("\tpush bp ");  //push bp
            mov("bp", "sp"); // mov bp, sp
            SymTab.entry.function fptr = ptr as SymTab.entry.function;
            emit("\tsub sp, " + fptr.sizeOfLocal.ToString()); // sub sp, size of locals from symtab

            //TRANSLATED CODE
            emit("");
            if(token != "endp")
                token = getNextToken(); // get L 
            transCode();


            //Last part of template
            emit("\tadd sp, " + fptr.sizeOfLocal.ToString());
            emit("\tpop bp");
            emit("\tret " + fptr.sizeOfParams);

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
            emit("\tend main");
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
            emit("\tmov\t" + r1 + "," +  r2);
        }
        

        private void transCode()
        {
            
            L();
            ct();
           
        }

        private void ct()
        {
            if(token != "endp")
            {
                
                transCode();
               // ct();
            }
            else
            {
                
            }
        }

        private void R(string l)
        {
            token = getNextToken(); // get R
            char c = l[0];

            switch(c)
            {
                case ('@'):

                    //De-reference
                    token = trim(token);


                    l = trim(l);
                    if (l[0] == '_')
                        l = trim(l);


                    mov("ax", "[" + token + "]");
                   // l = trim(l);
                    token = getNextToken();
                    rTail();
                    mov("bx", "[" + l + "]");
                    mov("[bx]", "ax");

                    break;

                default:



                    if(char.IsDigit(c))
                    {
                        mov("heey", "lool");

                        break;
                    }

                    switch (token[0])
                    {

                        case ('-'):
                            //Unary negation?
                            //extracting two tokens ignores it for now!
                            token = getNextToken();
                            token = getNextToken();

                            break;
                        //BP
                        case ('_'):
                            token = trim(token);
                            mov("ax", "[" + token + "]");

                            if(l[0] == '_')
                            {
                                l = trim(l);

                                mov("[" + l + "]", "ax");
                            }
                            else
                            {
                                mov( l , "ax");
                            }
                            token = getNextToken();
                            rTail();
                            //mov("[" + l + "]", " ax");


                            break;
                        case ('@'):
                            //De-reference

                            token = trim(token);
                            if (l[0] == '_')
                                l = trim(l);
                            mov("ax", "[" + trim(token) + "]");
                           // l = trim(l);
                            token = getNextToken();
                            rTail();
                            mov("bx", "[" + l + "]");
                            mov("[ bx ]", "ax");

                            //    mov("[" + l + "]", " ax");

                            break;
                        default:
                            if (char.IsDigit(token[0]))
                            {
                                mov("ax", token);
                                l = trim(l);
                                token = getNextToken();
                                // token = getNextToken();
                                rTail();
                                mov("[" + l + "]", "ax");

                            }
                            else
                            {
                                if (l[0] == '_')
                                    l = trim(l);
                                mov("ax ", token);
                                //l = trim(l);
                                token = getNextToken();
                                // token = getNextToken();
                                rTail();
                                mov("[" + l + "]", "ax");
                                //token = getNextToken();
                            }


                            break;
                    }
                    break;

            }


        }


        private string trim(string s)
        {
            s = s.Substring(1, s.Length - 1);
            return s;
        }

        private void rTail()
        {

            
            //ch = getNextChar();
            switch (token[0])
            {
                case ('+'):
                   

                    token = getNextToken();
                    //  token = getNextToken();
                    // token = trim(token);
                    //mov("bx ", "[" + trim(token) + "]");

                    SymTab.entry ptr = st.lookUp(token);
                    char c = token[0];
                    if(char.IsDigit(c))
                    {

                    }
                    else if(ptr.token == lexicalScanner.SYMBOL.unkownt)
                    {
                        if (token[0] == '@')
                        {
                            if(token[0] == '_')
                                emit("\tadd ax, " + "[" +trim(trim(token)) + "]");
                            else
                                emit("\tadd ax, " + "[" + token + "]");
                        }
                        else
                        {
                            if (token[0] == '_')
                                emit("\tadd ax," + "[" + trim(token) + "]");
                            else
                                emit("\tadd ax, " + "[" + token + "]");
                        }
                    }
                    else
                    {
                        if (token[0] == '@')
                        {
                            if (token[0] == '_')
                                emit("\tadd ax, " +  trim(trim(token)) );
                            else
                                emit("\tadd ax, " +  token );
                        }
                        else
                        {
                            if (token[0] == '_')
                                emit("\tadd ax," + trim(token) );
                            else
                                emit("\tadd ax, " + token );
                        }
                    }

                    token = getNextToken();
                    break;
                case ('*'):
                    
                    token = getNextToken();
                    token = getNextToken();
                    token = trim(token);
                    mov("bx ", "[" + trim(token) + "]");
                    emit("\timul " + "bx");

                    break;
                default:
                    break;
            }
        }

        private void L()
        {
            
            string l;
            switch (token[0])
            {
                //BP
                case ('_'):
                    l = token;
                    token = getNextToken(); // get = 
                    R(l);
                    emit("");

                    break;

                case ('@'):
                    
                    l = token;
                    token = getNextToken(); // get = 
                    R(l);

                    emit("");

                    break;


                
                default:

                    switch (token)
                    {



                        case ("wri"):

                            token = getNextToken();

                            //If pushing a number
                            if (char.IsDigit(token[0]))
                            {

                            }
                            //Else a variable
                            else
                            {

                                if (token[0] == '_')
                                    mov("ax",  trim(token) );
                                else
                                    mov("ax",  token );
                      

                            }

                            emit("\tcall writeint");
                            token = getNextToken();

                            break;
                        case ("wrs"):


                            break;
                        case ("wrln"):
                            token = getNextToken();

                            break;
                        case ("rdi"):
                            break;
                        case ("rds"):
                            break;
                        case ("push"):
                            token = getNextToken();

                            if (token[0] == '@') // De reference?   
                            {
                                token = trim(token);
                                if (token[0] == '_')
                                    token = trim(token);
                                mov("ax", "offset " + token );
                               // mov("ax", "offset " + token);
                                emit("\tpush ax \n");

                            }
                            else
                            {

                                if (token[0] == '_')
                                    token = trim(token);

                                mov("ax", "offset " + token);
                                emit("\tpush ax \n");

                                //token = getNextToken();

                                //Console.WriteLine(token);
                                //token = getNextToken();
                            }

                            token = getNextToken();
                            break;
                        case ("call"):

                            token = getNextToken();
                            emit("\tcall " + token);

                            token = getNextToken();
                            break;

                        case ("proc"):
                            break;
                        case ("endp"):
                            break;

                        default:
                            SymTab.entry tmp = st.lookUp(token);

                            if (tmp.token == lexicalScanner.SYMBOL.unkownt)
                            {
                                token = getNextToken();
                            }
                            else
                            {
                                l = token;
                                token = getNextToken(); // get = 
                                
                                R(l);
                                emit("");
                                //Console.WriteLine("Second switch -- variable?");
                                //  if(token != "proc" || token != "endp")   

                            }
                            break;
                    }
                    break;
            }
            
        }

        private void addCode()
        {
            //Trim underscore if underscore

        }
    }//End class assembler
}//end namespace
