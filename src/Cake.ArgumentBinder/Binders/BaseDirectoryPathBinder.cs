//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System.IO;
using System.Reflection;
using Cake.Core;
using Cake.Core.IO;

namespace Cake.ArgumentBinder.Binders
{
    internal abstract class BaseDirectoryPathBinder<TInstance, TAttribute> : BaseBinder<TInstance, TAttribute>
        where TAttribute : BaseDirectoryPathAttribute
    {
        // ---------------- Fields ----------------

        // ---------------- Constructor ----------------

        protected BaseDirectoryPathBinder( ICakeContext cakeContext ) :
            base( cakeContext )
        {
        }

        // ---------------- Functions ----------------

        protected sealed override void BindInternal( TInstance instance, PropertyInfo propertyInfo, TAttribute attribute )
        {
            string cakeArg;
            if( this.HasArgument( attribute.ArgName ) )
            {
                cakeArg = this.GetArgument( attribute.ArgName );
            }
            else if( attribute.Required )
            {
                throw new MissingRequiredArgumentException( attribute.ArgName );
            }
            else
            {
                cakeArg = attribute.DefaultValue?.ToString() ?? null;
            }

            DirectoryPath value = ( cakeArg != null ) ? new DirectoryPath( cakeArg ) : null;

            if( attribute.MustExist && ( value == null ) )
            {
                throw new ArgumentValueNullException(
                    attribute.ArgName
                );
            }

            if( attribute.MustExist )
            {
                IDirectory file = cakeContext.FileSystem.GetDirectory( value );
                if( ( file == null ) || ( file.Exists == false ) )
                {
                    throw new DirectoryNotFoundException(
                        "Directory must exist before executing cake task."
                    );
                }
            }

            propertyInfo.SetValue(
                instance,
                value
            );
        }

        protected abstract bool HasArgument( string argumentName );

        protected abstract string GetArgument( string argumentName );
    }
}
