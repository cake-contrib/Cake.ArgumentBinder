//
// Copyright Seth Hendrick 2019.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;

namespace Cake.ArgumentBinder
{
    /// <summary>
    /// This is an argument that should be limited to an string.
    /// 
    /// Note that this attribute can only be attached to a property whose type is a string,
    /// or run-time exceptions will occur.
    /// </summary>
    [AttributeUsage( AttributeTargets.Property, Inherited = true, AllowMultiple = false )]
    public class StringArgumentAttribute : BaseStringAttribute, IReadOnlyArgumentAttribute
    {
        // ---------------- Constructor ----------------

        public StringArgumentAttribute( string arg ) :
            base( arg )
        {
        }
    }
}
