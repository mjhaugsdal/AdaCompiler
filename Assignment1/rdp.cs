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
            st.insert(token.lexeme, lexicalScanner.SYMBOL.proct, depth);

            SymTab.entry ptr = new SymTab.entry();
            ptr = st.lookUp(token.lexeme);

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


            insertFunction(f);
            match(lexicalScanner.SYMBOL.ist);
            depth++;
            

            //DeclarativePart
            if (error != true)
                declPart();
            //Procedures
            if (error != true)
                procedures();
            //Match begint
            match(lexicalScanner.SYMBOL.begint);
            //Not implemented. Sequence of statements.
            if (error != true)
                seqOfStatements();
            match(lexicalScanner.SYMBOL.endt);
            match(lexicalScanner.SYMBOL.idt);
            match(lexicalScanner.SYMBOL.semicolont);
            depth--;

            //Console.WriteLine("I should only be here two times");
            st.writeTable(depth);

           
            return token;

        }
        private void insertVar(SymTab.entry.var v)
        {
            
            //Console.WriteLine("Entering: " + v.lexeme);
            
            SymTab.entry eptr;
            eptr = st.lookUp(v.lexeme);
            v.typeOfEntry = SymTab.entryType.varEntry;
                
            if (eptr.Next != null)
            {
                v.Prev = eptr.Prev;
                v.Next = eptr.Next;
                eptr.Prev.Next = v;
                v.Next.Prev = v;
            }
            else
            {
                v.Prev = eptr.Prev;
                eptr.Prev.Next = v;
            }

        }

        private void insertFunction(SymTab.entry.function f)
        {
            SymTab.entry eptr;
            eptr = st.lookUp(f.lexeme);
            f.typeOfEntry = SymTab.entryType.functionEntry;
            f.numberOfParams = f.paramList.Count;

            for (int i = 0; i < f.paramList.Count; i++)
               // Console.WriteLine(f.paramList.ElementAt(i));

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

         
        private void declPart()
        {
            switch(token.token)
            {
                case (lexicalScanner.SYMBOL.idt):
                    if (error != true)
                    {
                        LinkedList<SymTab.varType> ll = new LinkedList<SymTab.varType>();
                        idList(ref ll);
                    }
                    /*match(lexicalScanner.SYMBOL.colont);
                    if (error != true)
                        typeMark();*/
                    match(lexicalScanner.SYMBOL.semicolont);
                    if (error != true)
                        declPart();
                    break;
                default:
                    //Lambda / empty
                    break;
            }
        }

        private void typeMark(SymTab.varType typ)
        {
            
            switch (token.token)
            {
                case (lexicalScanner.SYMBOL.integert):
                    
                    v.typeOfEntry = SymTab.entryType.varEntry;
                    v.typeOfVar = SymTab.varType.intType;
                    v.size = 2;
                    v.offset = v.offset + 2;
                    match(lexicalScanner.SYMBOL.integert);
                    
                    break;
                case (lexicalScanner.SYMBOL.floatt):
                    v.typeOfEntry = SymTab.entryType.varEntry;
                    v.typeOfVar = SymTab.varType.floatType;
                    v.size = 4;
                    v.offset = v.offset + 4;
                    match(lexicalScanner.SYMBOL.floatt);
                
                    break;
               /* case (lexicalScanner.SYMBOL.realt):
                    match(lexicalScanner.SYMBOL.realt);
                    break;*/
                case (lexicalScanner.SYMBOL.chart):
                    v.typeOfEntry = SymTab.entryType.varEntry;
                    v.typeOfVar = SymTab.varType.charType;
                    v.size = 1;
                    v.offset = v.offset + 1;
                    match(lexicalScanner.SYMBOL.chart);
                  

                    break;
                case (lexicalScanner.SYMBOL.constt):
                    v.typeOfEntry = SymTab.entryType.constEntry;
                    match(lexicalScanner.SYMBOL.constt);
                    match(lexicalScanner.SYMBOL.assignopt);
                    

                    if (error != true)
                        value( v.typeOfVar);
                    
                    break;
            }
            //Console.Write("HERE: ");
           // Console.WriteLine("Variable info: " + v.lexeme + " " + v.typeOfVar);


        }

       
        /// <summary>
        /// Checks if token has value (number token)
        /// </summary>
        private void value( SymTab.varType typeOfVar)
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
            //f.token = token.token;
            //Console.WriteLine(f.token);
            switch(token.token)
            {
                case (lexicalScanner.SYMBOL.lparent):
                    match(lexicalScanner.SYMBOL.lparent);
                    //We have variables!
                    if (error != true)
                        argList(ref f);
                    match(lexicalScanner.SYMBOL.rparent);
                    break;
                default:
                    //empty 
                    break;
            }



        }

        private void argList(ref SymTab.entry.function f)
        {
            
            //SymTab.entry.var v = new SymTab.entry.var();

            if (error != true)
                mode(ref f);
            //match(lexicalScanner.SYMBOL.idt);

            if (error != true)
            {
                idList(ref f.paramList);

            }
                
            if (error != true)
                moreArgs(ref f);

        }

        private void moreArgs(ref SymTab.entry.function f)
        {
            switch(token.token)
            { 
                case (lexicalScanner.SYMBOL.semicolont):
                    match(lexicalScanner.SYMBOL.semicolont);
                    if(error!= true)
                        argList(ref f);
                    break;
                default:
                    //empty / lambda
                    break;
            }
        }

        private void idList(ref LinkedList<SymTab.varType> ll)
        {
            switch (token.token)
            {
                case (lexicalScanner.SYMBOL.idt):

                    SymTab.entry.var v = new SymTab.entry.var();
                    
                    // { 1 }                   
                    checkForDups();
                    st.insert(token.lexeme, token.token, depth);
                    SymTab.entry ptr = st.lookUp(token.lexeme);
                    SymTab.varType typ = new SymTab.varType();
                    //Copy data
                    v.lexeme = ptr.lexeme;
                    v.token = ptr.token;
                    v.depth = ptr.depth;
                    

                    match(lexicalScanner.SYMBOL.idt);
                    if (error != true)
                        idListTail(typ, ref ll);

                    Console.WriteLine("Variable info: " + v.lexeme +" "  + v.typeOfVar);

                    if (error != true)
                        idList(ref ll);
                    Console.WriteLine("Variable info: " + v.lexeme + " " + v.typeOfVar);


                    // insertVar(v);
                    break;
                default:
                    
                    //empty
                    break;
            }

            

        }
        private void idListTail(SymTab.varType typ, ref LinkedList<SymTab.varType> ll)
        {
            switch(token.token)
                {
                case (lexicalScanner.SYMBOL.commat):
                    match(lexicalScanner.SYMBOL.commat);

                    SymTab.entry.var var = new SymTab.entry.var();

                    // { 1 } 
                    checkForDups();
                    st.insert(token.lexeme, token.token, depth);
                    SymTab.entry ptr  = st.lookUp(token.lexeme);

                    //Copy to variable var
                    var.lexeme = ptr.lexeme;
                    var.token = ptr.token;
                    var.depth = ptr.depth;
                    
                    match(lexicalScanner.SYMBOL.idt);

                    if (error != true)
                        idListTail(typ, ref ll);

                    //var.typeOfVar = v.typeOfVar;
                    //Console.WriteLine(var.typeOfVar);
                    //ll.AddFirst(v.typeOfVar);
                    Console.WriteLine("Variable info: " + var.lexeme + " " + var.typeOfVar);
                    insertVar(var);
                    break;
                case(lexicalScanner.SYMBOL.colont):

                    match(lexicalScanner.SYMBOL.colont);    
                    if (error != true)
                        typeMark(typ);


                   // Console.Write("HERE NOW: ");
                    //Console.WriteLine("Variable info after type: " + v.lexeme + " " + v.typeOfVar);
                    //insertVar(v);
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
