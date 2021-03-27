//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.Text;
using Cake.Core.IO;

namespace Cake.ArgumentBinder
{
    public abstract class BaseFilePathAttribute : BaseAttribute
    {
        // ---------------- Constructor ----------------

        protected BaseFilePathAttribute( string arg ) :
            base( arg )
        {
            this.DefaultValue = null;
            this.MustExist = false;
        }

        // ---------------- Properties ----------------

        /// <summary>
        /// If not specified, the default value is null.
        /// 
        /// If this is null, and <see cref="BaseAttribute.Required"/> is false,
        /// <see cref="MustExist"/> can not be true.
        /// </summary>
        /// <remarks>
        /// This is a string, and not a <see cref="FilePath"/>
        /// because <see cref="FilePath"/> can not be a property on an attribute.
        /// </remarks>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Must the file exist before executing the task?
        /// Defaulted to false.
        /// 
        /// If this is true, and <see cref="BaseAttribute.Required"/> is false,
        /// <see cref="DefaultValue"/> can not be null.
        /// </summary>
        public bool MustExist { get; set; }

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
                return typeof( FilePath );
            }
        }

        // ---------------- Functions ----------------

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            this.ToString( builder );
            builder.AppendLine( $"\t\tMust Exist: {this.MustExist}." );

            return builder.ToString();
        }

        /// <summary>
        /// Validates this object.  Returns <see cref="string.Empty"/>
        /// if nothing is wrong, otherwise this returns an error message.
        /// </summary>
        internal string TryValidate()
        {
            StringBuilder builder = new StringBuilder();

            // Null or empty only.  Files can technically be
            // nothing but whitespace.
            if( string.IsNullOrEmpty( this.ArgName ) )
            {
                builder.AppendLine( nameof( this.ArgName ) + " can not be null or empty." );
            }

            if(
                this.MustExist &&
                ( Required == false ) &&
                ( DefaultValue == null )
            )
            {
                builder.AppendLine(
                    $"If a {nameof( FilePath )} must exist, but the argument is not {nameof( Required )}, the {DefaultValue} can not be null."
                );
            }

            return builder.ToString();
        }
    }
}
