//
// Copyright Seth Hendrick 2019-2022.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.Reflection;
using Cake.Core;

namespace Cake.ArgumentBinder.Binders
{
    internal sealed class EnumArgumentBinder<TInstance> : BaseBinder<TInstance, EnumArgumentAttribute>
    {
        // ---------------- Constructor ----------------

        public EnumArgumentBinder( ICakeContext cakeContext ) :
            base( cakeContext )
        {
        }

        // ---------------- Functions ----------------

        protected sealed override void BindInternal( TInstance instance, PropertyInfo propertyInfo, EnumArgumentAttribute attribute )
        {
            Enum value = null;
            string cakeArg;
            if( this.HasArgument( attribute.ArgName, attribute ) )
            {
                cakeArg = this.GetArgument( attribute.ArgName, attribute );

                // No TryParse (well, no TryParse that doesn't require a generic).
                // Need to do a try{} catch{} :/.
                try
                {
                    value = Enum.Parse( attribute.BaseType, cakeArg, attribute.IgnoreCase ) as Enum;
                }
                catch( ArgumentException )
                {
                    throw new ArgumentFormatException( attribute.BaseType, attribute.ArgName );
                }

                if( value == null )
                {
                    throw new ArgumentFormatException( attribute.BaseType, attribute.ArgName );
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
                value
            );
        }
    }
}
