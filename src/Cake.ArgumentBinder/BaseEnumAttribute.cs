//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.Text;

namespace Cake.ArgumentBinder
{
    public abstract class BaseEnumAttribute : BaseAttribute
    {
        // ---------------- Fields ----------------

        private readonly Type enumType;

        // ---------------- Constructor ----------------

        protected BaseEnumAttribute( Type enumType, string arg ) :
            base( arg )
        {
            if( enumType.IsEnum == false )
            {
                throw new ArgumentException(
                    "Passed in type is not an Enum",
                    nameof( enumType )
                );
            }

            this.enumType = enumType;
            this.DefaultValue = (Enum)Activator.CreateInstance( enumType );
        }

        // ---------------- Properties ----------------

        /// <summary>
        /// The default value of the argument; defaulted to the default
        /// value of the enum (the first one defined).
        /// </summary>
        public Enum DefaultValue { get; set; }

        protected override object BaseDefaultValue => this.DefaultValue;

        protected override Type BaseType => this.enumType;
    }
}