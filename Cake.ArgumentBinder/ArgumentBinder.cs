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
    public static class ArgumentBinder
    {
        public static string GetDescription<T>( string taskDescription )
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine( taskDescription );
            bool addedArgumentString = false;

            List<Type> argumentTypes = new List<Type>
            {
                typeof(StringArgumentAttribute),
                typeof(IntegerArgumentAttribute)
            };

            Type type = typeof( T );
            IEnumerable<PropertyInfo> properties = type.GetProperties();
            foreach ( PropertyInfo info in properties )
            {
                bool propertyDone = false;
                foreach ( Type argumentType in argumentTypes )
                {
                    Attribute argumentAttribute = info.GetCustomAttribute( argumentType );
                    if ( argumentAttribute != null )
                    {
                        if ( addedArgumentString == false )
                        {
                            builder.AppendLine( "- Arguments:" );
                            addedArgumentString = true;
                        }
                        builder.AppendLine( argumentAttribute.ToString() );
                        propertyDone = true;
                        break;
                    }
                }

                if ( propertyDone )
                {
                    continue;
                }
            }

            return builder.ToString();
        }

        public static T FromArguments<T>( this ICakeContext cakeContext, params object[] constructorArgs )
        {
            Type type = typeof( T );
            T instance = (T)Activator.CreateInstance( type, constructorArgs );

            IEnumerable<PropertyInfo> properties = type.GetProperties();

            ArgumentBinderHelper<T> binderHelper = new ArgumentBinderHelper<T>(
                instance,
                properties,
                cakeContext
            );

            if ( binderHelper.Success == false )
            {
                throw new InvalidOperationException(
                    "Errors when validating arguments: " + Environment.NewLine + binderHelper.Errors
                );
            }

            return instance;
        }

        // ---------------- Helper Classes ----------------

        private class ArgumentBinderHelper<T>
        {
            // --------------- Fields ----------------

            private readonly StringBuilder errorString;

            private readonly ICakeContext cakeContext;

            private readonly IEnumerable<PropertyInfo> properties;

            private readonly T instance;

            // --------------- Constructor ----------------

            public ArgumentBinderHelper( T instance, IEnumerable<PropertyInfo> properties, ICakeContext cakeContext )
            {
                this.errorString = new StringBuilder();
                this.cakeContext = cakeContext;
                this.properties = properties;
                this.instance = instance;

                this.Success = true;
                this.TryStringArguments();
                this.TryIntegerArguments();
            }

            // ---------------- Properties ----------------

            /// <summary>
            /// True if there were no errors during argument parsing,
            /// otherwise false.
            /// </summary>
            public bool Success { get; private set; }

            /// <summary>
            /// Error message, <see cref="string.Empty" /> if there are no errors.
            /// </summary>
            public string Errors
            {
                get
                {
                    if ( this.Success )
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return errorString.ToString();
                    }
                }
            }

            // ---------------- Functions ----------------

            public override string ToString()
            {
                return this.Errors;
            }

            private void TryStringArguments()
            {
                foreach ( PropertyInfo info in this.properties )
                {
                    StringArgumentAttribute argumentAttribute = info.GetCustomAttribute<StringArgumentAttribute>();
                    if ( argumentAttribute != null )
                    {
                        string argumentErrors = argumentAttribute.TryValidate();
                        if ( string.IsNullOrWhiteSpace( argumentErrors ) == false )
                        {
                            this.AddError( argumentErrors );
                            continue;
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
                            this.AddError( "Argument not specified, but is required: " + argumentAttribute.ArgName );
                            continue;
                        }

                        info.SetValue(
                            instance,
                            cakeArg
                        );
                    }
                }
            }

            private void TryIntegerArguments()
            {
                foreach ( PropertyInfo info in this.properties )
                {
                    IntegerArgumentAttribute argumentAttribute = info.GetCustomAttribute<IntegerArgumentAttribute>();
                    if ( argumentAttribute != null )
                    {
                        string argumentErrors = argumentAttribute.TryValidate();
                        if ( string.IsNullOrWhiteSpace( argumentErrors ) == false )
                        {
                            this.AddError( argumentErrors );
                            continue;
                        }

                        int? value = null;
                        string cakeArg;
                        if ( cakeContext.Arguments.HasArgument( argumentAttribute.ArgName ) )
                        {
                            cakeArg = cakeContext.Arguments.GetArgument( argumentAttribute.ArgName );
                            if ( int.TryParse( cakeArg, out int result ) )
                            {
                                if ( result > argumentAttribute.Max )
                                {
                                    this.AddError( $"Argument is greater than maximum of {argumentAttribute.Max}: {argumentAttribute.ArgName}" );
                                    continue;
                                }
                                if ( result < argumentAttribute.Min )
                                {
                                    this.AddError( $"Argument is less than minimum of {argumentAttribute.Min}: {argumentAttribute.ArgName}" );
                                    continue;
                                }

                                value = result;
                            }
                            else
                            {
                                this.AddError( "Argument is not an integer: " + argumentAttribute.ArgName );
                                continue;
                            }
                        }

                        if ( argumentAttribute.Required && ( value == null ) )
                        {
                            this.AddError( "Argument not specified, but is required: " + argumentAttribute.ArgName );
                            continue;
                        }

                        if ( value == null )
                        {
                            value = argumentAttribute.DefaultValue;
                        }

                        info.SetValue(
                            instance,
                            value.Value
                        );
                    }
                }
            }

            private void AddError( string errorMessage )
            {
                this.Success = false;
                this.errorString.AppendLine( errorMessage );
            }
        }
    }
}
