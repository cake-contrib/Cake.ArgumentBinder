//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;

namespace Cake.ArgumentBinder
{
    /// <inheritdoc/>
    [AttributeUsage( AttributeTargets.Property, Inherited = true, AllowMultiple = false )]
    public sealed class EnumArgumentAttribute : BaseEnumAttribute, IReadOnlyArgumentAttribute
    {
        // ---------------- Constructor ----------------

        public EnumArgumentAttribute( Type enumType, string argName ) :
            base( enumType, argName )
        {
        }
    }
}
