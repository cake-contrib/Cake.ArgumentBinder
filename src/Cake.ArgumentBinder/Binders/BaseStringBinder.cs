//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System.Reflection;
using Cake.Core;

namespace Cake.ArgumentBinder.Binders
{
    internal abstract class BaseStringBinder<TInstance, TAttribute> : BaseBinder<TInstance, TAttribute>
        where TAttribute : BaseStringAttribute
    {
        // ---------------- Fields ----------------

        // ---------------- Constructor ----------------

        protected BaseStringBinder( ICakeContext cakeContext ) :
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
                cakeArg = attribute.DefaultValue;
            }

            propertyInfo.SetValue(
                instance,
                cakeArg
            );
        }

        protected abstract bool HasArgument( string argumentName );

        protected abstract string GetArgument( string argumentName );
    }
}
