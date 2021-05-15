//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System.Reflection;
using Cake.Core;

namespace Cake.ArgumentBinder.Binders
{
    internal abstract class BaseBooleanBinder<TInstance, TAttribute> : BaseBinder<TInstance, TAttribute>
        where TAttribute : BaseBooleanAttribute
    {
        // ---------------- Fields ----------------

        // ---------------- Constructor ----------------

        protected BaseBooleanBinder( ICakeContext cakeContext ) :
            base( cakeContext )
        {
        }

        // ---------------- Functions ----------------

        protected sealed override void BindInternal( TInstance instance, PropertyInfo propertyInfo, TAttribute attribute )
        {
            bool? value = null;
            string cakeArg;
            if( this.HasArgument( attribute.ArgName ) )
            {
                cakeArg = this.GetArgument( attribute.ArgName );
                if( bool.TryParse( cakeArg, out bool result ) )
                {
                    value = result;
                }
                else
                {
                    throw new ArgumentFormatException( typeof( bool ), attribute.ArgName );
                }
            }

            if( attribute.Required && ( value == null ) )
            {
                throw new MissingRequiredArgumentException( attribute.ArgName );
            }

            if( value == null )
            {
                value = attribute.DefaultValue;
            }

            propertyInfo.SetValue(
                instance,
                value.Value
            );
        }

        protected abstract bool HasArgument( string argumentName );

        protected abstract string GetArgument( string argumentName );
    }
}
