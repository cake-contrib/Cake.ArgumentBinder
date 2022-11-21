//
// Copyright Seth Hendrick 2019-2022.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.Text;

namespace Cake.ArgumentBinder
{
    /// <summary>
    /// This is an argument that should be limited to an boolean.
    /// 
    /// Note that this attribute can only be attached to a property whose type is an bool,
    /// or run-time exceptions will occur.
    /// </summary>
    public sealed class BooleanArgumentAttribute : BaseAttribute
    {
        // ---------------- Constructor ----------------

        public BooleanArgumentAttribute( string argumentName ) :
            this( argumentName, DefaultArgumentSource )
        {
        }

        public BooleanArgumentAttribute( string argumentName, ArgumentSource argumentSource ) :
            base( argumentName, argumentSource )
        {
            this.DefaultValue = false;
        }

        // ---------------- Properties ----------------

        /// <summary>
        /// The default value of the argument; defaulted to false.
        /// </summary>
        public bool DefaultValue { get; set; }

        protected sealed override object BaseDefaultValue
        {
            get
            {
                return this.DefaultValue;
            }
        }

        internal sealed override Type BaseType
        {
            get
            {
                return typeof( bool );
            }
        }

        // ---------------- Functions ----------------

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            this.ToString( builder );

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

            return builder.ToString();
        }
    }
}
