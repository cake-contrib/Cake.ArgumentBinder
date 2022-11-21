//
// Copyright Seth Hendrick 2019-2022.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.Text;

namespace Cake.ArgumentBinder
{
    [AttributeUsage( AttributeTargets.Property, Inherited = true, AllowMultiple = false )]
    public abstract class BaseAttribute : Attribute, IReadOnlyArgumentAttribute
    {
        // ---------------- Fields ----------------

        private const ArgumentSource defaultArgumentSource = ArgumentSource.CommandLine;

        internal static readonly string RequiredPrefix = "Required";

        internal static readonly string DefaultValuePrefix = "Default Value";

        internal static readonly string ValueIsSecretPrefix = "Value is Secret";

        internal static readonly string TypePrefix = "Type";

        internal static readonly string DefaultArgumentDescription = "(No Description Given)";

        internal static readonly string SourcePrefix = "Source";

        // ---------------- Constructor ----------------

        protected BaseAttribute( string argumentName, ArgumentSource argumentSource )
        {
            this.ArgName = argumentName;
            this.ArgumentSource = argumentSource;
            this.Description = string.Empty;
            this.Required = false;
            this.HasSecretValue = false;
        }

        // ---------------- Properties ----------------

        public static ArgumentSource DefaultArgumentSource => defaultArgumentSource;

        /// <summary>
        /// The name of the argument that is passed in via the command-line.
        /// </summary>
        public string ArgName { get; set; }

        /// <summary>
        /// Where to find the argument's value.
        /// </summary>
        public ArgumentSource ArgumentSource { get; set; }

        /// <summary>
        /// Description of what the argument does.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// If the value is not specified, this will fail validation when this is set to true.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Set to true if the value passed into the argument
        /// should be hidden from any print-outs to the console.
        /// </summary>
        public bool HasSecretValue { get; set; }

        /// <summary>
        /// The default value if an argument isn't specified.
        /// </summary>
        protected abstract object BaseDefaultValue { get; }

        /// <summary>
        /// The base C# type this will bind to.
        /// </summary>
        internal abstract Type BaseType { get; }

        // ---------------- Functions ----------------

        /// <summary>
        /// Appends this class's string representation to the passed
        /// in string builder object.
        /// 
        /// This should be the first thing called when creating a ToString()
        /// function of all child classes.
        /// </summary>
        protected void ToString( StringBuilder builder )
        {
            builder.AppendLine( "\t --" + this.ArgName );
            if( string.IsNullOrEmpty( this.Description ) )
            {
                builder.AppendLine( $"\t\t{DefaultArgumentDescription}." );
            }
            else
            {
                builder.AppendLine( $"\t\t{this.Description}." );
            }
            builder.AppendLine( $"\t\t{TypePrefix}: {this.BaseType.Name}." );
            builder.AppendLine( $"\t\t{SourcePrefix}: {this.ArgumentSource}." );
            if( this.Required )
            {
                builder.AppendLine( $"\t\t{RequiredPrefix}: {this.Required}." );
            }
            else
            {
                if( this.HasSecretValue )
                {
                    builder.AppendLine( $"\t\t{DefaultValuePrefix}: {ArgumentBinder.HiddenString}." );
                }
                else
                {
                    builder.AppendLine( $"\t\t{DefaultValuePrefix}: {this.BaseDefaultValue?.ToString() ?? "[null]"}." );
                }
            }

            if( this.HasSecretValue )
            {
                // Doesn't seem to be a need to print this out unless the value
                // is actually secret; it just adds more stuff to parse through.
                builder.AppendLine( $"\t\t{ValueIsSecretPrefix}: {this.HasSecretValue}." );
            }
        }

        /// <summary>
        /// Validates this object.  Returns <see cref="string.Empty"/>
        /// if nothing is wrong, otherwise this returns an error message.
        /// </summary>
        internal abstract string TryValidate();
    }
}
