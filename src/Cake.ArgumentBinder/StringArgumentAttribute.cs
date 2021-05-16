//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.Text;

namespace Cake.ArgumentBinder
{
    /// <summary>
    /// This is an argument that should be limited to an string.
    /// 
    /// Note that this attribute can only be attached to a property whose type is a string,
    /// or run-time exceptions will occur.
    /// </summary>
    public sealed class StringArgumentAttribute : BaseAttribute
    {
        // ---------------- Constructor ----------------

        public StringArgumentAttribute( string argumentName ) :
            base( argumentName )
        {
            this.DefaultValue = string.Empty;
        }

        // ---------------- Properties ----------------

        public string DefaultValue { get; set; }

        protected override object BaseDefaultValue
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
                return typeof( string );
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
