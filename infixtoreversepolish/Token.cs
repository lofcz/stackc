using System;
using System.Collections.Generic;
using System.Text;

namespace infixtoreversepolish
{
    public enum TokenTypes
    {
        Null,
        Void,
        Number,
        Word,
        Operator,
        Semicolon,
        Keyword,
        String
    }

    public enum Precedence
    {
        None, // -1

        Eq, // 0

        BracketEnd, // 1
        BracketStart, // 1      

        Plus, // 2
        Minus, // 2

        Times, // 3
        Divide, // 3
        Tilde, // 3

        Power, // 4

        UnaryPlus, // 5
        UnaryMinus // 5
    }

    public class Token
    {
        public Precedence Precedence { get; set; }
        public TokenTypes Type {get; set;}
        public string Text { get; set; }
        public dynamic Value {get; set; }

        public static int PrattPrecedence(Precedence precedence)
        {
            if (precedence == Precedence.None)
            {
                return -1;
            }
            
            if (precedence == Precedence.Eq)
            {
                return 0;
            }

            if (precedence == Precedence.BracketStart || precedence == Precedence.BracketEnd)
            {
                return 1;
            }

            if (precedence == Precedence.Plus || precedence == Precedence.Minus)
            {
                return 2;
            }

            if (precedence == Precedence.Tilde || precedence == Precedence.Times || precedence == Precedence.Divide)
            {
                return 3;
            }

            if (precedence == Precedence.Power)
            {
                return 4;
            }

            if (precedence == Precedence.UnaryPlus || precedence == Precedence.UnaryMinus)
            {
                return 5;
            }

            return -1;
        }
    }
}
