//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;

namespace Cake.ArgumentBinder
{
    /// <summary>
    /// This is an argument that should be limited to an integer.
    /// 
    /// Note that this attribute can only be attached to a property whose type is an int,
    /// or run-time exceptions will occur.
    /// </summary>
    [AttributeUsage( AttributeTargets.Property, Inherited = true, AllowMultiple = false )]
    public class IntegerArgumentAttribute : BaseIntegerAttribute, IReadOnlyArgumentAttribute
    {
        // ---------------- Constructor ----------------

        public IntegerArgumentAttribute( string arg ) :
            base( arg )
        {
        }
    }
}
