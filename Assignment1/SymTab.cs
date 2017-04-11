
/// <summary>
/// Name: Markus Johan Haugsdal
/// Class: CSC 446 Compiler Construction
/// Assignment: 
/// Due Date: 04.04.2017
/// Instructor: Hamer
/// 
/// Description: Symbol table for compiler assignment 4
/// 
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Assignment1
{

    /// <summary>
    /// Class for symbol table
    /// </summary>
    public class SymTab
    {
        //Size of table (Prime number)
        const int tableSize = 211;
    

        public enum varType { charType, intType, floatType, def }
        public enum entryType { constEntry, varEntry, functionEntry }

        public class paramNode
        {
            public varType typeVar;
            public lexicalScanner.SYMBOL mode;
            public paramNode()
            {

            }
        }

        

        /// <summary>
        /// Polymorphic structure for entries to symbol table
        /// </summary>
        public class entry
        {
            public lexicalScanner.SYMBOL token = lexicalScanner.SYMBOL.unkownt;
            public string lexeme = " ";
            public int depth = 0;
            public entryType typeOfEntry;
            public bool isParameter = false;


            public entry()
            {

            }

            public entry(lexicalScanner.SYMBOL token, string lexeme, int depth )
            {
                this.token = token;
                this.lexeme = lexeme;
                this.depth = depth;
                //this.typeOfEntry = typeOfEntry;

            }
            public class var : entry
            {
                public varType typeOfVar;
                public int offset;
                public int size;
                public lexicalScanner.SYMBOL mode;
               // public bool isParameter = false;

                public var()
                {

                }

                /*public var(lexicalScanner.SYMBOL token, string lexeme, int depth, entryType typeOfEntry, varType typeOfVar, int offset, int size) : base(token, lexeme, depth, typeOfEntry)
                {

                    this.typeOfVar = typeOfVar;
                    this.offset = offset;
                    this.size = size;

                }*/
            }

            public class constant : entry
            {
                public varType typeOfConstant;
                public int offset;
                public int size;
                public constant()
                {

                }

              /*  public constant(lexicalScanner.SYMBOL token, string lexeme, int depth, entryType typeOfEntry, varType typeOfConstant) : base(token, lexeme, depth, typeOfEntry)
                {
                    this.typeOfConstant = typeOfConstant;
                }
*/
                public class intConstant : constant
                {
                    public int value;

                    public intConstant()
                    {

                    }

                   /* public intConstant(lexicalScanner.SYMBOL token, string lexeme, int depth, entryType typeOfEntry, varType typeOfConstant, int value) : base(token, lexeme, depth, typeOfEntry, typeOfConstant)
                    {
                        this.value = value;

                    }
                    */
                }
                public class floatConstant : constant
                {
                    public float valueR;


                    public floatConstant()
                    {

                    }

                    /*public floatConstant(lexicalScanner.SYMBOL token, string lexeme, int depth, entryType typeOfEntry, varType typeOfConstant, int valueR) : base(token, lexeme, depth, typeOfEntry, typeOfConstant)
                    {
                        this.valueR = valueR;
                    }*/
                }
            } // end constant

            public class function : entry
            {
                public int sizeOfLocal ;
                public int numberOfParams ;
                //public varType returnType ;
                public LinkedList<paramNode> paramList = new LinkedList<paramNode>();
                public int sizeOfParams;

                public function() 
                {

                }
/*
                public function(lexicalScanner.SYMBOL token, string lexeme, int depth, entryType typeOfEntry, int sizeOfLocal, int numberOfParams, varType returnType, LinkedList<varType> paramList, lexicalScanner.SYMBOL mode) : base(token, lexeme, depth, typeOfEntry)
                {
                    this.sizeOfLocal = sizeOfLocal;
                    this.numberOfParams = numberOfParams;
                    this.returnType = returnType;
                    this.paramList = paramList;
                    this.mode = mode;
                }

*/

            }

            public entry Next;
            public entry Prev;
        }// end entry



        //HashTable

        public static LinkedList<entry>[] hashTable = new LinkedList<entry>[tableSize];


        /// <summary>
        /// Populate the array with linked lists
        /// </summary>
        public SymTab( )
        {
            for (int i = 0; i < tableSize; i++)
            {
                hashTable[i] = new LinkedList<entry>();
            }

            for(int i = 0; i<tableSize; i++)
            {
                hashTable[i].AddFirst(new entry());
            }
        }


        /// <summary>
        /// Writes the contents of the table. Skips empty lists
        /// </summary>
        /// 


        public void writeTable(int depth)
        {




            if (rdp.error == true)
                return;
            string output = "";
            //string vOutput = String.Format("| {0,-10}  {1,-10} {2, -10} {3, -5} {4, -20}  {5, -20} \t\t\t  |", "Entrytype ", "Token", "Type", "Depth", "Size", "Offset");
            string fOutput = String.Format("| {0,-10}  {1,-10} {2, -10} {3, -15} {4, -20}  {5, -26} |", "Token", "Lexeme", "Depth", "Number of parameters", "Size of Locals", "Size of parameters");
           // Console.WriteLine("Following symbols are at depth " + depth);
            Console.WriteLine("|---------------------------------------------------------------------------------------------------------|");
            Console.WriteLine("|-------------------------------"+" Depth: "+ depth + " ----------------------------------------------------------------|");
            Console.WriteLine("|---------------------------------------------------------------------------------------------------------|");

            int t = 0;
            for (int i=0; i<hashTable.Length;i++)
            {
/*
                if(t>4)
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    t = 0;
                }*/

                try
                {


                    entry temp = hashTable[i].ElementAt(0);


                    while (temp.Next != null)
                    {

                        if (temp.Next.depth == depth)
                        {
                          //  t++;

                            switch (temp.Next.typeOfEntry)
                            {
                                case (SymTab.entryType.functionEntry):
                                    SymTab.entry.function t1 = temp.Next as SymTab.entry.function;
                                    Console.WriteLine("| Procedure: \t\t\t\t\t\t\t\t\t\t\t\t\t  |");
                                    output = string.Format("| {0,-10}  {1,-10} {2, -10} {3, -20} {4, -20}  {5, -26} |",
                                        t1.token, t1.lexeme, t1.depth,  t1.numberOfParams, t1.sizeOfLocal, t1.sizeOfParams);

                                    Console.WriteLine(fOutput);
                                    Console.WriteLine(output);
                                    Console.Write("| Parameters: ");

                                    if (t1.numberOfParams > 0)
                                        for (int j = 1; j <= t1.paramList.Count; j++)
                                        {
                                            Console.Write(t1.paramList.ElementAt(j - 1).mode + " " + t1.paramList.ElementAt(j - 1).typeVar + "  ");
                                        }
                                           // Console.Write(t1.paramList.ElementAt(j-1)+ "   " );
                                    else
                                        Console.WriteLine(" none");
                                     Console.WriteLine();
                                    break;

                                case (SymTab.entryType.constEntry):
                                    Console.WriteLine();
                                    SymTab.entry.constant t2 = temp.Next as SymTab.entry.constant;
                                    switch(t2.typeOfConstant)
                                    {
                                        case (SymTab.varType.floatType):
                                            SymTab.entry.constant.floatConstant f1 = temp.Next as SymTab.entry.constant.floatConstant;

                          
                                            Console.WriteLine("Constant: " + f1.lexeme + " With the following data: ");
                                            Console.WriteLine("Token: " + f1.token);
                                            Console.WriteLine("Type: " + f1.typeOfConstant);
                                            Console.WriteLine("Depth: " + f1.depth);
                                            Console.WriteLine("Size: " + f1.size);
                                            Console.WriteLine("Offset: " + f1.offset);
                                            break;
                                        case (SymTab.varType.intType):
                                            SymTab.entry.constant.intConstant i1 = temp.Next as SymTab.entry.constant.intConstant;
                                            Console.WriteLine("Constant: " + i1.lexeme + " With the following data: ");
                                            Console.WriteLine("Token: " + i1.token);
                                            Console.WriteLine("Type: " + i1.typeOfConstant);
                                            Console.WriteLine("Depth: " + i1.depth);
                                            Console.WriteLine("Size: " + i1.size);
                                            Console.WriteLine("Offset: " + i1.offset);

                                            break;
                                    }


                                    Console.WriteLine();
                                    break;

                                case (SymTab.entryType.varEntry):
                                    SymTab.entry.var t3 = temp.Next as SymTab.entry.var;
                          
                                    Console.WriteLine();
                                    Console.WriteLine("Variable: " + t3.lexeme + " With the following data: ");
                                    Console.WriteLine("Token: " + t3.token);
                                    Console.WriteLine("Type: " + t3.typeOfVar);
                                    Console.WriteLine("Depth: " + t3.depth);
                                    Console.WriteLine("Size: " + t3.size);
                                    Console.WriteLine("Offset: " + t3.offset);
                                    Console.WriteLine();
                                    break;
                            }
                        }
                        //Console.WriteLine(temp.Next.lexeme);
                        //Console.WriteLine(temp.Next.typeOfEntry);
                        if(temp.Next != null)
                        {
                            temp = temp.Next;
                        }
                    }


                }
                catch (NullReferenceException)
                {
                    //Console.WriteLine("NULL REFERENCE!");
                    //Empty entry
                }
            }
        }



        /// <summary>
        /// Inserts an entry into the symbol table. Uses a hash function to find its position
        /// </summary>
        /// <param name="lexeme"></param>
        /// <param name="token"></param>
        /// <param name="depth"></param>

        unsafe public void insert(string lexeme, lexicalScanner.SYMBOL token, int depth)
        {
            
            //int h = hash(convertStringToCharP(lexeme)); // Get hash
            uint h = hash(lexeme.ToLower());
            

            entry temp = new entry(token, lexeme, depth); //Create new record

            //Console.WriteLine(hashTable[h].Count);
           //S Console.WriteLine(token);

            //If there are elements in the list
            if (hashTable[h].ElementAt(0).Next != null)
            {
                temp.Next = hashTable[h].ElementAt(0).Next;
                hashTable[h].ElementAt(0).Next = temp;

                temp.Next.Prev = temp;
                temp.Prev = hashTable[h].ElementAt(0);

            }
            //If there are no elements in the list
            else
            {
              
                hashTable[h].ElementAt(0).Next = temp;
                temp.Prev = hashTable[h].ElementAt(0);

             
            }

            

        }
        
        /// <summary>
        /// Uses the lexeme to find an entry in the symbol table
        /// </summary>
        /// <param name="lexeme"></param>
        /// <returns>object of type entry</returns>

        unsafe public entry lookUp(string lexeme) //Lookup using the lexeme
        {
           
            uint h = hash(lexeme.ToLower());
            entry temp = new entry();


            temp = hashTable[h].ElementAt(0);

            while (temp.Next != null)
            {
                    
                if (temp.lexeme == lexeme || temp.lexeme == lexeme.ToLower())
                {
                    return temp;
                }
                if (temp.Next != null)
                    temp = temp.Next;
            }
            return temp;
        }

        /// <summary>
        /// Deletes all entries with given depth
        /// </summary>
        /// <param name="depth"></param>
        public void deleteDepth(int depth)
        {
            for(int i =0; i<hashTable.Length; i++)
            {
                entry temp = hashTable[i].ElementAt(0); // Create temp and point to head

                if (temp != null) // If element is not empty.
                {
                    while (temp.Next != null) //Iterate list
                    {
                        if (temp.Next.depth == depth) //found!
                        {
                            if (temp.Next.Next != null) //If there is at least two elements
                            {
                                temp.Next = temp.Next.Next; // Remove reference to middle node

                            }
                            else // only one element
                            {
                                temp.Next = null; // delete

                            }

                        }

                        else if (temp.Next != null)
                            temp = temp.Next;
                    }
                }



            }
        }


        /// <summary>
        /// HashPJW function.  For each character, it shifts the bits of h 4 positions left and adds.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>

        private uint hash(string s)
        {
            uint h = 0, g;
            uint prime = tableSize;

            for (int i = 0; i < s.Length; i++)
            {
                h = (h << 4) + s[i];
                g = h & 0xF0000000;
                if (g != 0)
                {
                    h = h ^ (g >> 24);
                    h = h ^ g;
                }
            }

            return h % prime;
        }

    /*
        unsafe public int hash(char * s)
        {

            int prime = tableSize;

            char* p;
            uint h = 0, g;
      

            for(p=s; *p != '\0'; p++)
            {
                h = (h << 4) + (*p);
                g = h & 0xF0000000;

                if (g != 0)
                {
                    h = h ^ (g >> 24);
                    h = h ^ g;
                }
            }

            return Convert.ToInt32(h)%prime;
        }
        */

        /// <summary>
        /// Converts string to char *
        /// </summary>
        /// <param name="s"></param>
        /// <returns>char *</returns>
        /// 

            /*
        unsafe char* convertStringToCharP(string s)
        {

            IntPtr p = Marshal.StringToHGlobalAnsi(s);
            char* cp = (char*)(p.ToPointer());
            return cp;
        }
            */
  

    }// emd class symTab
}



