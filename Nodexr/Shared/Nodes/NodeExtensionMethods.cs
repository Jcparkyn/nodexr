using System.Linq;
using System.Collections.Generic;
using BlazorNodes.Core;

namespace Nodexr.Shared.Nodes
{
    public static class NodeExtensionMethods
    {
        /// <summary>
        /// Checks whether the output of the node is dependent on the value of the given input.
        /// Used for finding cyclic dependencies.
        /// </summary>
        /// <returns>True if there is a dependency, false otherwise</returns>
        public static bool IsDependentOn(this INodeViewModel that, INodeInput childInput)
        {
            return GetAllProceduralInputsRecursive(that).Contains(childInput);

            static IEnumerable<IInputPort> GetAllProceduralInputsRecursive(INodeViewModel parent)
            {
                foreach (var input in parent.GetAllInputs().OfType<IInputPort>())
                {
                    yield return input;

                    if (input.ConnectedNodeUntyped is INodeViewModel childNode)
                    {
                        foreach (var input2 in GetAllProceduralInputsRecursive(childNode))
                            yield return input2;
                    }
                }
            }
        }
    }
}
