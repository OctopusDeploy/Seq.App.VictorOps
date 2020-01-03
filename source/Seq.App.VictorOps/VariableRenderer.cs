using System.Collections.Generic;
using Octostache;

namespace Seq.App.VictorOps
{
    public static class VariableRenderer 
    {
        public static string Render(string input, IDictionary<string, string> variables)
        {
            var variableDict = new VariableDictionary();
            foreach (var v in variables)
            {
                variableDict[v.Key] = v.Value;
            }
        
            return variableDict.Evaluate(input, out var error, false);
        }
    }
}