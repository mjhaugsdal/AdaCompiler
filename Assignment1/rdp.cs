/// <summary>
/// Name: Markus Johan Haugsdal
/// Class: CSC 446 Compiler Construction
/// Assignment: 
/// Due Date: 03.29.2017
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
   
        public static bool error = false;
        public int depth = 1;
        private lexicalScanner.Token token;
        private lexicalScanner lx;
        private StreamReader sr;
        private SymTab st;


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
            //

            match(lexicalScanner.SYMBOL.proct);

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
            int oldOffset = 0;
            if (error != true)
                declPart(ref f, ref offset, ref oldOffset);
            //Procedures

     
            if (error != true)
                procedures();
            //Match begint
            match(lexicalScanner.SYMBOL.begint);
            //Not implemented. Sequence of statements.
            if (error != true)
                seqOfStatements();
            //{ 3 }

 
            insertFunction(f);
            match(lexicalScanner.SYMBOL.endt);
            match(lexicalScanner.SYMBOL.idt);
            match(lexicalScanner.SYMBOL.semicolont);
            st.writeTable(depth);
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
        private void insertArg(SymTab.entry ptr, SymTab.varType typ, SymTab.entryType eTyp, ref int offset)
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

        private void seqOfStatements()
        {
            //EMPTY
            //throw new NotImplementedException();
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
                    oldOffset = offset;
                  
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

            int counter = 1;
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

                        ce.offset = counter * ce.size + oldOffset;
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


                        ce.offset = counter * ce.size + oldOffset;
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

                        v.offset = counter * v.size + oldOffset;
                        offset = offset + v.size;

                    }

                    else if (typ == SymTab.varType.intType)
                    {
                        v.size = 2;

                        v.offset = counter * v.size + oldOffset;
                        offset = offset + v.size;

                    }
                    else if (typ == SymTab.varType.charType)
                    {
                        v.size = 1;

                        v.offset = counter * v.size + oldOffset;
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
            
            if (error != true)
                mode(ref f);

            if (error != true)
            {
                procList(ref f.paramList, ref offset);

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

        private void procList(ref LinkedList<SymTab.varType> ll, ref int offset)
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
                        procListTail(ref typ , ref eTyp , ref ll, ref offset);

                  /*  if (error != true)
                        procList(ref ll, ref offset);
*/
                    insertArg(ptr, typ, eTyp, ref offset);
                    ll.AddFirst(typ);
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
        private void procListTail(ref SymTab.varType typ, ref SymTab.entryType eTyp, ref LinkedList<SymTab.varType> ll, ref int offset)
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
                        procListTail(ref typ, ref eTyp, ref ll, ref offset);

                    //{ 3 }
                    insertArg(ptr, typ, eTyp, ref offset);
                    ll.AddFirst(typ);
                    break;
                case(lexicalScanner.SYMBOL.colont):

                    match(lexicalScanner.SYMBOL.colont);    
                    if (error != true)
                        typeMark(ref typ, ref eTyp);

                    if (error != true)
                        procListTail(ref typ, ref eTyp, ref ll, ref offset);

                    break;
                case (lexicalScanner.SYMBOL.semicolont):
                    match(lexicalScanner.SYMBOL.semicolont);
                    if (error != true)
                        procList(ref ll, ref offset);
                    break;
            }

        }



        private void processVar(ref SymTab.entry.var var, SymTab.varType typ)
        {
            switch (typ)
            {
                case (SymTab.varType.intType):
                    break;
            }
        }

        private void mode(ref SymTab.entry.function f)
        {
            switch (token.token)
            {
                case (lexicalScanner.SYMBOL.intt):
                    f.mode = lexicalScanner.SYMBOL.intt;
                    match(lexicalScanner.SYMBOL.intt);
                    break;
                case (lexicalScanner.SYMBOL.outt):
                    f.mode = lexicalScanner.SYMBOL.outt;
                    match(lexicalScanner.SYMBOL.outt);
                    break;
                case (lexicalScanner.SYMBOL.inoutt):
                    f.mode = lexicalScanner.SYMBOL.inoutt;
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

       
    }//End class RDP
}
