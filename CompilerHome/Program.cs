using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace CompilerHome
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader read = new StreamReader("C:/Users/CHAMPHAHA/documents/visual studio 2015/Projects/CompilerHome/CompilerHome/CodeTest.txt");
            string temp_read = read.ReadToEnd();

            Lexical lex = new Lexical(temp_read+"$");
            lex.WordLex();
            lex.printlex();

            Paser pas = new Paser(lex.list);
            pas.S();
            
            
        }
    }
}
