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

namespace Mathos.Parser.Scripting
{
    /// <summary>
    /// An implementation of <see cref="IScriptParserLog"/> that doesn't do anything with logs
    /// </summary>
    public sealed class NullScriptParserLog : IScriptParserLog
    {
        public void Log(string log)
        {
            //don't do anything with logs in this class
        }
    }
}
