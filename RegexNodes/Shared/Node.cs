using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegexNodes.Shared
{
    public interface INode : IPositionable
    {
        string GetValue();
        string Title { get; }
        string NodeInfo { get; }
        bool IsCollapsed { get; set; }

        string CachedValue { get; set; }
        string GetValueAndUpdateCache();

        List<INodeInput> NodeInputs { get; }

        Vector2L OutputPos { get; }

        void MoveBy(long x, long y);
        void MoveBy(Vector2L delta);
        void CalculateInputsPos();
    }

    public abstract class Node : INode
    {
        private Vector2L pos;

        public Vector2L Pos
        {
            get => pos;
            set
            {
                pos = value;
                CalculateInputsPos();
            }
        }

        public abstract List<INodeInput> NodeInputs { get; }
        public abstract string Title { get; }
        public abstract string NodeInfo { get; }

        public string CachedValue { get; set; } = "";

        public Vector2L OutputPos => Pos + new Vector2L(150, 14);

        public bool IsCollapsed { get; set; }


        [Obsolete("Update cache in derived class instead")]
        public string GetValueAndUpdateCache()
        {
            string value = GetValue();
            //Console.WriteLine("Updating cache value to " + value);
            CachedValue = value;
            return value;
        }

        /// <summary>
        /// Set the position of each input based on the position of the node
        /// </summary>
        public void CalculateInputsPos()
        {
            //TODO: refactor using GetHeight() on each input
            if (IsCollapsed)
            {
                int startHeight = 13;
                foreach (var input in NodeInputs)
                {
                    switch (input)
                    {
                        case InputProcedural input1:
                            input1.Pos = new Vector2L(Pos.x + 2, Pos.y + startHeight);
                            break;
                        case InputCollection input1:
                            input1.Pos = new Vector2L(Pos.x + 2, Pos.y + startHeight);
                            foreach (var input2 in input1.Inputs)
                            {
                                input2.Pos = new Vector2L(Pos.x + 2, Pos.y + startHeight);
                            }
                            break;
                    }
                }
            }
            else
            {
                int startHeight = 47;
                int inputHeight = 35;
                foreach (var input in NodeInputs)
                {
                    switch (input)
                    {
                        case InputProcedural input1:
                            input1.Pos = new Vector2L(Pos.x, Pos.y + startHeight);
                            startHeight += inputHeight;
                            break;
                        case InputCollection input1:
                            startHeight += 30;
                            input1.Pos = new Vector2L(Pos.x, Pos.y + startHeight);
                            foreach (var input2 in input1.Inputs)
                            {
                                input2.Pos = new Vector2L(Pos.x, Pos.y + startHeight);
                                startHeight += inputHeight;
                            }
                            break;
                    }
                }
            }
        }

        public string UpdateCache(string result)
        {
            CachedValue = result;
            return result;
        }

        //public NodeOutput NodeOutput { get; set; } = new NodeOutput();

        public void MoveBy(long x, long y)
        {
            Pos = new Vector2L(Pos.x + x, Pos.y + y);
            CalculateInputsPos();
        }
        public void MoveBy(Vector2L delta) => MoveBy(delta.x, delta.y);

        public abstract string GetValue();
    }
}
