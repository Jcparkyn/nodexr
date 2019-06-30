using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RegexNodes.Shared.NodeTypes
{
    public class RegexRaw : Node
    {
        public override string Title => "Raw Regex";
        public override string NodeInfo => "[Obsolete: replaced by 'Text' node] Outputs characters exactly as you type them.";

        public override List<INodeInput> NodeInputs
        {
            get
            {
                return new List<INodeInput> { Input };
            }
        }

        protected InputString Input { get; set; } = new InputString("");

        public RegexRaw() { }
        public RegexRaw(string contents)
        {
            Input.InputContents = contents;
        }

        public override string GetValue()
        {
            string result = Input.GetValue();
            //Thread.Sleep(500);
            CachedValue = result;
            return result;
        }
    }
}
