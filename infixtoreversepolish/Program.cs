using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;

namespace infixtoreversepolish
{

    class Program
    {
        static void Main(string[] args)
        {
            Eval eval = new Eval();
            Ast ast = new Ast();

            while(true)
            {
                Console.Write("Zadej výraz: ");
                string input = Console.ReadLine();
                Console.WriteLine();

                // Rekurzivní pasrsování do hloubky
                // Funkce - čísla, binární + - * /

                List<int> r = ast.BuildTree(input);

                Console.Write("Výsledky: ");
                foreach(int result in r)
                {
                    Console.Write(result + " ");
                }

                Console.WriteLine();

                // RPN:
                // Funkce - čísla, binární + - * /, unární + -, závorky

                /* 
                 EvalResult result = eval.Evaluate(input);
                 Console.WriteLine("RPN: " + result.InputAsRpn);
                 Console.WriteLine("Výsledek: " + result.Result);
                */

                Console.WriteLine();
            }
        }
    }
}
