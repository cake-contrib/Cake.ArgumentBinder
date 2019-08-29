//
// Copyright Seth Hendrick 2019.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Cake.Core;

namespace Cake.ArgumentBinder
{
    public static class ArgumentHelpers
    {
        public static string GetDescription<T>( string taskDescription )
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine( taskDescription );
            bool addedArgumentString = false;

            Type type = typeof( T );
            IEnumerable<PropertyInfo> properties = type.GetProperties();
            foreach ( PropertyInfo info in properties )
            {
                StringArgumentAttribute argumentAttribute = info.GetCustomAttribute<StringArgumentAttribute>();
                if ( argumentAttribute != null )
                {
                    if ( addedArgumentString == false )
                    {
                        builder.AppendLine( "- Arguments:" );
                        addedArgumentString = true;
                    }
                    builder.AppendLine( argumentAttribute.ToString() );
                }
            }

            return builder.ToString();
        }

        public static T FromArguments<T>( this ICakeContext cakeContext, params object[] constructorArgs )
        {
            Type type = typeof( T );
            T instance = (T)Activator.CreateInstance( type, constructorArgs );

            StringBuilder errors = new StringBuilder();

            IEnumerable<PropertyInfo> properties = type.GetProperties();
            foreach ( PropertyInfo info in properties )
            {
                StringArgumentAttribute argumentAttribute = info.GetCustomAttribute<StringArgumentAttribute>();
                if ( argumentAttribute != null )
                {
                    string argumentErrors = argumentAttribute.TryValidate();
                    if ( string.IsNullOrWhiteSpace( argumentErrors ) == false )
                    {
                        errors.AppendLine( argumentErrors );
                    }

                    string cakeArg;
                    if ( cakeContext.Arguments.HasArgument( argumentAttribute.ArgName ) )
                    {
                        cakeArg = cakeContext.Arguments.GetArgument( argumentAttribute.ArgName );
                    }
                    else
                    {
                        cakeArg = argumentAttribute.DefaultValue;
                    }

                    if ( argumentAttribute.Required && string.IsNullOrWhiteSpace( cakeArg ) )
                    {
                        errors.AppendLine( "Argument not specified, but is required: " + argumentAttribute.ArgName );
                    }

                    info.SetValue(
                        instance,
                        cakeArg
                    );
                }
            }

            if ( errors.Length != 0 )
            {
                throw new InvalidOperationException(
                    "Errors when validating arguments: " + Environment.NewLine + errors.ToString()
                );
            }

            return instance;
        }
    }
}
