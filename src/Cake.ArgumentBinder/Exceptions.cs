//
// Copyright Seth Hendrick 2019-2022.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.Reflection;

namespace Cake.ArgumentBinder
{
    /// <summary>
    /// Exception that is thrown during the valiation of an attribute.
    /// NOT during argument processing.
    /// </summary>
    public class AttributeValidationException : Exception
    {
        // ---------------- Constructor ----------------

        public AttributeValidationException( PropertyInfo property, string message ) :
            base( $"Errors when validating attribute on '{property.DeclaringType.Name}.{property.Name}'{Environment.NewLine}{message}." )
        {
        }
    }

    public class InvalidPropertyTypeForAttributeException : Exception
    {
        // ---------------- Constructor ----------------

        public InvalidPropertyTypeForAttributeException(
            PropertyInfo property,
            BaseAttribute attribute
        ) :
            base( $"The property '{property.DeclaringType.Name}.{property.Name}' is set to type '{property.PropertyType.Name}', which is not compatible with attribute '{attribute.GetType().Name}', which expects a type of '{attribute.BaseType.Name}'." )
        {
        }
    }

    /// <summary>
    /// Exception that gets thrown if an argument is required,
    /// but was never specified.
    /// </summary>
    public class MissingRequiredArgumentException : Exception
    {
        // ---------------- Constructor ----------------

        public MissingRequiredArgumentException( string argumentName ) :
            base( $"Argument '{argumentName}' is required, but was never specified." )
        {
        }
    }

    /// <summary>
    /// Exception that gets thrown if an argument can not be converted
    /// to the argument's base type.
    /// </summary>
    public class ArgumentFormatException : Exception
    {
        // ----------------- Constructor ----------------

        public ArgumentFormatException( Type expectedType, string argumentName ) :
            base( $"Could not convert value specified in argument '{argumentName}' to type '{expectedType.Name}'." )
        {
        }
    }

    /// <summary>
    /// Exception that gets thrown if an argument's value is too big.
    /// </summary>
    public class ArgumentTooLargeException : Exception
    {
        // ---------------- Constructor ----------------

        public ArgumentTooLargeException( string maximumValue, string argumentName ) :
            base( $"Value specified in argument '{argumentName}' is greater than the maximum value of '{maximumValue}'." )
        {
        }
    }

    /// <summary>
    /// Exception that gets thrown if an argument's value is too small.
    /// </summary>
    public class ArgumentTooSmallException : Exception
    {
        // ---------------- Constructor ----------------

        public ArgumentTooSmallException( string minimumValue, string argumentName ) :
            base( $"Value specified in argument '{argumentName}' is less than the minimum value of '{minimumValue}'." )
        {
        }
    }

    /// <summary>
    /// Exception that gets thrown if an argument value is null,
    /// when it is not allowed to be.
    /// </summary>
    public class ArgumentValueNullException : Exception
    {
        // ---------------- Constructor ----------------

        public ArgumentValueNullException( string argumentName ) :
            base( $"Value specified in argument '{argumentName}' is null, but a value is needed." + Environment.NewLine +
                  $"This could mean an argument attribute is configured incorrectly."
            )
        {
        }
    }
}
