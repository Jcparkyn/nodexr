using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RegexNodes.Shared.NodeTypes
{
    public class RegexRaw : Node
    {
        public override string Title => "Raw Regex";
        public override string NodeInfo => "[Obsolete: replaced by 'Text' node] Outputs characters exactly as you type them.";

        
        [NodeInput]
        protected InputString Input { get; } = new InputString("");

        public RegexRaw(string contents = "")
        {
            Input.InputContents = contents;
        }

        protected override string GetValue()
        {
            string result = Input.GetValue();
            //Thread.Sleep(500);
            CachedValue = result;
            return result;
        }
    }
}
