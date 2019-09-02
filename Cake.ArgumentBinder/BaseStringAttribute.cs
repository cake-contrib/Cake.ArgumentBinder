//
// Copyright Seth Hendrick 2019.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.Text;

namespace Cake.ArgumentBinder
{
    public class BaseStringAttribute : Attribute
    {
        // ---------------- Constructor ----------------

        protected BaseStringAttribute( string arg )
        {
            this.ArgName = arg;
            this.DefaultValue = string.Empty;
            this.Description = string.Empty;
            this.Required = false;
            this.HasSecretValue = false;
        }

        // ---------------- Properties ----------------

        public string DefaultValue { get; set; }

        public string ArgName { get; private set; }

        public string Description { get; set; }

        public bool Required { get; set; }

        public bool HasSecretValue { get; set; }

        // ---------------- Functions ----------------

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine( "\t --" + this.ArgName );
            if ( string.IsNullOrEmpty( this.Description ) )
            {
                builder.AppendLine( "\t\t(No Description Given)." );
            }
            else
            {
                builder.AppendLine( "\t\t" + this.Description );
            }
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
            builder.AppendLine( "\t\tValue is Secret: " + this.HasSecretValue );

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