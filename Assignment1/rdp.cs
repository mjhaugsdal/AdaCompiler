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

            checkForDups();
            st.insert(token.lexeme, token.token, depth);

            SymTab.entry ptr = new SymTab.entry();
            ptr = st.lookUp(token.lexeme);
            //Console.WriteLine(token.token);
            match(lexicalScanner.SYMBOL.idt);

            //Create f for functionEntry
            SymTab.entry.function f = new SymTab.entry.function();
            
            f.lexeme = ptr.lexeme;
            f.depth = ptr.depth;
            f.token = ptr.token;

            //Args
            if (error != true)
                pArgs(ref f);
            //is

            match(lexicalScanner.SYMBOL.ist);
            depth++;


            //DeclarativePart
            int offset = 0;
            if (error != true)
                declPart(ref f, ref offset);
            //Procedures
            if (error != true)
                procedures();
            //Match begint
            match(lexicalScanner.SYMBOL.begint);
            //Not implemented. Sequence of statements.
            if (error != true)
                seqOfStatements();

            insertFunction(f);
            match(lexicalScanner.SYMBOL.endt);
            match(lexicalScanner.SYMBOL.idt);
            match(lexicalScanner.SYMBOL.semicolont);
            st.writeTable(depth);
            depth--;

            return token;

        }

        private void insertVar(SymTab.entry ptr, SymTab.varType typ, SymTab.entryType eTyp, ref int offset)
        {
            Console.WriteLine(eTyp);
           
            switch (eTyp)
            {
                case (SymTab.entryType.constEntry):
                 

                    if (typ == SymTab.varType.floatType)
                    {
                        //Console.WriteLine("he");
                        SymTab.entry.constant.floatConstant ce = new SymTab.entry.constant.floatConstant();
                        ce.lexeme = ptr.lexeme;
                        ce.token = ptr.token;
                        ce.depth = ptr.depth;
                        ce.typeOfConstant = typ;
                        ce.typeOfEntry = eTyp;
                        ce.size = 4;

                        offset = offset + ce.size;
                        ce.offset = offset;

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
                        Console.WriteLine("Int const ");

                        SymTab.entry.constant.intConstant ce = new SymTab.entry.constant.intConstant();

                        Console.WriteLine(ptr.lexeme);
                        ce.lexeme = ptr.lexeme;
                        ce.token = ptr.token;
                        ce.depth = ptr.depth;
                        ce.typeOfConstant = typ;
                        ce.typeOfEntry = eTyp;
                        ce.size = 2;
                        offset = offset + ce.size;
                        ce.offset = offset;

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
                    //Console.WriteLine("VARENTRY");
                    SymTab.entry.var v = new SymTab.entry.var();
                    v.lexeme = ptr.lexeme;
                    v.token = ptr.token;
                    v.depth = ptr.depth;
                    v.typeOfVar = typ;
                    v.typeOfEntry = eTyp;

                    if (typ == SymTab.varType.floatType)
                    {

                        v.size = 4;
                        offset = offset + v.size;
                        v.offset = offset;
                    }

                    else if (typ == SymTab.varType.intType)
                    {
                        v.size = 2;
                        offset = offset + v.size;
                        v.offset = offset;

                    }
                    else if(typ == SymTab.varType.charType)
                    {
                        v.size = 1;
                        offset = offset + v.size;
                        v.offset = offset;
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

        private void insertFunction(SymTab.entry.function f)
        {
            SymTab.entry eptr;
            eptr = st.lookUp(f.lexeme);
            //Console.WriteLine(f.token);

            f.typeOfEntry = SymTab.entryType.functionEntry;
            f.numberOfParams = f.paramList.Count;


            if(eptr.Next != null)
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

        private void procedures()
        {

            //-> Prog Procedures | e
            switch (token.token)
            {
                case (lexicalScanner.SYMBOL.proct):
                    if(error != true)
                        parse(token);
                    break;
                default:
                    //Lambda / empty
                    break;
            }

        }

         
        private void declPart(ref SymTab.entry.function f, ref int offset)
        {
            switch(token.token)
            {

                case (lexicalScanner.SYMBOL.idt):
                    
                    if (error != true)
                    {
                        LinkedList<SymTab.varType> ll = new LinkedList<SymTab.varType>();
                        idList(ref ll, ref offset);
                    }
                    /*match(lexicalScanner.SYMBOL.colont);
                    if (error != true)
                        typeMark();*/
                    f.sizeOfLocal = offset;
                    match(lexicalScanner.SYMBOL.semicolont);
                    if (error != true)
                        declPart(ref f, ref offset);

                   // f.sizeOfLocal = offset;

                    break;
                default:
                    //Lambda / empty
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
            //Console.Write("HERE: ");
           // Console.WriteLine("Variable info: " + v.lexeme + " " + v.typeOfVar);


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

            int offset = 2;
            switch (token.token)
            {
                
                case (lexicalScanner.SYMBOL.lparent):
                    match(lexicalScanner.SYMBOL.lparent);
                    //We have variables!
                    if (error != true)
                        argList(ref f, offset);
                    
                    match(lexicalScanner.SYMBOL.rparent);
                    break;
                default:
                    //empty 
                    break;
            }



        }

        private void argList(ref SymTab.entry.function f,  int offset)
        {

            //SymTab.entry.var v = new SymTab.entry.var();
            
            if (error != true)
                mode(ref f);
            //match(lexicalScanner.SYMBOL.idt);

            if (error != true)
            {
                idList(ref f.paramList, ref offset);

            }
            //Console.WriteLine(offset);
            f.sizeOfParams = offset-2;
            if (error != true)
                moreArgs(ref f, ref offset);

            //Console.WriteLine(offset);
           
        }

        private void moreArgs(ref SymTab.entry.function f, ref int offset)
        {
            switch(token.token)
            { 
                case (lexicalScanner.SYMBOL.semicolont):
                    match(lexicalScanner.SYMBOL.semicolont);
                    if(error!= true)
                        argList(ref f, offset);
                    break;
                default:
                    //empty / lambda
                    break;
            }
        }

        private void idList(ref LinkedList<SymTab.varType> ll,  ref int offset)
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

                    Console.WriteLine(token.lexeme);

                    match(lexicalScanner.SYMBOL.idt);
                    if (error != true)
                        idListTail(ref typ , ref eTyp , ref ll, ref offset);

                    insertVar(ptr, typ, eTyp, ref offset);
                    ll.AddLast(typ);
                    //st.writeTable(2);

                    if (error != true)
                        idList(ref ll, ref offset);

                    break;
                default:
                    
                    //empty
                    break;
            }

            

        }
        private void idListTail(ref SymTab.varType typ, ref SymTab.entryType eTyp, ref LinkedList<SymTab.varType> ll, ref int offset)
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
                        idListTail(ref typ, ref eTyp, ref ll, ref offset);

                    // insertToSymbolTable(ptr, typ, eTyp, ll);
                    //processVar(ref var, typ);
           
                    insertVar(ptr, typ, eTyp, ref offset);

                    ll.AddLast(typ);
                    break;
                case(lexicalScanner.SYMBOL.colont):

                    match(lexicalScanner.SYMBOL.colont);    
                    if (error != true)
                        typeMark(ref typ, ref eTyp);

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

            //Console.WriteLine("Found: " + eptr.lexeme);

            if (eptr.depth == depth && eptr.lexeme == token.lexeme)
            {
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
                    //sr.ReadToEnd();
                    //throw new Exception("Error in line " + lexicalScanner.ln + ". Expected token: " + desiredToken + ", Recieved: " + token.token);
                    Console.WriteLine("Error in line " + lexicalScanner.ln + ". Expected token: " + desiredToken + ", Recieved: " + token.token);

                    error = true;
                }

                else
                {
                   // Console.WriteLine("MATCHED " + desiredToken + " AND " + token.token);
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
