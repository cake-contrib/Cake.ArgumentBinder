//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System.Reflection;
using Cake.Core;

namespace Cake.ArgumentBinder.Binders
{
    internal sealed class StringArgumentBinder<TInstance> : BaseBinder<TInstance, StringArgumentAttribute>
    {
        // ---------------- Fields ----------------

        // ---------------- Constructor ----------------

        public StringArgumentBinder( ICakeContext cakeContext ) :
            base( cakeContext )
        {
        }

        // ---------------- Functions ----------------

        protected sealed override void BindInternal( TInstance instance, PropertyInfo propertyInfo, StringArgumentAttribute attribute )
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
    }
}
