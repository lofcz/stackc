using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace infixtoreversepolish
{
    public class Ast
    {
        AstNode LastNode = null;
        Queue<Token> tokens = new Queue<Token>();
        Tokenizer tokenizer = new Tokenizer();
        Token lastToken = null;

        void Clear()
        {
            LastNode = null;
            tokens.Clear();
            lastToken = null;
        }

        void PopToken()
        {
            lastToken = tokens.Dequeue();
        }

        Token Next()
        {
            if (tokens.Count == 0)
            {
                return null;
            }

            return tokens.Peek();
        }

        AstNode Primary()
        {
            PopToken();
            return MakeLeaf(lastToken.Type, lastToken.Precedence, lastToken.Value, lastToken.Text);
        }

        AstNode BinExpr(int precGrp)
        {
            AstNode left = Primary();

            if (Next() == null || Next().Type == TokenTypes.Semicolon)
            {
                return left;
            }

            while (Token.PrattPrecedence(Next().Precedence) > precGrp)
            {
                PopToken();

                TokenTypes type = lastToken.Type;
                Precedence precedence = lastToken.Precedence;
                dynamic value = lastToken.Value;
                string text = lastToken.Text;

                AstNode right = BinExpr(Token.PrattPrecedence(lastToken.Precedence));
                left = MakeNode(type, precedence, value, text, left, right);

                if (Next() == null || Next().Type == TokenTypes.Semicolon)
                {
                    return left;
                }
            }

            return left;
        }
        
        public List<int> BuildTree(string input)
        {
            Clear();
            tokens = tokenizer.Tokenize(input);
            List<int> results = new List<int>();

            while (true)
            {
                AstNode root = BinExpr(0);
                int result = InterpretTree(root);
                results.Add(result);

                if (Next() == null)
                {
                    break;
                }
            }

            return results;
        }

        int InterpretTree(AstNode rootNode)
        {
            int left = 0;
            int right = 0;

            if (rootNode.Left != null)
            {
                left = InterpretTree(rootNode.Left);
            }

            if (rootNode.Right != null)
            {
                right = InterpretTree(rootNode.Right);
            }

            if (rootNode.Type == TokenTypes.Operator)
            {
                if (rootNode.Precedence == Precedence.Plus || rootNode.Precedence == Precedence.UnaryPlus)
                {
                    return left + right;
                } 

                if (rootNode.Precedence == Precedence.Minus || rootNode.Precedence == Precedence.UnaryMinus)
                {
                    return left - right;
                } 

                if (rootNode.Precedence == Precedence.Times)
                {
                    return left * right;
                } 

                if (rootNode.Precedence == Precedence.Divide)
                {
                    return left / right;
                } 
            }

            return rootNode.Value;
        }

        AstNode MakeNode(TokenTypes Type, Precedence Precedence, dynamic Value, string Text, AstNode Left, AstNode Right)
        {
            return new AstNode {Type = Type, Precedence = Precedence, Value = Value, Text = Text, Left = Left, Right = Right};
        }

        AstNode MakeLeaf(TokenTypes Type, Precedence Precedence, dynamic Value, string Text)
        {
            return new AstNode {Type = Type, Precedence = Precedence, Value = Value, Text = Text};
        }

        AstNode MakeUnary(TokenTypes Type, Precedence Precedence, dynamic Value, string Text, AstNode Left)
        {
            return new AstNode {Type = Type, Precedence = Precedence, Value = Value, Text = Text, Left = Left};
        }
    }

    public class AstNode
    {
        public AstNode Left {get; set;}
        public AstNode Right {get; set;}
        public dynamic Value {get; set;}
        public string Text { get; set; }
        public Precedence Precedence { get; set; }
        public TokenTypes Type {get; set;}

        public AstNode()
        {

        }
    }
}
