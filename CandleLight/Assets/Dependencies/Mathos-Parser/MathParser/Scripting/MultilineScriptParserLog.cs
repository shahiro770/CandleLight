/* 
 * Copyright (C) 2012-2019, Mathos Project.
 * All rights reserved.
 * 
 * Please see the license file in the project folder
 * or go to https://github.com/MathosProject/Mathos-Parser/blob/master/LICENSE.md.
 * 
 * Please feel free to ask me directly at my email!
 *  artem@artemlos.net
 */

using System;
using System.Text;

namespace Mathos.Parser.Scripting
{
    /// <summary>
    /// An implementation of <see cref="IScriptParserLog"/> that appends logs to a multiline string
    /// </summary>
    public class MultilineScriptParserLog : IScriptParserLog
    {
        private StringBuilder sb;
        public MultilineScriptParserLog()
        {
            sb = new StringBuilder();
        }
        public string Output { get { return sb.ToString(); } }
        public void Log(string log)
        {
            if (Output.Length > 0)
            {
                sb.Append(Environment.NewLine);
            }
            sb.Append(log);
        }

        public void Clear()
        {
            sb.Clear();
        }
    }
}
