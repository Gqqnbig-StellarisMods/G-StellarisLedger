using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var sr = new StreamReader(@"D:\Documents\Paradox Interactive\Stellaris\save games\2_333029590\2259.05.18\gamestate", System.Text.Encoding.UTF8))
            {
                var data = MyParseMethod(sr.ReadToEnd());
                Console.WriteLine(data.Count);
                
            }
            Console.ReadKey();
        }

        public static IList<IParseTree> MyParseMethod(string input)
        {
            //ICharStream cstream = CharStreams.fromStream(stream);
            ICharStream cstream = CharStreams.fromstring(input);
            ITokenSource lexer = new ParadoxLexer(cstream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            var parser = new ParadoxParser(tokens);


            var listen = new ParadoxBaseListener();
            return parser.paradox().children;


        }
    }
}