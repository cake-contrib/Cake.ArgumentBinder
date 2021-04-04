//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cake.ArgumentBinder
{
    public abstract class BaseEnumAttribute : BaseAttribute
    {
        // ---------------- Fields ----------------

        private readonly Type enumType;

        // ---------------- Constructor ----------------

        protected BaseEnumAttribute( Type enumType, string arg ) :
            base( arg )
        {
            if( enumType.IsEnum == false )
            {
                throw new ArgumentException(
                    "Passed in type is not an Enum",
                    nameof( enumType )
                );
            }

            this.enumType = enumType;

            foreach( Enum e in Enum.GetValues( enumType ) )
            {
                this.DefaultValue = e;
                break;
            }
        }

        // ---------------- Properties ----------------

        /// <summary>
        /// The default value of the argument.  This is set to the default
        /// value of the enum, which is the value set to 0.
        /// </summary>
        /// <remarks>
        /// You can not set this to a specific value because of how attributes work.
        /// <see cref="Enum"/> is not a compile-time constant, so it can not be set
        /// when creating an attribute.  The best we can do for a default value is
        /// the enum set to 0.
        /// </remarks>
        public Enum DefaultValue { get; private set; }

        protected sealed override object BaseDefaultValue => this.DefaultValue;

        internal sealed override Type BaseType => this.enumType;

        // ---------------- Functions ----------------

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            this.ToString( builder );

            if( this.HasSecretValue == false )
            {
                builder.AppendLine( "\t\tPossible Values:" );
                foreach( Enum e in Enum.GetValues( this.BaseType ) )
                {
                    builder.AppendLine( $"\t\t\t- {e}" );
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Validates this object.  Returns <see cref="string.Empty"/>
        /// if nothing is wrong, otherwise this returns an error message.
        /// </summary>
        internal string TryValidate()
        {
            StringBuilder builder = new StringBuilder();
            if( string.IsNullOrWhiteSpace( this.ArgName ) )
            {
                builder.AppendLine( nameof( this.ArgName ) + " can not be null, empty, or whitespace." );
            }
            else if( this.DefaultValue == null )
            {
                builder.AppendLine(
                    $"{nameof( this.DefaultValue )} is null, is the enum type, {this.BaseType.Name}, an empty enum?"
                );
            }

            return builder.ToString();
        }
    }
}
