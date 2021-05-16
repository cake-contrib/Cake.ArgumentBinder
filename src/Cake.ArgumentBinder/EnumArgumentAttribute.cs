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
    /// <summary>
    /// This class helps bind an enum to an argument.  This allows one to limit
    /// the choices a user is able to pass into the property.
    /// </summary>
    /// <remarks>
    /// Any enum can be used that follows the following rules.  If any
    /// of these rules are broken, the attribute will not validate,
    /// and an exception will be thrown when the binding is attempted:
    /// 
    /// - The enum must contain at least one value within it.
    ///   Empty enums are not allowed.
    /// - If <see cref="BaseAttribute.Required"/> is set to false,
    ///   the enum must have a value set to 0.  0 is the default value
    ///   for an enum, and its the only way we can define a default enum value
    ///   with <see cref="Attribute"/> objects.
    /// </remarks>
    public sealed class EnumArgumentAttribute : BaseAttribute
    {
        // ---------------- Fields ----------------

        private readonly Type enumType;

        internal static readonly string PossibleValuePrefix = "Possible values";
        internal static readonly string CasingIgnorePrefix = "Casing is ignored";

        // ---------------- Constructor ----------------

        public EnumArgumentAttribute( Type enumType, string argumentName ) :
            base( argumentName )
        {
            if( enumType.IsEnum == false )
            {
                throw new ArgumentException(
                    "Passed in type is not an Enum",
                    nameof( enumType )
                );
            }

            this.enumType = enumType;

            this.IgnoreCase = false;
            this.DefaultValue = (Enum)Activator.CreateInstance( enumType );
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
        /// the enum value set to 0.
        /// 
        /// If <see cref="BaseAttribute.Required"/> is set to false, and 
        /// if an enum does not have a value equal to 0, a validation error will happen.
        /// </remarks>
        public Enum DefaultValue { get; private set; }

        /// <summary>
        /// When parsing the enum values, ignore all casing.
        /// This is defaulted to false.
        /// 
        /// If this is set to true, and there are two enum values that, when casing is ignored,
        /// match, a validation error will happen.
        /// </summary>
        public bool IgnoreCase { get; set; }

        protected sealed override object BaseDefaultValue => this.DefaultValue;

        internal sealed override Type BaseType => this.enumType;

        // ---------------- Functions ----------------

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            this.ToString( builder );

            builder.AppendLine( $"\t\t{CasingIgnorePrefix}: {this.IgnoreCase}" );

            if( this.HasSecretValue == false )
            {
                builder.AppendLine( $"\t\t{PossibleValuePrefix}:" );
                foreach( Enum e in Enum.GetValues( this.BaseType ) )
                {
                    builder.AppendLine( $"\t\t\t- {e}" );
                }
            }

            return builder.ToString();
        }

        /// <inheritdoc/>
        internal override string TryValidate()
        {
            StringBuilder builder = new StringBuilder();
            if( string.IsNullOrWhiteSpace( this.ArgName ) )
            {
                builder.AppendLine( nameof( this.ArgName ) + " can not be null, empty, or whitespace." );
            }

            Array values = Enum.GetValues( this.enumType );
            if( values.Length == 0 )
            {
                builder.AppendLine(
                    $"There a no values contained within enum type {this.enumType.Name}.  At least one value must be specified."
                );
            }

            if( this.Required == false )
            {
                bool foundDefaultValue = false;
                foreach( Enum value in values )
                {
                    if( value.Equals( this.DefaultValue ) )
                    {
                        foundDefaultValue = true;
                        break;
                    }
                }

                if( foundDefaultValue == false )
                {
                    builder.AppendLine(
                        $"{nameof(Required)} is set to {false}, but there is no value contained within the enum equal to 0, to represent a default value."
                    );
                }
            }

            if( this.IgnoreCase )
            {
                List<string> listedValues = new List<string>();
                foreach( Enum value in values )
                {
                    listedValues.Add( value.ToString().ToLowerInvariant() );
                }

                if( listedValues.Distinct().Count() != listedValues.Count )
                {
                    builder.AppendLine(
                        $"Since {nameof( this.IgnoreCase )} is set to true, there are multiple values an argument can be assigned to."
                    );
                    builder.AppendLine(
                        "Please ensure all enum values are unique when casing is ignored."
                    );
                }
            }

            return builder.ToString();
        }
    }
}
