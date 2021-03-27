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
        /// <summary>
        /// Creates a Cake Description string that can be put into
        /// a Task's Description function during a printout of "cake --showdescription".
        /// </summary>
        /// <typeparam name="T">The class to bind arguments to.</typeparam>
        /// <param name="taskDescription">Top-level description of the task.</param>
        /// <returns>A string of all of the argument's descriptions.</returns>
        public static string GetDescription<T>( string taskDescription )
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine( taskDescription );
            bool addedArgumentString = false;

            Type type = typeof( T );
            IEnumerable<PropertyInfo> properties = type.GetProperties();
            foreach( PropertyInfo info in properties )
            {
                Attribute argumentAttribute = info.GetCustomAttribute<Attribute>();

                if( argumentAttribute is IReadOnlyArgumentAttribute )
                {
                    if( addedArgumentString == false )
                    {
                        builder.AppendLine( "- Arguments:" );
                        addedArgumentString = true;
                    }
                    builder.AppendLine( argumentAttribute.ToString() );
                    continue;
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Helper function that creates a string representation of configured values
        /// on a class that we can bind arguments to.
        /// </summary>
        /// <typeparam name="T">The type of class to bind arguments to.</typeparam>
        /// <returns>A string that shows what the user passed into the class.</returns>
        public static string ConfigToStringHelper<T>( T obj )
        {
            Type type = typeof( T );

            StringBuilder builder = new StringBuilder();

            builder.AppendLine( type.Name + "'s Configuration:" );

            foreach( PropertyInfo property in type.GetProperties() )
            {
                Attribute attr = property.GetCustomAttribute<Attribute>();
                if( attr is IReadOnlyArgumentAttribute argumentAttribute )
                {
                    if( argumentAttribute.HasSecretValue )
                    {
                        builder.AppendLine( $"\t-{property.Name}: ******" );
                    }
                    else
                    {
                        builder.AppendLine( $"\t-{property.Name}: {property.GetValue( obj ).ToString()}" );
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Creates an instance of a class that can bind arguments to.
        /// </summary>
        /// <typeparam name="T">The type of class to bind arguments to.</typeparam>
        /// <param name="constructorArgs">Any constructor arguments for the config class.</param>
        public static T FromArguments<T>( ICakeContext cakeContext, params object[] constructorArgs )
        {
            Type type = typeof( T );
            T instance = (T)Activator.CreateInstance( type, constructorArgs );

            IEnumerable<PropertyInfo> properties = type.GetProperties();

            ArgumentBinderHelper<T> binderHelper = new ArgumentBinderHelper<T>(
                instance,
                properties,
                cakeContext
            );

            if( binderHelper.Success == false )
            {
                throw binderHelper.GetException();
            }

            return instance;
        }

        // ---------------- Helper Classes ----------------

        private class ArgumentBinderHelper<T>
        {
            // --------------- Fields ----------------

            private readonly List<Exception> exceptions;

            private readonly ICakeContext cakeContext;

            private readonly IEnumerable<PropertyInfo> properties;

            private readonly T instance;

            // --------------- Constructor ----------------

            public ArgumentBinderHelper( T instance, IEnumerable<PropertyInfo> properties, ICakeContext cakeContext )
            {
                this.exceptions = new List<Exception>();
                this.cakeContext = cakeContext;
                this.properties = properties;
                this.instance = instance;

                this.TryStringArguments();
                this.TryBooleanArguments();
                this.TryIntegerArguments();
            }

            // ---------------- Properties ----------------

            /// <summary>
            /// True if there were no errors during argument parsing,
            /// otherwise false.
            /// </summary>
            public bool Success
            {
                get
                {
                    return this.exceptions.Count == 0;
                }
            }

            // ---------------- Functions ----------------

            public override string ToString()
            {
                return this.GetException().ToString();
            }

            public AggregateException GetException()
            {
                return new AggregateException( "Errors when parsing arguments", this.exceptions );
            }

            private void TryStringArguments()
            {
                foreach( PropertyInfo info in this.properties )
                {
                    StringArgumentAttribute argumentAttribute = info.GetCustomAttribute<StringArgumentAttribute>();
                    if( argumentAttribute != null )
                    {
                        string argumentErrors = argumentAttribute.TryValidate();
                        if( string.IsNullOrWhiteSpace( argumentErrors ) == false )
                        {
                            this.exceptions.Add(
                                new AttributeValidationException( info, argumentErrors )
                            );
                            continue;
                        }

                        string cakeArg;
                        if( cakeContext.Arguments.HasArgument( argumentAttribute.ArgName ) )
                        {
                            cakeArg = cakeContext.Arguments.GetArgument( argumentAttribute.ArgName );
                        }
                        else if( argumentAttribute.Required )
                        {
                            this.exceptions.Add(
                                new MissingRequiredArgumentException( argumentAttribute.ArgName )
                            );
                            continue;
                        }
                        else
                        {
                            cakeArg = argumentAttribute.DefaultValue;
                        }

                        info.SetValue(
                            instance,
                            cakeArg
                        );
                    }
                }
            }

            private void TryBooleanArguments()
            {
                foreach( PropertyInfo info in this.properties )
                {
                    BooleanArgumentAttribute argumentAttribute = info.GetCustomAttribute<BooleanArgumentAttribute>();
                    if( argumentAttribute != null )
                    {
                        string argumentErrors = argumentAttribute.TryValidate();
                        if( string.IsNullOrWhiteSpace( argumentErrors ) == false )
                        {
                            this.exceptions.Add(
                                new AttributeValidationException( info, argumentErrors )
                            );
                            continue;
                        }

                        bool? value = null;
                        string cakeArg;
                        if( cakeContext.Arguments.HasArgument( argumentAttribute.ArgName ) )
                        {
                            cakeArg = cakeContext.Arguments.GetArgument( argumentAttribute.ArgName );
                            if( bool.TryParse( cakeArg, out bool result ) )
                            {
                                value = result;
                            }
                            else
                            {
                                this.exceptions.Add(
                                    new ArgumentFormatException( typeof( bool ), argumentAttribute.ArgName )
                                );
                                continue;
                            }
                        }

                        if( argumentAttribute.Required && ( value == null ) )
                        {
                            this.exceptions.Add(
                                new MissingRequiredArgumentException( argumentAttribute.ArgName )
                            );
                            continue;
                        }

                        if( value == null )
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

            private void TryIntegerArguments()
            {
                foreach( PropertyInfo info in this.properties )
                {
                    IntegerArgumentAttribute argumentAttribute = info.GetCustomAttribute<IntegerArgumentAttribute>();
                    if( argumentAttribute != null )
                    {
                        string argumentErrors = argumentAttribute.TryValidate();
                        if( string.IsNullOrWhiteSpace( argumentErrors ) == false )
                        {
                            this.exceptions.Add(
                                new AttributeValidationException( info, argumentErrors )
                            );
                            continue;
                        }

                        int? value = null;
                        string cakeArg;
                        if( cakeContext.Arguments.HasArgument( argumentAttribute.ArgName ) )
                        {
                            cakeArg = cakeContext.Arguments.GetArgument( argumentAttribute.ArgName );
                            if( int.TryParse( cakeArg, out int result ) )
                            {
                                if( result > argumentAttribute.Max )
                                {
                                    this.exceptions.Add(
                                        new ArgumentTooLargeException( argumentAttribute.Max.ToString(), argumentAttribute.ArgName )
                                    );
                                    continue;
                                }
                                if( result < argumentAttribute.Min )
                                {
                                    this.exceptions.Add(
                                        new ArgumentTooSmallException( argumentAttribute.Min.ToString(), argumentAttribute.ArgName )
                                    );
                                    continue;
                                }

                                value = result;
                            }
                            else
                            {
                                this.exceptions.Add(
                                    new ArgumentFormatException( typeof( int ), argumentAttribute.ArgName )
                                );
                                continue;
                            }
                        }

                        if( argumentAttribute.Required && ( value == null ) )
                        {
                            this.exceptions.Add(
                                new MissingRequiredArgumentException( argumentAttribute.ArgName )
                            );
                            continue;
                        }

                        if( value == null )
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
        }
    }
}
