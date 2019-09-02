//
// Copyright Seth Hendrick 2019.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.Text;

namespace Cake.ArgumentBinder
{
    public class BaseIntegerAttribute : BaseAttribute
    {
        // ---------------- Constructor ----------------

        protected BaseIntegerAttribute( string arg ) :
            base( arg )
        {
            this.DefaultValue = 0;
            this.Min = 0;
            this.Max = int.MaxValue;
        }

        // ---------------- Properties ----------------

        /// <summary>
        /// The default value of the argument; defaulted to 0.
        /// 
        /// The default value IS allowed to be outside of the range specified
        /// in <see cref="Min"/> and <see cref="Max"/>.
        /// 
        /// This is ignored if <see cref="Required"/> is flagged as true,
        /// as the value MUST be specified in this case.
        /// </summary>
        public int DefaultValue { get; set; }

        /// <summary>
        /// The minimum acceptable value.  Defaulted to 0.
        /// If the argument is less than (equal to is okay) this value,
        /// validation will fail.
        /// </summary>
        public int Min { get; set; }

        /// <summary>
        /// The maximum acceptable value.  Defaulted to <see cref="int.MaxValue"/>.
        /// If the argument is greater than (equal to is okay) this value,
        /// validation will fail.
        /// </summary>
        public int Max { get; set; }

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
                return typeof( int );
            }
        }

        // ---------------- Functions ----------------

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            this.ToString( builder );
            builder.AppendLine( "\t\tMinimum Value: " + this.Min );
            builder.AppendLine( "\t\tMaximum Value: " + this.Max );

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

            if ( this.Min > this.Max )
            {
                builder.AppendLine( nameof( this.Min ) + " can not be greater than the " + nameof( this.Max ) );
            }

            return builder.ToString();
        }
    }
}