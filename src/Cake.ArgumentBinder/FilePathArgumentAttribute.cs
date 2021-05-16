//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using Cake.Core.IO;

namespace Cake.ArgumentBinder
{
    public sealed class FilePathArgumentAttribute : BasePathAttribute
    {
        // ---------------- Constructor ----------------

        public FilePathArgumentAttribute( string argumentName ) :
            base( argumentName )
        {
        }

        // ---------------- Properties ----------------

        internal sealed override Type BaseType
        {
            get
            {
                return typeof( FilePath );
            }
        }
    }
}
