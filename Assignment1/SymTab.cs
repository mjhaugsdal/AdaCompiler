
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


        /// <summary>
        /// Polymorphic structure for entries to symbol table
        /// </summary>
        public class entry
        {
            public lexicalScanner.SYMBOL token = lexicalScanner.SYMBOL.unkownt;
            public string lexeme = " ";
            public int depth = 0;

            public entry(lexicalScanner.SYMBOL a, string b, int c)
            {
                token = a;
                lexeme = b;
                depth = c;

            }

        }

        public class var : entry
        {
            public varType typeOfVar;
            public int Offset;
            public int size;

            public var(lexicalScanner.SYMBOL a, string b, int c, varType d, int e, int f) : base(a, b, c)
            {
                typeOfVar = d;
                Offset = e;
                size = f;

            }
        }

        public class constant : entry
        {
            public varType typeOfConstant;

            public constant(lexicalScanner.SYMBOL a, string b, int c, varType d) : base(a, b, c)
            {
                typeOfConstant = d;
            }

            public class intConstant : constant
            {
                public int value;

                public intConstant(lexicalScanner.SYMBOL a, string b, int c, varType d, int e) : base(a, b, c, d)
                {
                    value = e;

                }
            }
            public class floatConstant : constant
            {
                public float valueR;

                public floatConstant(lexicalScanner.SYMBOL a, string b, int c, varType d, int e) : base(a, b, c, d)
                {
                    valueR = e;
                }
            }
        } // end constant

        public class function : entry 
        {
            public int sizeOfLocal;
            public int numberOfParams;
            public varType returnType;
            public LinkedList<varType> paramList = new LinkedList<varType>();

            public function(lexicalScanner.SYMBOL a, string b, int c, int d, int e, varType f, LinkedList<varType> g) : base(a, b, c)
            {
                sizeOfLocal = d;
                numberOfParams = e;
                returnType = f;
                paramList = g;
            }
        }

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
                hashTable[i].AddFirst(new entry(lexicalScanner.SYMBOL.unkownt, "", 0));
            }
        }


        /// <summary>
        /// Writes the contents of the table. Skips empty lists
        /// </summary>
        public void writeTable()
        {
            for(int i=0; i<hashTable.Length;i++)
            {
                try
                {
                    for (int j = 0; j < hashTable[i].Count; j++)
                    {
                        if(hashTable[i].ElementAt(j).depth != 0)
                        Console.Write(hashTable[i].ElementAt(j).lexeme + " " + hashTable[i].ElementAt(j).token + " " + hashTable[i].ElementAt(j).depth+ "  " );
                    }

                    if (hashTable[i].ElementAt(0).depth != 0)
                        Console.WriteLine();
                }
                catch(NullReferenceException)
                {
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

            entry temp = new entry(token, lexeme, depth); //Create new record
           

            
            hashTable[h].AddFirst(temp); // Enter record at location in symbol table

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
            entry temp = new entry(lexicalScanner.SYMBOL.unkownt, "", 0);

            for (i=0; i<tableSize; i++)
            {
                for (j = 0; j < hashTable[j].Count; j++)
                {
                    if(hashTable[i].ElementAt(j).lexeme == lexeme)
                    {
                         temp = hashTable[i].ElementAt(j);
                        return temp;
                    }

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
                for(int j= 0; j<hashTable[i].Count; j++)
                {
                    if (hashTable[i].ElementAt(j).depth == depth)
                    {
                        hashTable[i].ElementAt(j).depth = 0;
                        hashTable[i].ElementAt(j).lexeme = "";
                        hashTable[i].ElementAt(j).token = lexicalScanner.SYMBOL.unkownt;
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

            for(int i =0; i<s.Length; i++)
            {
                h = (h << 4) + s[i];
                g = h & 0xF0000000;
                if (g != 0)
                {
                    h = h ^ (g >> 24);
                    h = h ^ g;
                }
            }

            return h%prime;
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



