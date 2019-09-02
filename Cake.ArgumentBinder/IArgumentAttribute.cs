//
// Copyright Seth Hendrick 2019.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

namespace Cake.ArgumentBinder
{
    public interface IReadOnlyArgumentAttribute
    {
        // ---------------- Properties ----------------

        /// <summary>
        /// The argument name 
        /// </summary>
        string ArgName { get; }

        /// <summary>
        /// Description of what the argument does.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// If the value is not specified, this will fail validation.
        /// </summary>
        bool Required { get; }
    }
}
