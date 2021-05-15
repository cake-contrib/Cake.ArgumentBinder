//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using Cake.Core;

namespace Cake.ArgumentBinder.Binders.Argument
{
    internal sealed class ArgumentFilePathBinder<TInstance> : BaseFilePathBinder<TInstance, FilePathArgumentAttribute>
    {
        // ---------------- Constructor ----------------

        public ArgumentFilePathBinder( ICakeContext cakeContext ) :
            base( cakeContext )
        {
        }

        // ---------------- Functions ----------------

        protected override bool HasArgument( string argumentName )
        {
            return this.cakeContext.Arguments.HasArgument( argumentName );
        }

        protected override string GetArgument( string argumentName )
        {
            return this.cakeContext.Arguments.GetArgument( argumentName );
        }
    }
}
