//
// Copyright Seth Hendrick 2019.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.Text;

namespace Cake.ArgumentBinder
{
    public class BaseStringAttribute : BaseAttribute
    {
        // ---------------- Constructor ----------------

        protected BaseStringAttribute( string arg ) :
            base( arg )
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

        protected override Type BaseType
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

        /// <summary>
        /// Validates this object.  Returns <see cref="string.Empty"/>
        /// if nothing is wrong, otherwise this returns an error message.
        /// </summary>
        internal string TryValidate()
        {
            StringBuilder builder = new StringBuilder();
            if ( string.IsNullOrWhiteSpace( this.ArgName ) )
            {
                builder.AppendLine( nameof( this.ArgName ) + " can not be null, empty, or whitespace." );
            }

            return builder.ToString();
        }
    }
}