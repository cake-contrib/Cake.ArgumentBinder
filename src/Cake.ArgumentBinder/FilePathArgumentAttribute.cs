//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

namespace Cake.ArgumentBinder
{
    public sealed class FilePathArgumentAttribute : BaseFilePathAttribute, IReadOnlyArgumentAttribute
    {
        // ---------------- Constructor ----------------

        public FilePathArgumentAttribute( string argumentName ) : 
            base( argumentName )
        {
        }
    }
}
