using System.Linq;
using System.Collections.Generic;
using Nodexr.Shared.NodeInputs;

namespace Nodexr.Shared.Nodes
{
    public static class NodeExtensionMethods
    {
        /// <summary>
        /// Checks whether the output of the node is dependent on the value of the given input.
        /// Used for finding cyclic dependencies.
        /// </summary>
        /// <returns>True if there is a dependency, false otherwise</returns>
        public static bool IsDependentOn(this RegexNodeViewModelBase that, INodeInput childInput)
        {
            return GetAllProceduralInputsRecursive(that).Any(input => input == childInput);

            static IEnumerable<InputProcedural> GetAllProceduralInputsRecursive(RegexNodeViewModelBase parent)
            {
                foreach (var input in parent.GetAllInputs().OfType<InputProcedural>())
                {
                    yield return input;

                    if (input.ConnectedNode is RegexNodeViewModelBase childNode)
                    {
                        foreach (var input2 in GetAllProceduralInputsRecursive(childNode))
                            yield return input2;
                    }
                }
            }
        }
    }
}
