using System.Linq;
using System.Collections.Generic;
using Nodexr.Shared.NodeInputs;

namespace Nodexr.Shared.Nodes
{
    public static class NodeExtensionMethods
    {
        /// <summary>
        /// Get all of the inputs to the node, including the 'previous' input and the sub-inputs of any InputCollections.
        /// InputCollections themselves are not returned.
        /// </summary>
        public static IEnumerable<NodeInput> GetAllInputs(this INode that)
        {
            yield return that.Previous;
            foreach (var input in that.NodeInputs)
            {
                if (input is InputCollection coll)
                {
                    foreach (var subInput in coll.Inputs)
                        yield return subInput;
                }
                else
                {
                    yield return input;
                }
            }
        }

        /// <summary>
        /// Checks whether the output of the node is dependent on the value of the given input.
        /// Used for finding cyclic dependencies.
        /// </summary>
        /// <returns>True if there is a dependency, false otherwise</returns>
        public static bool IsDependentOn(this INode that, INodeInput childInput)
        {
            return GetAllProceduralInputsRecursive(that).Any(input => input == childInput);

            static IEnumerable<InputProcedural> GetAllProceduralInputsRecursive(INode parent)
            {
                foreach (var input in parent.GetAllInputs().OfType<InputProcedural>())
                {
                    yield return input;

                    if (input.ConnectedNode is INode childNode)
                    {
                        foreach (var input2 in GetAllProceduralInputsRecursive(childNode))
                            yield return input2;
                    }
                }
            }
        }
    }
}
