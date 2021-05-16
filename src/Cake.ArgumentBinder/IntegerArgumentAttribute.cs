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
    /// This is an argument that should be limited to an integer.
    /// 
    /// Note that this attribute can only be attached to a property whose type is an int,
    /// or run-time exceptions will occur.
    /// </summary>
    public sealed class IntegerArgumentAttribute : BaseAttribute
    {
        // ---------------- Fields ----------------

        internal static readonly string MinValuePrefix = "Minimum Value";
        internal static readonly string MaxValuePrefix = "Maximum Value";

        // ---------------- Constructor ----------------

        public IntegerArgumentAttribute( string argumentName ) :
            base( argumentName )
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
        /// </summary>
        public int DefaultValue { get; set; }

        /// <summary>
        /// The minimum acceptable value.  Defaulted to 0.
        /// If the argument is less than (equal to is okay) this value,
        /// validation will fail.
        /// </summary>
        /// <remarks>
        /// 0 is chosen to be the default value since most arguments
        /// (in the author's experience) usually are never negative.
        /// </remarks>
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

        internal sealed override Type BaseType
        {
            get
            {
                return typeof( int );
            }
        }

        // ---------------- Functions ----------------

        /// <remarks>
        /// <see cref="Min"/> and <see cref="Max"/> will be hidden
        /// if <see cref="BaseAttribute.HasSecretValue"/> is set to true.
        /// We don't want anyone to guess what a valid range of the value could be.
        /// </remarks>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            this.ToString( builder );

            if( this.HasSecretValue )
            {
                builder.AppendLine( $"\t\t{MinValuePrefix}: {ArgumentBinder.HiddenString}" );
                builder.AppendLine( $"\t\t{MaxValuePrefix}: {ArgumentBinder.HiddenString}" );
            }
            else
            {
                builder.AppendLine( $"\t\t{MinValuePrefix}: {this.Min}" );
                builder.AppendLine( $"\t\t{MaxValuePrefix}: {this.Max}" );
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

            if( this.Min > this.Max )
            {
                builder.AppendLine( nameof( this.Min ) + " can not be greater than the " + nameof( this.Max ) );
            }

            return builder.ToString();
        }
    }
}
