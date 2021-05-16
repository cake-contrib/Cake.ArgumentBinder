//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using Cake.Core.IO;

namespace Cake.ArgumentBinder
{
    public sealed class DirectoryPathArgumentAttribute : BasePathAttribute
    {
        // ---------------- Constructor ----------------

        public DirectoryPathArgumentAttribute( string argumentName ) :
            this( argumentName, DefaultArgumentSource )
        {
        }

        public DirectoryPathArgumentAttribute( string argumentName, ArgumentSource argumentSource ) :
            base( argumentName, argumentSource )
        {
        }

        // ---------------- Properties ----------------

        internal sealed override Type BaseType
        {
            get
            {
                return typeof( DirectoryPath );
            }
        }
    }
}
