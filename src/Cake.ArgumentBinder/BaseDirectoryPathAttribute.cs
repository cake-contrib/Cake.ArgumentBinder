//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.Text;
using Cake.Core.IO;

namespace Cake.ArgumentBinder
{
    public abstract class BaseDirectoryPathAttribute : BasePathAttribute
    {
        // ---------------- Constructor ----------------

        protected BaseDirectoryPathAttribute( string arg ) :
            base( arg )
        {
        }

        // ---------------- Properties ----------------

        protected override Type BaseType
        {
            get
            {
                return typeof( DirectoryPath );
            }
        }
    }
}
