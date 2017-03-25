
/// <summary>
/// Name: Markus Johan Haugsdal
/// Class: CSC 446 Compiler Construction
/// Assignment: 
/// Due Date: 03.20.2017
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

        /// <summary>
        /// Polymorphic structure for entries to symbol table
        /// </summary>
        public class entry
        {
            public lexicalScanner.SYMBOL token = lexicalScanner.SYMBOL.unkownt;
            public string lexeme = " ";
            public int depth = 0;
            public entryType typeOfEntry;


            public entry()
            {

            }

            /*public entry(lexicalScanner.SYMBOL token, string lexeme, int depth, entryType typeOfEntry)
            {
                this.token = token;
                this.lexeme = lexeme;
                this.depth = depth;
                this.typeOfEntry = typeOfEntry;

            }*/
            public class var : entry
            {
                public varType typeOfVar;
                public int offset;
                public int size;

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
                public varType returnType ;
                public LinkedList<varType> paramList = new LinkedList<varType>();
                public lexicalScanner.SYMBOL mode = lexicalScanner.SYMBOL.intt;


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
        public void writeTable(int depth)
        {

            Console.WriteLine("Following symbols are at depth " + depth);
            for (int i=0; i<hashTable.Length;i++)
            {
                try
                {
                    //Console.WriteLine(depth);

                    entry temp = hashTable[i].ElementAt(0);

                    // Console.WriteLine(temp.Next.lexeme);
                    //Console.WriteLine("HELLO");


                    while (temp.Next != null)
                    {
                        //Console.WriteLine("Hey");

                        if (temp.Next.depth == depth)
                            Console.WriteLine(temp.Next.lexeme + " " + temp.Next.token + " " + temp.Next.depth);
                           // Console.WriteLine("FOUND!");

                        if(temp.Next != null)
                            temp = temp.Next;
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
            uint h = hash(lexeme);

            entry temp = new entry(); //Create new record

            //Console.WriteLine(hashTable[h].Count);

            if (hashTable[h].Count > 2)
            {
                temp.Next = hashTable[h].ElementAt(0).Next.Next;
                hashTable[h].ElementAt(0).Next = temp; // Enter record to ht
                
            }
            else
            {
              
                hashTable[h].ElementAt(0).Next = temp;

            }

            //hashTable[h].AddFirst(temp); // Enter record at location in symbol table

        }
        
        /// <summary>
        /// Uses the lexeme to find an entry in the symbol table
        /// </summary>
        /// <param name="lexeme"></param>
        /// <returns>object of type entry</returns>

        unsafe public entry lookUp(string lexeme) //Lookup using the lexeme
        {
            int i = 0;
            int j = 0;

            uint h = hash(lexeme);
            entry temp = new entry();


            temp = hashTable[h].ElementAt(0);

            if(temp != null)
            {
                while (temp.Next != null)
                {
                    if (temp.lexeme == lexeme)
                    {
                        return temp;
                    }

                    else if (temp.Next != null)
                        temp = temp.Next;
                }
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



