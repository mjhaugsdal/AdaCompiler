/// <summary>
/// Name: Markus Johan Haugsdal
/// Class: CSC 446 Compiler Construction
/// Assignment: 
/// Due Date: 04.04.2017
/// Instructor: Hamer
/// 
/// Description: Recursive Descent Parser 5
/// 
/// </summary>


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    class rdp
    {
        bool visual = true;
        public string mainProc;
        bool procSet = false;
        public static bool error = false;
        public int depth = 1;
        public int tempVarNr = 1;
        private lexicalScanner.Token token;
        private lexicalScanner lx;
        private StreamReader sr;
        private SymTab st;
      //  private FileStream fs;
     //   private StreamWriter sw;
        private string path;

        public rdp(lexicalScanner.Token token)
        {
            this.token = token;
        }

        public rdp(lexicalScanner.Token token, lexicalScanner lx) : this(token)
        {
            this.lx = lx;
        }

        public rdp(lexicalScanner.Token token, lexicalScanner lx, StreamReader sr) : this(token, lx)
        {
            this.sr = sr;
        }

        public rdp(lexicalScanner.Token token, lexicalScanner lx, StreamReader sr, SymTab st) : this(token, lx, sr)
        {
            this.st = st;
        }

        public rdp(lexicalScanner.Token token, lexicalScanner lx, StreamReader sr, SymTab st, string path) : this(token, lx, sr, st)
        {
            this.path = path;
        }




        //Parse
        internal lexicalScanner.Token parse(lexicalScanner.Token firstToken)
        {
            
            //Sets the first token
            token.token = firstToken.token;
            while (token.token == lexicalScanner.SYMBOL.commentt)
            {
                token = lx.getNextToken();
            }

            //Got the tokens!
            /* START FOLLOWING THE GRAMMAR RULES!
             * 
             * prog
             * ->
             * proct, idt, Args, is
             * DeclerativePart
             * Procedures
             * begin
             * SeqOfStatements
             * end, idt.
             * 
             * NOTE: Changes will come after friday the 10th
             */
            //proct

            match(lexicalScanner.SYMBOL.proct);
            string procName = token.lexeme;
            if(procSet == false)
            {
                mainProc = token.lexeme;
                procSet = true;
            }


            

            //{ 1 }
            checkForDups();
            st.insert(token.lexeme, token.token, depth);
            SymTab.entry ptr = new SymTab.entry();
            ptr = st.lookUp(token.lexeme);
            match(lexicalScanner.SYMBOL.idt);

            
            //Create f for functionEntry
            SymTab.entry.function f = new SymTab.entry.function();
            
            f.lexeme = ptr.lexeme;
            f.depth = ptr.depth;
            f.token = ptr.token;
            depth++;

            //Args
            if (error != true)
                pArgs(ref f);
            //is

            match(lexicalScanner.SYMBOL.ist);
            

            //DeclarativePart
            int offset = 0;
            int oldOffset = 2;
            if (error != true)
                declPart(ref f, ref offset, ref oldOffset);
            //Procedures

     
            if (error != true)
                procedures();
            //Match begint
            match(lexicalScanner.SYMBOL.begint);
            //Sequence of statements.
            emit("proc " + procName + "\n");


            if (error != true)
                seqOfStatements(offset);
            //{ 3 }

 

            insertFunction(f);
            emit("endp " + procName + "\n");
            match(lexicalScanner.SYMBOL.endt);

            if (token.lexeme != procName && error != true)
            {
                Console.WriteLine("ERROR! PROCEDURE " + procName + " DID NOT MATCH AT END " + token.lexeme);
                error = true;
            }


            match(lexicalScanner.SYMBOL.idt);
            match(lexicalScanner.SYMBOL.semicolont);
           // st.writeTable(depth);
            st.deleteDepth(depth);
            depth--;
  
            return token;

        }
        /// <summary>
        /// Inserts the args of the procedure
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="typ"></param>
        /// <param name="eTyp"></param>
        /// <param name="offset"></param>
        private void insertArg(SymTab.entry ptr, SymTab.varType typ, SymTab.entryType eTyp, ref int offset, lexicalScanner.SYMBOL mode)
        {
            
           
            switch (eTyp)
            {
                case (SymTab.entryType.constEntry):
                 

                    if (typ == SymTab.varType.floatType)
                    {
                        SymTab.entry.constant.floatConstant ce = new SymTab.entry.constant.floatConstant();
                        ce.lexeme = ptr.lexeme;
                        ce.token = ptr.token;
                        ce.depth = ptr.depth;
                        ce.typeOfConstant = typ;
                        ce.typeOfEntry = eTyp;
                        ce.size = 4;

                        //ce.offset = offset + ce.size;
                        //offset = offset + ce.size;

                        ce.offset = offset;
                        offset = offset + ce.size;


                        if (ptr.Next != null)
                        {
                            ce.Prev = ptr.Prev;
                            ce.Next = ptr.Next;
                            ptr.Prev.Next = ce;
                            ce.Next.Prev = ce;
                        }
                        else
                        {
                            ce.Prev = ptr.Prev;
                            ptr.Prev.Next = ce;
                        }

                    }

                    else if (typ == SymTab.varType.intType)
                    {
                     
                        SymTab.entry.constant.intConstant ce = new SymTab.entry.constant.intConstant();
                        ce.lexeme = ptr.lexeme;
                        ce.token = ptr.token;
                        ce.depth = ptr.depth;
                        ce.typeOfConstant = typ;
                        ce.typeOfEntry = eTyp;
                        ce.size = 2;

                        ce.offset = offset ;
                        offset = offset + ce.size;


                        if (ptr.Next != null)
                        {
                            ce.Prev = ptr.Prev;
                            ce.Next = ptr.Next;
                            ptr.Prev.Next = ce;
                            ce.Next.Prev = ce;
                        }
                        else
                        {
                            ce.Prev = ptr.Prev;
                            ptr.Prev.Next = ce;
                        }
                    }
                    break;
                case (SymTab.entryType.varEntry):
                    SymTab.entry.var v = new SymTab.entry.var();
                    v.lexeme = ptr.lexeme;
                    v.token = ptr.token;
                    v.depth = ptr.depth;
                    v.typeOfVar = typ;
                    v.typeOfEntry = eTyp;
                    v.isParameter = true;
                    v.mode = mode;

                    if (typ == SymTab.varType.floatType)
                    {
                        v.size = 4;


                        v.offset = offset;
                        offset = offset + v.size;
                    }

                    else if (typ == SymTab.varType.intType)
                    {
                        v.size = 2;
                        v.offset = offset;
                        offset = offset + v.size;

                    }
                    else if(typ == SymTab.varType.charType)
                    {
                        v.size = 1;
                        v.offset = offset;
                        offset = offset + v.size;

                    }


                    if (ptr.Next != null)
                    {
                        v.Prev = ptr.Prev;
                        v.Next = ptr.Next;
                        ptr.Prev.Next = v;
                        v.Next.Prev = v;
                    }
                    else
                    {
                        v.Prev = ptr.Prev;
                        ptr.Prev.Next = v;

                    }
                    break;
            }
        }
        /// <summary>
        /// Inserts the procedure to the symbolTable
        /// </summary>
        /// <param name="f"></param>
        private void insertFunction(SymTab.entry.function f)
        {
            SymTab.entry eptr;
            eptr = st.lookUp(f.lexeme);

   
            f.typeOfEntry = SymTab.entryType.functionEntry;
           
            f.numberOfParams = f.paramList.Count;
        
            if (eptr.Next != null)
            {
              
                f.Prev = eptr.Prev;
                f.Next = eptr.Next;
                eptr.Prev.Next = f;
                f.Next.Prev = f;
            }
            else
            {
                
                f.Prev = eptr.Prev;
               
                eptr.Prev.Next = f;
              
            }
            
        }
        /// <summary>
        /// Additional grammar for Sequence of Statements.
        /// </summary>
        private void seqOfStatements(int offset)
        {
            switch(token.token)
            {
                case (lexicalScanner.SYMBOL.idt):

                    if(error != true)
                        statement(ref offset);
                    match(lexicalScanner.SYMBOL.semicolont);
                    if (error != true)
                        statTail(ref offset);

                    break;
                default:
                    //Lambda
                    break;

            }
        
        }
        /// <summary>
        /// Tail for statements
        /// </summary>
        private void statTail(ref int offset)
        {
            switch (token.token)
            {
                case (lexicalScanner.SYMBOL.idt):

                    if (error != true)
                        statement(ref offset);
                    match(lexicalScanner.SYMBOL.semicolont);
                    if (error != true)
                        statTail(ref offset);

                    break;
                default:
                    //Lambda
                    break;
            }
        }
        /// <summary>
        /// Statement -> Assignstat
        /// </summary>
        private void statement(ref int offset)
        {
            switch (token.token)
            {
                case (lexicalScanner.SYMBOL.idt):
                    if (error != true)
                        AssignStat(ref offset);
                    break;
                default:
                    if (error != true)
                        IOStat();
                    break;
            }
        }


        private void IOStat()
        {
            //Lambda, for now
        }

        /// <summary>
        /// A-> idt := Expression
        /// </summary>
        private void AssignStat(ref int offset)
        {

            switch(token.token)
            {
                case (lexicalScanner.SYMBOL.idt):
                    //Check for undeclared variable!
                    if (error != true)
                        checkUndeclared(token.lexeme);
                    //Start building code string
                    string code = null;
                 
                    SymTab.entry idtPtr = st.lookUp(token.lexeme); // IdtPtr for left side of statement
                    SymTab.entry.var vPtr = idtPtr as SymTab.entry.var;
                    SymTab.entry e_syn = new SymTab.entry();       // e_syn for right side of statement

                    match(lexicalScanner.SYMBOL.idt);
                    bool stat = true;
                    statOrProc(ref stat, idtPtr, ref e_syn, ref offset); // E (SYN)   
                    if(stat == true)
                    { 

                        if(depth == 2)
                        {
                            code = string.Concat(code, vPtr.lexeme);
                            code = string.Concat(code, "\t=\t");
                            code = string.Concat(code, e_syn.lexeme );
                        }
                    
                        else
                        {
                            e_syn = st.lookUp(e_syn.lexeme);
                            SymTab.entry.var ePtr = e_syn as SymTab.entry.var;
                            
                           // Console.Write(ePtr.offset);
                      
                            if(vPtr.isParameter == true)
                            {
                                if(vPtr.mode == lexicalScanner.SYMBOL.outt)
                                    code = string.Concat(code, "@_bp+" + vPtr.offset);
                                else
                                    code = string.Concat(code, "_bp+" + vPtr.offset);


                                code = string.Concat(code, "\t=\t");


                                if (e_syn.isParameter == true)
                                {
                                    if(ePtr.mode == lexicalScanner.SYMBOL.outt)

                                        code = string.Concat(code, "@_bp+" + ePtr.offset);
                                    else
                                        code = string.Concat(code, "_bp+" + ePtr.offset);

                                }
                                   
                                else
                                    code = string.Concat(code, "_bp-" + ePtr.offset);

                            }
                            else
                            {
                                code = string.Concat(code, "_bp-" + vPtr.offset);
                                code = string.Concat(code, "\t=\t");

                                if(ePtr.isParameter)
                                {
                                    if(ePtr.mode == lexicalScanner.SYMBOL.outt)
                                        code = string.Concat(code, "@_bp+" + ePtr.offset);
                                    else
                                        code = string.Concat(code, "_bp+" + ePtr.offset);
                                }
                                    
                                else
                                    code = string.Concat(code, "_bp-" + ePtr.offset );
                            }
                        }
                    
                        emit(code + "\n");

                    }

                    break;
                default:
                    Console.WriteLine("ASSIGNSTAT: ERROR ON LINE: " + lexicalScanner.ln);
                    error = true;
                    break;
            }

        }


        private void statOrProc(ref bool stat, SymTab.entry idtPtr, ref SymTab.entry e_syn, ref int offset)
        {
            switch(token.token)
            {
                case (lexicalScanner.SYMBOL.lparent):
                    match(lexicalScanner.SYMBOL.lparent);
                    stat = false;
                    SymTab.entry.function fPtr = idtPtr as SymTab.entry.function;
                    int i = 0;
                    if (error!=true)
                        parameters(fPtr.paramList, i);                       //PARAMETERS -> PROCEDURE CALL

                    emit("call " + idtPtr.lexeme+ "\n");
                    match(lexicalScanner.SYMBOL.rparent);
                    break;
                case (lexicalScanner.SYMBOL.assignopt):
                    stat = true;
                    match(lexicalScanner.SYMBOL.assignopt); // ASSIGNOPT -> EXPRESSION
                    if (error != true)
                        Expr(ref e_syn, ref offset);
                    break;
                default:
                    Console.WriteLine("STATORPROC: ERROR ON LINE: " + lexicalScanner.ln);
                    break;
            }
        }

        private void parameters(LinkedList<SymTab.paramNode> ll, int i)
        {


            switch (token.token)
            {
                case (lexicalScanner.SYMBOL.idt):
                    if (error != true)
                        checkUndeclared(token.lexeme);

                    SymTab.paramNode node = ll.ElementAt(i);
                    i++;

                    if (node.mode == lexicalScanner.SYMBOL.outt)
                        emit("push " + "@"+ token.lexeme +"\n");
                    else
                        emit("push "  + token.lexeme + "\n");

                    //CHECK PROCEDURE IN SYMTAB FOR INOUT OR OUT MODE
                    match(lexicalScanner.SYMBOL.idt);
                    if (error != true)
                        parametersTail(ll,i);
                    break;


                case (lexicalScanner.SYMBOL.numt):

                    emit("push " + token.lexeme + "\n");

                    match(lexicalScanner.SYMBOL.numt);
                    if (error != true)
                        parametersTail(ll,i);
                    break;
                default:
                   // Console.WriteLine("Lambda in parameters");
                    break;
            }
        }

        private void parametersTail(LinkedList<SymTab.paramNode> ll,int i)
        {
            switch (token.token)
            {
                case (lexicalScanner.SYMBOL.commat):
                    match(lexicalScanner.SYMBOL.commat);
                    if(error!=true)
                        parametersTailTail(ll,i);
                    break;
                default:
                    //Console.WriteLine("Lambda in ParametersTail");
                    break;
            }
            
           

        }

        private void parametersTailTail(LinkedList<SymTab.paramNode> ll,int i)
        {
            switch(token.token)
            {
                case (lexicalScanner.SYMBOL.idt):
                    if (error != true)
                        checkUndeclared(token.lexeme);
                    SymTab.paramNode node = ll.ElementAt(i);
                    i++;

                    if (node.mode == lexicalScanner.SYMBOL.outt)
                        emit("push " + "@" + token.lexeme + "\n");
                    else
                        emit("push " + token.lexeme + "\n");



                    match(lexicalScanner.SYMBOL.idt);
                    if (error != true)
                        parametersTail(ll,i);

                    break;
                case (lexicalScanner.SYMBOL.numt):
                    emit("push " + token.lexeme + "\n");
                    match(lexicalScanner.SYMBOL.numt);
                    if(error!=true)
                        parametersTail(ll,i);   
                    break;
                default:
                    error = true;
                    Console.WriteLine("ERROR IN TAIL TAIL");
                    break;
            }
        }

        /// <summary>
        /// Checks for undeclared variables
        /// </summary>
        /// <param name="lexeme"></param>
        private void checkUndeclared(string lexeme)
        {


            SymTab.entry ePtr = st.lookUp(lexeme);
            if(ePtr.lexeme != token.lexeme && error != true)
            {
                Console.WriteLine("ERROR! UNDECLARED VARIABLE " + token.lexeme +  " AT LINE "  + lexicalScanner.ln );
                error = true;
            }
            //If it is the right lexeme, the depth has to be LOWER
        /*    else if(ePtr.lexeme == token.lexeme && ePtr.depth > depth)
            {
                Console.WriteLine("ERROR! VARIABLE NOT DECLARED IN THIS SCOPE!" + token.lexeme + " SCOPE: " + depth );
            }*/
        }

        /// <summary>
        /// Expression -> Term, RestOfExpression
        /// </summary>
        private void Expr(ref SymTab.entry syn, ref int offset)
        {
            if (error != true)
                relation(ref syn, ref offset);
        }

        /// <summary>
        /// Rule to call Simple Expression
        /// </summary>
        /// <param name="syn"></param>
        private void relation(ref SymTab.entry syn, ref int offset)
        {
            if (error != true)
                simpleExpression(ref syn, ref offset);
        }
        /// <summary>
        /// SimpleExpression
        /// </summary>
        /// <param name="syn"></param>
        private void simpleExpression(ref SymTab.entry syn, ref int offset)
        {
            SymTab.entry tSyn = new SymTab.entry();
            


            if (error != true)
                term(ref tSyn, ref offset);
            if (error != true)
                moreTerm(ref tSyn,  ref offset);

           
            syn = tSyn; // Set syn to t_syn
        }

        /// <summary>
        /// Term -> Factor MoreFactor
        /// </summary>
        /// <param name="tSyn"></param>
        private void term(ref SymTab.entry tSyn, ref int offset)
        {

            if (error != true)
                factor(ref tSyn, ref offset);
            if (error != true)
                moreFactor(ref tSyn, ref offset);
        }

        /// <summary>
        /// MoreFactor -> Mulop Factor Morefactor | E
        /// </summary>
        /// <param name="tSyn"></param>
        private void moreFactor(ref SymTab.entry rVal, ref int offset)
        {
            SymTab.entry Tval = new SymTab.entry();
            SymTab.entry tmpPtr;
            string code = null;
                
            switch (token.token)
            {
                case (lexicalScanner.SYMBOL.multopt):


                    tmpPtr = newTemp(ref offset);

                    if (depth == 2)
                    {
                        
                        code = string.Concat(code, tmpPtr.lexeme);
                        code = string.Concat(code, "\t=\t");
                        code = string.Concat(code, rVal.lexeme);
                    }
                    else
                    {
                        tmpPtr = st.lookUp(tmpPtr.lexeme);
                        SymTab.entry.var tp = tmpPtr as SymTab.entry.var;
                        rVal = st.lookUp(rVal.lexeme);
                        SymTab.entry.var rp = rVal as SymTab.entry.var;

                        if (tp.isParameter)
                        {
                            if(tp.mode == lexicalScanner.SYMBOL.outt)
                                code = string.Concat(code, "@_bp+" + tp.offset);
                            else
                                code = string.Concat(code, "_bp+" + tp.offset);

                        }
                            
                        else
                            code = string.Concat(code, "_bp-" + tp.offset);


                        code = string.Concat(code, "\t=\t"); // Assign



                        if (rp.isParameter)
                        {
                            if (rp.mode == lexicalScanner.SYMBOL.outt)
                                code = string.Concat(code, "@_bp+" + rp.offset);
                            else
                                code = string.Concat(code, "_bp+" + rp.offset);
                        }
                            
                        else
                            code = string.Concat(code, "_bp-" + rp.offset);
                    }
                   
                    code = string.Concat(code, " " + token.lexeme + " "); //multopt

                    if (error != true)
                        mulop(); //Match mulopt
                       
                    if (error != true)
                        factor(ref Tval, ref offset);

                    //Try lookup
                    SymTab.entry tvalp = st.lookUp(Tval.lexeme);

                    if(tvalp.token != lexicalScanner.SYMBOL.unkownt)
                    {
                        if (depth == 2)
                        {

                            code = string.Concat(code, Tval.lexeme);
                        }

                        else
                        {
                            SymTab.entry.var tvalpp = tvalp as SymTab.entry.var;

                            if(tvalpp.isParameter)
                            {
                                if(tvalpp.mode == lexicalScanner.SYMBOL.outt)
                                    code = string.Concat(code, "@_bp+" + tvalpp.offset);
                                  else
                                    code = string.Concat(code, "_bp+" + tvalpp.offset);
                            }
                            
                            else
                                code = string.Concat(code, "_bp-" + tvalpp.offset);
                        }
                    }
                    else
                    {
                        code = string.Concat(code, Tval.lexeme);
                    }



                        

                    emit(code + "\n");
                    
                    if (error != true)
                        moreFactor(ref tmpPtr, ref offset);


                    rVal = tmpPtr;
                    break;

                default:
                    //Lambda
                    break;
            }


        }

        /// <summary>
        /// * | / | mod | rem | and
        /// </summary>
        private void mulop()
        {

            match(lexicalScanner.SYMBOL.multopt);
            //Lexeme contains specific operation
        }

        /// <summary>
        /// Factor -> Id | num | ( Expr ) | not Factor | SignOp Factor | 
        /// </summary>
        /// <param name="tSyn"></param>
        private void factor(ref SymTab.entry tSyn, ref int offset)
        {
            switch (token.token)
            { 
                case (lexicalScanner.SYMBOL.idt): // IDT

                    //Check for undeclared
                    if (error != true)
                        checkUndeclared(token.lexeme);

                    tSyn = st.lookUp(token.lexeme);
                    match(lexicalScanner.SYMBOL.idt);   

                    break;
                case (lexicalScanner.SYMBOL.numt): // Numt
                    tSyn.lexeme = token.lexeme;

                    match(lexicalScanner.SYMBOL.numt);
                    break;
                case (lexicalScanner.SYMBOL.lparent): // ( EXPR )
                    match(lexicalScanner.SYMBOL.lparent);
                    if(error!=true)
                        Expr(ref tSyn, ref offset);
                    match(lexicalScanner.SYMBOL.rparent);
                    break;
                case (lexicalScanner.SYMBOL.nott): // not Factor
                    match(lexicalScanner.SYMBOL.nott);
                    if (error != true)
                        factor(ref tSyn, ref offset);
                    break;
                case (lexicalScanner.SYMBOL.addopt): // SignOp Facto
                    if (error != true)
                        signOp(ref tSyn);
                    if (error != true)
                        factor(ref tSyn, ref offset);
                    break;
                default:
                    //Lambda NOT allowed
                    Console.WriteLine("ERROR! ILLEGAL SYMBOL " + token.lexeme + " AT LINE " + lexicalScanner.ln);
                    error = true;
                    break;
            }

        }


        /// <summary>
        /// - Checks for - Sign. Sets error flag for other addopt's
        /// </summary>
        /// <param name="tSyn"></param>
        private void signOp(ref SymTab.entry tSyn)
        {
            if(token.lexeme == "-" && token.token == lexicalScanner.SYMBOL.addopt)
            {
                match(lexicalScanner.SYMBOL.addopt);
            }
            else
            {
                error = true;
                Console.WriteLine("ERROR! \"" + token.lexeme + "\" NOT ALLOWED ON LINE " + lexicalScanner.ln + ". CHECK YOUR STATEMENT. (Perhaps you have two additions?)");
            }
           //Check if addopt's lexeme == a minus sign
        }

        /// <summary>
        /// MoreTerm -> Addop Term MoreTerm | E
        /// </summary>
        /// <param name="tSyn"></param>
        private void moreTerm(ref SymTab.entry rVal, ref int offset)
        {
            SymTab.entry Tval = new SymTab.entry();
            SymTab.entry tmpPtr;
            string code = null;

            switch (token.token)
            {
                case (lexicalScanner.SYMBOL.addopt):


                    tmpPtr = newTemp(ref offset);
                    //SymTab.entry.var v = rVal as SymTab.entry.var;
                    
                    if (depth == 2)
                    {
                        code = string.Concat(code, tmpPtr.lexeme);
                        code = string.Concat(code, "\t=\t");
                        code = string.Concat(code, rVal.lexeme);
                    }

                    else
                    {
                        rVal = st.lookUp(rVal.lexeme);
                        SymTab.entry.var v = rVal as SymTab.entry.var;
                        tmpPtr = st.lookUp(tmpPtr.lexeme);
                        SymTab.entry.var tptr = tmpPtr as SymTab.entry.var;

                        if (tptr.isParameter == true)
                        {
                            if(tptr.mode == lexicalScanner.SYMBOL.outt)
                            
                                code = string.Concat(code, "@_bp+" + tptr.offset);
                            else
                                code = string.Concat(code, "_bp+" + tptr.offset);


                            code = string.Concat(code, "\t=\t");


                            if (v.isParameter == true)
                            {
                                if(tptr.mode == lexicalScanner.SYMBOL.outt)
                                    code = string.Concat(code, "@_bp+" + v.offset);
                                else
                                    code = string.Concat(code, "_bp+" + v.offset);

                            }
                               
                            else
                                code = string.Concat(code, "_bp-" + v.offset);

                        }
                        else
                        
                        code = string.Concat(code, "_bp-" + tptr.offset);
                        code = string.Concat(code, "\t=\t");

                        if (v.isParameter)
                        {
                            if(v.mode == lexicalScanner.SYMBOL.outt)
                                code = string.Concat(code, "@_bp+" + v.offset);
                            else
                                code = string.Concat(code, "_bp+" + v.offset);

                        }
                            
                        else
                            code = string.Concat(code, "_bp-" + v.offset);
                    }


                    //Console.Write("HERE" + token.lexeme);
                    code = string.Concat(code, " "+token.lexeme+" "); //addop
                    if (error != true)
                        addOp(); //Match addop

                    if (error != true)
                        factor(ref Tval, ref offset);

                    //code = string.Concat(code, Tval.lexeme);

                    //Try lookup
                    SymTab.entry tvalp = st.lookUp(Tval.lexeme);

                    if (tvalp.token != lexicalScanner.SYMBOL.unkownt)
                    {
                        if (depth == 2)
                        {
                            SymTab.entry.var tvalVar = Tval as SymTab.entry.var;
                            if(tvalVar.mode == lexicalScanner.SYMBOL.outt)
                                code = string.Concat(code, "@"+Tval.lexeme);
                            else
                                code = string.Concat(code, Tval.lexeme);
                        }

                        else
                        {
                            SymTab.entry.var tvalpp = tvalp as SymTab.entry.var;

                            if (tvalpp.isParameter)
                            {
                                if(tvalpp.mode == lexicalScanner.SYMBOL.outt)
                                    code = string.Concat(code, "@_bp+" + tvalpp.offset);
                                else
                                    code = string.Concat(code, "_bp+" + tvalpp.offset);
                            }
 
                            else
                                code = string.Concat(code, "_bp-" + tvalpp.offset);
                        }
                    }
                    else
                    {
                        code = string.Concat(code, Tval.lexeme);
                    }

                    emit(code + "\n");

                    if (error != true)
                        moreFactor(ref tmpPtr, ref offset);

                    rVal = tmpPtr;
                   
                    break;
                default:

                    //Lambda allowed

                    


                    tmpPtr = newTemp(ref offset);
                    tmpPtr = st.lookUp(tmpPtr.lexeme);
                    SymTab.entry.var tp = tmpPtr as SymTab.entry.var;
                    SymTab.entry tempRval = st.lookUp(rVal.lexeme);

                    if (tempRval.token != lexicalScanner.SYMBOL.unkownt)
                        rVal = tempRval;

                    SymTab.entry.var rp = rVal as SymTab.entry.var;

                    if (depth == 2)
                    {
                        if(tp.mode == lexicalScanner.SYMBOL.outt)
                            code = string.Concat(code, "@"+tmpPtr.lexeme);
                        else
                            code = string.Concat(code, tmpPtr.lexeme);

                        code = string.Concat(code, "\t=\t");
                        if(rp.mode == lexicalScanner.SYMBOL.outt)
                            code = string.Concat(code, "@"+rVal.lexeme);
                        else
                            code = string.Concat(code, rVal.lexeme);
                    }           
                    else
                    {
                        if (tp.isParameter)
                        {
                            if (tp.mode == lexicalScanner.SYMBOL.outt)
                                code = string.Concat(code, "@_bp+" + tp.offset);
                            else
                                code = string.Concat(code, "_bp+" + tp.offset);


                            code = string.Concat(code, "\t=\t");
                            if(rp.mode == lexicalScanner.SYMBOL.outt)
                                code = string.Concat(code, "@"+rVal.lexeme);
                            else
                                code = string.Concat(code, "@" + rVal.lexeme);
                        }

                        else
                        {
                            code = string.Concat(code, "_bp-" + tp.offset);
                            code = string.Concat(code, "\t=\t");
                            code = string.Concat(code, rVal.lexeme);
                        }


                    }

                    emit(code + "\n");
                    rVal = tmpPtr;

                    break;

            }

        }
/*
        private string processTwoPointers(SymTab.entry tmpPtr, SymTab.entry rVal, string code)
        {

            tmpPtr = st.lookUp(tmpPtr.lexeme);
            SymTab.entry.var tp = tmpPtr as SymTab.entry.var;
            rVal = st.lookUp(rVal.lexeme);

            if (depth == 2)
            {
                

                code = string.Concat(code, tmpPtr.lexeme);
                code = string.Concat(code, "\t=\t");
                code = string.Concat(code, rVal.lexeme);
            }
            else
            {



                if (tp.isParameter)
                {
                    if (tp.mode == lexicalScanner.SYMBOL.outt)
                        code = string.Concat(code, "@_bp+" + tp.offset);
                    else
                        code = string.Concat(code, "_bp+" + tp.offset);
                }

                else
                    code = string.Concat(code, "_bp-" + tp.offset);

                code = string.Concat(code, "\t=\t");
                code = string.Concat(code, rVal.lexeme);

            }

            return code;
        }
*/
        /*private void concatCode(string code, SymTab.entry left, SymTab.entry.var right)
{
   if (depth == 2)
   {
       code = string.Concat(code, left.lexeme);
       code = string.Concat(code, "\t=\t");
       code = string.Concat(code, right.lexeme);
   }

   else
   {
       left = st.lookUp(left.lexeme);
       SymTab.entry.var v = left as SymTab.entry.var;
       // Console.Write(ePtr.offset);

       if (v.isParameter == true)
       {

           code = string.Concat(code, "_bp+" + v.offset);
           code = string.Concat(code, "\t=\t");
           if (v.isParameter == true)
               code = string.Concat(code, "_bp+" + v.offset);
           else
               code = string.Concat(code, "_bp-" + v.offset);

       }
       else

           code = string.Concat(code, "_bp-" + v.offset);
       code = string.Concat(code, "\t=\t");

       if (v.isParameter)
           code = string.Concat(code, "_bp+" + v.offset);
       else
           code = string.Concat(code, "_bp-" + v.offset);
   }
}*/

        private SymTab.entry newTemp(ref int offset)
        {
            SymTab.entry temp = new SymTab.entry();
            

            temp.lexeme = "_t" + tempVarNr;
            tempVarNr++;

            st.insert(temp.lexeme, lexicalScanner.SYMBOL.idt, depth);
            temp = st.lookUp(temp.lexeme);
            
            insertVar(temp, SymTab.varType.intType, SymTab.entryType.varEntry, ref offset, 1, offset);
            

            return temp;

        }

        /// <summary>
        /// + | - | or
        /// </summary>
        /// <param name="tSyn"></param>
        private void addOp()
        {

            match(lexicalScanner.SYMBOL.addopt);
            //Lexeme contains specific operation.
        }

        /// <summary>
        /// Grammar rule for procedures
        /// </summary>
        private void procedures()
        {

            //-> Prog Procedures | e
            switch (token.token)
            {
                case (lexicalScanner.SYMBOL.proct):
                    if(error != true)
                        parse(token);
                    if (error != true)
                        procedures();
                    break;
                default:
                    //Lambda / empty
                    break;
            }

        }

        /// <summary>
        /// Declarative Part grammar rule
        /// </summary>
        /// <param name="f"></param>
        /// <param name="offset"></param>
        /// <param name="oldOffset"></param>
        private void declPart(ref SymTab.entry.function f, ref int offset, ref int oldOffset   )
        {
            switch(token.token)
            {

                case (lexicalScanner.SYMBOL.idt):

                    // { 1 }
                    
                    if (error != true)
                    {
                        vars(ref offset, ref oldOffset);
                    }
                    oldOffset = offset+2;
                  
                    match(lexicalScanner.SYMBOL.semicolont);
                    if (error != true)
                        declPart(ref f,  ref offset, ref oldOffset);

                    f.sizeOfLocal = offset;
                    break;
                default:
                    //Lambda / empty
                    break;
            }
        }
        /// <summary>
        /// Variables grammar rule
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="oldOffset"></param>
        private void vars(ref int offset, ref int oldOffset)
        {
            // { 1 }  

            int counter = 0;
            checkForDups();
            st.insert(token.lexeme, token.token, depth);
            SymTab.entry ptr = st.lookUp(token.lexeme);
            SymTab.varType typ = new SymTab.varType();
            SymTab.entryType eTyp = new SymTab.entryType();

            match(lexicalScanner.SYMBOL.idt);
            if (error != true)
                varsTail( ref typ, ref eTyp, ref offset,  counter, oldOffset);
            //{ 3 }



            insertVar(ptr, typ, eTyp, ref offset,  counter, oldOffset);
        }

        private void varsTail(ref SymTab.varType typ, ref SymTab.entryType eTyp ,  ref int offset,  int counter, int oldOffset)
        {
            switch(token.token)
            {
                case (lexicalScanner.SYMBOL.commat):
                    match(lexicalScanner.SYMBOL.commat);
                    // { 1 } 
                    counter++;
                    checkForDups();
                    st.insert(token.lexeme, token.token, depth);
                    SymTab.entry ptr = st.lookUp(token.lexeme);

                    
                    match(lexicalScanner.SYMBOL.idt);
                    if (error != true)
                        varsTail(ref typ, ref eTyp, ref offset,  counter, oldOffset);

                    //{ 3 }
                    
                    insertVar(ptr, typ, eTyp, ref offset, counter, oldOffset);
                    break;

                case (lexicalScanner.SYMBOL.colont):
                    match(lexicalScanner.SYMBOL.colont);
                    if (error != true)
                        varType(ref offset, ref typ, ref eTyp);
                    break;
                
                   
                default:
                    break;
            }
        }
        /// <summary>
        /// Function for inserting variables to the symbol table
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="typ"></param>
        /// <param name="eTyp"></param>
        /// <param name="offset"></param>
        /// <param name="counter"></param>
        /// <param name="oldOffset"></param>
        private void insertVar(SymTab.entry ptr, SymTab.varType typ, SymTab.entryType eTyp, ref int offset, int counter, int oldOffset)
        {

            switch (eTyp)
            {
                case (SymTab.entryType.constEntry):


                    if (typ == SymTab.varType.floatType)
                    {
                        SymTab.entry.constant.floatConstant ce = new SymTab.entry.constant.floatConstant();
                        ce.lexeme = ptr.lexeme;
                        ce.token = ptr.token;
                        ce.depth = ptr.depth;
                        ce.typeOfConstant = typ;
                        ce.typeOfEntry = eTyp;
                        ce.size = 4;

                        ce.offset = (counter * ce.size + oldOffset);
                        offset = offset + ce.size;

                        

                        if (ptr.Next != null)
                        {
                            ce.Prev = ptr.Prev;
                            ce.Next = ptr.Next;
                            ptr.Prev.Next = ce;
                            ce.Next.Prev = ce;
                        }
                        else
                        {
                            ce.Prev = ptr.Prev;
                            ptr.Prev.Next = ce;
                        }

                    }

                    else if (typ == SymTab.varType.intType)
                    {

                        SymTab.entry.constant.intConstant ce = new SymTab.entry.constant.intConstant();
                        ce.lexeme = ptr.lexeme;
                        ce.token = ptr.token;
                        ce.depth = ptr.depth;
                        ce.typeOfConstant = typ;
                        ce.typeOfEntry = eTyp;
                        ce.size = 2;


                        ce.offset = (counter * ce.size + oldOffset);
                        offset = offset + ce.size;

                        if (ptr.Next != null)
                        {
                            ce.Prev = ptr.Prev;
                            ce.Next = ptr.Next;
                            ptr.Prev.Next = ce;
                            ce.Next.Prev = ce;
                        }
                        else
                        {
                            ce.Prev = ptr.Prev;
                            ptr.Prev.Next = ce;
                        }
                    }
                    break;


                case (SymTab.entryType.varEntry):
                    SymTab.entry.var v = new SymTab.entry.var();
                    v.lexeme = ptr.lexeme;
                    v.token = ptr.token;
                    v.depth = ptr.depth;
                    v.typeOfVar = typ;
                    v.typeOfEntry = eTyp;

                    if (typ == SymTab.varType.floatType)
                    {
                        v.size = 4;

                        v.offset = (counter * v.size + oldOffset);
                        offset = offset + v.size;

                    }

                    else if (typ == SymTab.varType.intType)
                    {
                        v.size = 2;

                        v.offset = (counter * v.size + oldOffset);
                        offset = offset + v.size;

            

                    }
                    else if (typ == SymTab.varType.charType)
                    {
                        v.size = 1;

                        v.offset = (counter * v.size + oldOffset);
                        offset = offset + v.size;
                    }


                    if (ptr.Next != null)
                    {
                        v.Prev = ptr.Prev;
                        v.Next = ptr.Next;
                        ptr.Prev.Next = v;
                        v.Next.Prev = v;
                    }
                    else
                    {
                        v.Prev = ptr.Prev;
                        ptr.Prev.Next = v;

                    }
                    break;
            }
        }
        private void typeMark(ref SymTab.varType typ, ref SymTab.entryType eTyp)
        {
            
            switch (token.token)
            {
                case (lexicalScanner.SYMBOL.integert):
                    
                    eTyp = SymTab.entryType.varEntry;
                    typ= SymTab.varType.intType;

                    match(lexicalScanner.SYMBOL.integert);
                    
                    break;
                case (lexicalScanner.SYMBOL.floatt):
                    eTyp  = SymTab.entryType.varEntry;
                    typ= SymTab.varType.floatType;

                    match(lexicalScanner.SYMBOL.floatt);
                
                    break;
               /* case (lexicalScanner.SYMBOL.realt):
                    match(lexicalScanner.SYMBOL.realt);
                    break;*/
                case (lexicalScanner.SYMBOL.chart):
                    eTyp  = SymTab.entryType.varEntry;
                    typ = SymTab.varType.charType;

                    match(lexicalScanner.SYMBOL.chart);
                  

                    break;
                case (lexicalScanner.SYMBOL.constt):

                    eTyp  = SymTab.entryType.constEntry;
                    match(lexicalScanner.SYMBOL.constt);
                    match(lexicalScanner.SYMBOL.assignopt);
                    

                    if (error != true)
                        value(ref typ);
                    
                    break;
            }

        }
        private void varType(ref int offset, ref SymTab.varType typ, ref SymTab.entryType eTyp)
        {

            switch (token.token)
            {
                case (lexicalScanner.SYMBOL.integert):
                    eTyp = SymTab.entryType.varEntry;
                    typ = SymTab.varType.intType;
                 
                    match(lexicalScanner.SYMBOL.integert);
                    break;
                case (lexicalScanner.SYMBOL.floatt):
                    eTyp = SymTab.entryType.varEntry;
                    typ = SymTab.varType.floatType;
                 
                    match(lexicalScanner.SYMBOL.floatt);
                     break;
                /* case (lexicalScanner.SYMBOL.realt):
                     match(lexicalScanner.SYMBOL.realt);
                     break;*/
                case (lexicalScanner.SYMBOL.chart):
                    eTyp = SymTab.entryType.varEntry;
                    typ = SymTab.varType.charType;
                
                    match(lexicalScanner.SYMBOL.chart);
                    break;
                case (lexicalScanner.SYMBOL.constt):
                    eTyp = SymTab.entryType.constEntry;
                    
                    match(lexicalScanner.SYMBOL.constt);
                    match(lexicalScanner.SYMBOL.assignopt);


                    if (error != true)
                        varValue(ref typ);

                    break;
            }

        }

        private void varValue(ref SymTab.varType typeOfVar)
        {
            //Numberical Literal
            if (token.valueR.HasValue)
            {
                //We have a float!
                typeOfVar = SymTab.varType.floatType;
            }
            else
            {
                typeOfVar = SymTab.varType.intType;
                //We have a integer!
            }
            match(lexicalScanner.SYMBOL.numt);
        }

        /// <summary>
        /// Checks if token has value (number token)
        /// </summary>
        private void value( ref SymTab.varType typeOfVar)
        {
            //Numberical Literal
            if(token.valueR.HasValue)
            {
                //We have a float!
                typeOfVar = SymTab.varType.floatType;
            }
            else
            {
                typeOfVar = SymTab.varType.intType;
                //We have a integer!
            }

            match(lexicalScanner.SYMBOL.numt);
            
        }

        /// <summary>
        /// Function for grammar rule Args -> ( ArgList ) | e
        /// </summary>
         private void pArgs(ref SymTab.entry.function f)
        {

            int offset = 4;
            switch (token.token)
            {
                
                case (lexicalScanner.SYMBOL.lparent):
                    match(lexicalScanner.SYMBOL.lparent);
                    //We have variables!
                    if (error != true)
                        argList(ref f, ref offset);
                    
                    match(lexicalScanner.SYMBOL.rparent);
                    break;
                default:
                    //empty 
                    break;
            }

        }

        private void argList(ref SymTab.entry.function f,  ref int offset)
        {

            lexicalScanner.SYMBOL paramMode = lexicalScanner.SYMBOL.intt;

            if (error != true)
                mode(ref paramMode);

            if (error != true)
            {
                procList(ref f.paramList, ref offset, ref paramMode);

            }

            f.sizeOfParams = offset-4;
            //if (error != true)
              //  moreArgs(ref f, ref offset);


        }

        private void moreArgs(ref SymTab.entry.function f, ref int offset)
        {
            switch(token.token)
            { 
                case (lexicalScanner.SYMBOL.semicolont):
                    match(lexicalScanner.SYMBOL.semicolont);
                    if(error!= true)
                        argList(ref f, ref offset);
                    break;
                default:
                    //empty / lambda
                    break;
            }
        }

        private void procList(ref LinkedList<SymTab.paramNode> ll, ref int offset, ref lexicalScanner.SYMBOL paramMode)
        {
            switch (token.token)
            {
                case (lexicalScanner.SYMBOL.idt):
    
                    // { 1 }                   
                    checkForDups();
                    st.insert(token.lexeme, token.token, depth);
                    SymTab.entry ptr = st.lookUp(token.lexeme);
                    SymTab.varType typ = new SymTab.varType();
                    SymTab.entryType eTyp = new SymTab.entryType();

                    match(lexicalScanner.SYMBOL.idt);
                    if (error != true)
                        procListTail(ref typ , ref eTyp , ref ll, ref offset, ref  paramMode);

                  /*  if (error != true)
                        procList(ref ll, ref offset);
*/
                    
                    insertArg(ptr, typ, eTyp, ref offset, paramMode);
                    SymTab.paramNode pn = new SymTab.paramNode();
                    pn.typeVar = typ;
                    pn.mode = paramMode;
                    ll.AddFirst(pn);
                    break;
                /*   case (lexicalScanner.SYMBOL.semicolont):
                       match(lexicalScanner.SYMBOL.semicolont);
                       if(error != true)
                           procList(ref ll, ref offset);



                       break;*/
                default:
                    
                    //empty
                    break;
            }



        }
        private void procListTail(ref SymTab.varType typ, ref SymTab.entryType eTyp, ref LinkedList<SymTab.paramNode> ll, ref int offset, ref lexicalScanner.SYMBOL paramMode)
        {
            switch(token.token)
                {
                case (lexicalScanner.SYMBOL.commat):
                    match(lexicalScanner.SYMBOL.commat);

                    // { 1 } 
                    checkForDups();
                    st.insert(token.lexeme, token.token, depth);
                    SymTab.entry ptr  = st.lookUp(token.lexeme);
              
                    match(lexicalScanner.SYMBOL.idt);

                    if (error != true)
                        procListTail(ref typ, ref eTyp, ref ll, ref offset, ref paramMode);

             
                    //{ 3 }
                    insertArg(ptr, typ, eTyp, ref offset, paramMode);
                    SymTab.paramNode pn = new SymTab.paramNode();
                    pn.typeVar = typ;
                    pn.mode = paramMode;
                    ll.AddFirst(pn);
                    break;
                case(lexicalScanner.SYMBOL.colont):

                    match(lexicalScanner.SYMBOL.colont);    
                    if (error != true)
                        typeMark(ref typ, ref eTyp);



                    if (error != true)
                       moreTail(ref typ, ref eTyp, ref ll, ref offset);

                    break;
                case (lexicalScanner.SYMBOL.semicolont):
                    match(lexicalScanner.SYMBOL.semicolont);
                    if (error != true)
                        procList(ref ll, ref offset, ref paramMode);
                    break;
            }

        }

        private void moreTail(ref SymTab.varType typ, ref SymTab.entryType eTyp, ref LinkedList<SymTab.paramNode> ll, ref int offset)
        {
            lexicalScanner.SYMBOL paramMode = lexicalScanner.SYMBOL.intt;

            switch(token.token)
            {
                case (lexicalScanner.SYMBOL.semicolont):
                    match(lexicalScanner.SYMBOL.semicolont);

                    if (error != true)
                        mode(ref paramMode);

                    if (error != true)
                        procList(ref ll, ref offset,  ref paramMode);

                    break;

                default:
                    //Empty
                    break;
            }


/*
            if (error != true)
                mode(ref paramMode);

            if (error != true)
                procList(ref ll, ref offset);

    */
    }

        private void mode(ref lexicalScanner.SYMBOL mode)
        {
            switch (token.token)
            {
                case (lexicalScanner.SYMBOL.intt):
                    mode = lexicalScanner.SYMBOL.intt;
                    
                    match(lexicalScanner.SYMBOL.intt);
                    break;
                case (lexicalScanner.SYMBOL.outt):
                    mode = lexicalScanner.SYMBOL.outt;
                    match(lexicalScanner.SYMBOL.outt);
                    break;
                case (lexicalScanner.SYMBOL.inoutt):
                    mode = lexicalScanner.SYMBOL.inoutt;
                    match(lexicalScanner.SYMBOL.inoutt);
                    break;
                default:
                    //empty 
              
                    break;
            }
            
        }
        
        void checkForDups()
        {
            
            SymTab.entry eptr;
            eptr = st.lookUp(token.lexeme);

           // Console.WriteLine(token.lexeme);
           // Console.WriteLine(eptr.lexeme);

            if (eptr.depth == depth && eptr.lexeme == token.lexeme.ToLower())
            {
                error = true;
                Console.WriteLine("Error - Multiple declaration of \"" + eptr.lexeme + "\" on line " + lexicalScanner.ln);

            }

        }

        /// <summary>
        /// Matches grammar rule to current token. If there is a match, it gets the next token.
        /// 
        /// </summary>
        /// <param name="token"></param>
        private void match(lexicalScanner.SYMBOL desiredToken)
        {
            if (error != true)
            {            
                if (desiredToken != token.token)
                {
                     
                    //Console.WriteLine(lexicalScanner.ln);
                    sr.ReadToEnd();
                    //throw new Exception("Error in line " + lexicalScanner.ln + ". Expected token: " + desiredToken + ", Recieved: " + token.token);
                    Console.WriteLine("Error in line " + lexicalScanner.ln + ". Expected token: " + desiredToken + ", Recieved: " + token.token);

                    error = true;
                }

                else
                {
                    //Console.WriteLine("MATCHED " + desiredToken + " AND " + token.token);
                    token = lx.getNextToken();
                    while(token.token == lexicalScanner.SYMBOL.commentt)
                    {
                        token = lx.getNextToken();
                    }

                   
                }
            }
            else
            {
                token.token = lexicalScanner.SYMBOL.eoft;
            }
        }

        /// <summary>
        /// Emits code to intermediate representation
        /// </summary>
        /// <param name="code"></param>
        public void emit(string code)
        {
            if (visual == true)
                Console.Write(code);
            using (StreamWriter sw = new StreamWriter(path, true))
                sw.Write(code);

            



        }

    }//End class RDP




}
