//
// Copyright Seth Hendrick 2019.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.Text;
using Cake.Core;

namespace Cake.ArgumentBinder
{
    public class BaseStringAttribute : Attribute
    {
        // ---------------- Constructor ----------------

        protected BaseStringAttribute( string arg )
        {
            if ( string.IsNullOrWhiteSpace( arg ) )
            {
                throw new ArgumentNullException( nameof( arg ) );
            }
            this.ArgName = arg;
            this.DefaultValue = string.Empty;
            this.Description = string.Empty;
            this.Required = false;
        }

        // ---------------- Properties ----------------

        /// <summary>
        /// The default value of the argument; defaulted to <see cref="string.Empty"/>.
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// The argument name 
        /// </summary>
        public string ArgName { get; private set; }

        /// <summary>
        /// Description of what the argument does.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// If the value is an empty string, this will fail validation.
        /// </summary>
        public bool Required { get; set; }

        // ---------------- Functions ----------------

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine( "\t --" + this.ArgName );
            builder.AppendLine( "\t\t" + this.Description );
            builder.AppendLine( "\t\tType: String" );
            if ( this.Required )
            {
                builder.AppendLine( "\t\tThis argument is Required." );
            }
            else
            {
                if ( string.IsNullOrWhiteSpace( this.DefaultValue ) == false )
                {
                    builder.AppendLine( "\t\tDefaulted to: " + this.DefaultValue );
                }
                else
                {
                    builder.AppendLine( "\t\tDefaulted to: (Empty String)" );
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
            if ( string.IsNullOrWhiteSpace( this.ArgName ) )
            {
                builder.AppendLine( nameof( this.ArgName ) + " can not be null, empty, or whitespace." );
            }
            if ( string.IsNullOrWhiteSpace( this.Description ) )
            {
                builder.AppendLine( nameof( this.Description ) + " can not be null, empty, or whitespace." );
            }

            return builder.ToString();
        }
    }

    public class BaseBooleanAttribute : Attribute
    {
        // ---------------- Constructor ----------------

        protected BaseBooleanAttribute( string arg )
        {
            if ( string.IsNullOrWhiteSpace( arg ) )
            {
                throw new ArgumentNullException( nameof( arg ) );
            }
            this.ArgName = arg;
            this.DefaultValue = false;
            this.Description = string.Empty;
            this.Required = false;
        }

        // ---------------- Properties ----------------

        /// <summary>
        /// The default value of the argument; defaulted to false.
        /// </summary>
        public bool DefaultValue { get; set; }

        /// <summary>
        /// The argument name 
        /// </summary>
        public string ArgName { get; private set; }

        /// <summary>
        /// Description of what the argument does.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// If the value is not specified, this will fail validation.
        /// </summary>
        public bool Required { get; set; }

        // ---------------- Functions ----------------

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine( "\t --" + this.ArgName );
            builder.AppendLine( "\t\t" + this.Description );
            builder.AppendLine( "\t\tType: Boolean" );
            if ( this.Required )
            {
                builder.AppendLine( "\t\tThis argument is Required." );
            }
            else
            {
                builder.AppendLine( "\t\tDefault Value: " + this.DefaultValue );
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
            if ( string.IsNullOrWhiteSpace( this.ArgName ) )
            {
                builder.AppendLine( nameof( this.ArgName ) + " can not be null, empty, or whitespace." );
            }
            if ( string.IsNullOrWhiteSpace( this.Description ) )
            {
                builder.AppendLine( nameof( this.Description ) + " can not be null, empty, or whitespace." );
            }

            return builder.ToString();
        }
    }

    public class BaseIntegerAttribute : Attribute
    {
        // ---------------- Constructor ----------------

        protected BaseIntegerAttribute( string arg )
        {
            if ( string.IsNullOrWhiteSpace( arg ) )
            {
                throw new ArgumentNullException( nameof( arg ) );
            }
            this.ArgName = arg;
            this.DefaultValue = 0;
            this.Description = string.Empty;
            this.Required = false;
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
        /// The argument name 
        /// </summary>
        public string ArgName { get; private set; }

        /// <summary>
        /// Description of what the argument does.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// If the value is not specified, this will fail validation.
        /// </summary>
        public bool Required { get; set; }

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

        // ---------------- Functions ----------------

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine( "\t --" + this.ArgName );
            builder.AppendLine( "\t\t" + this.Description );
            builder.AppendLine( "\t\tType: Integer" );
            if ( this.Required )
            {
                builder.AppendLine( "\t\tThis argument is Required." );
            }
            else
            {
                builder.AppendLine( "\t\tDefault Value: " + this.DefaultValue );
            }
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

            if ( string.IsNullOrWhiteSpace( this.Description ) )
            {
                builder.AppendLine( nameof( this.Description ) + " can not be null, empty, or whitespace." );
            }

            if ( this.Min > this.Max )
            {
                builder.AppendLine( nameof( this.Min ) + " can not be greater than the " + nameof( this.Max ) );
            }

            return builder.ToString();
        }
    }
}
