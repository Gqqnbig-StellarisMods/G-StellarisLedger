using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime.Misc;

namespace ConsoleApp2
{
    class Loader: ParadoxBaseListener
    {
        public override void EnterAtom([NotNull] ParadoxParser.AtomContext context)
        {
            if (context.Parent.RuleIndex == ParadoxParser.ID)
            {

            }
        }

        public override void EnterParadox([NotNull] ParadoxParser.ParadoxContext context)
        {
            //context.ch
            base.EnterParadox(context);
        }

    }
}
