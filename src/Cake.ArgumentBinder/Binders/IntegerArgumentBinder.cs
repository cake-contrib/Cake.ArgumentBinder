//
// Copyright Seth Hendrick 2019-2022.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System.Reflection;
using Cake.Core;

namespace Cake.ArgumentBinder.Binders
{
    internal sealed class IntegerArgumentBinder<TInstance> : BaseBinder<TInstance, IntegerArgumentAttribute>
    {
        // ---------------- Constructor ----------------

        public IntegerArgumentBinder( ICakeContext cakeContext ) :
            base( cakeContext )
        {
        }

        // ---------------- Functions ----------------

        protected sealed override void BindInternal( TInstance instance, PropertyInfo propertyInfo, IntegerArgumentAttribute attribute )
        {
            int? value = null;
            string cakeArg;
            if( this.HasArgument( attribute.ArgName, attribute ) )
            {
                cakeArg = this.GetArgument( attribute.ArgName, attribute );
                if( int.TryParse( cakeArg, out int result ) )
                {
                    if( result > attribute.Max )
                    {
                        throw new ArgumentTooLargeException( attribute.Max.ToString(), attribute.ArgName );
                    }
                    if( result < attribute.Min )
                    {
                        throw new ArgumentTooSmallException( attribute.Min.ToString(), attribute.ArgName );
                    }

                    value = result;
                }
                else
                {
                    throw new ArgumentFormatException( typeof( int ), attribute.ArgName );
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
