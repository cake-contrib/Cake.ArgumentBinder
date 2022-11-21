//
// Copyright Seth Hendrick 2019-2022.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System.Reflection;
using Cake.Core;

namespace Cake.ArgumentBinder.Binders
{
    internal sealed class BooleanArgumentBinder<TInstance> : BaseBinder<TInstance, BooleanArgumentAttribute>
    {
        // ---------------- Constructor ----------------

        public BooleanArgumentBinder( ICakeContext cakeContext ) :
            base( cakeContext )
        {
        }

        // ---------------- Functions ----------------

        protected sealed override void BindInternal( TInstance instance, PropertyInfo propertyInfo, BooleanArgumentAttribute attribute )
        {
            bool? value = null;
            string cakeArg;
            if( this.HasArgument( attribute.ArgName, attribute ) )
            {
                cakeArg = this.GetArgument( attribute.ArgName, attribute );
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
    }
}
