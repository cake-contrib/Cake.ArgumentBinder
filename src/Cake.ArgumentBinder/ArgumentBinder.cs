//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Cake.ArgumentBinder.Binders;
using Cake.ArgumentBinder.Binders.Argument;
using Cake.Core;

namespace Cake.ArgumentBinder
{
    public static class ArgumentBinder
    {
        // ---------------- Fields ----------------

        internal static readonly string HiddenString = "******";

        internal static readonly string NullString = "[null]";

        // ---------------- Functions ----------------

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
                        builder.AppendLine( $"\t- {property.Name}: {HiddenString}" );
                    }
                    else
                    {
                        builder.AppendLine( $"\t- {property.Name}: {property.GetValue( obj )?.ToString() ?? NullString}" );
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
                this.TryFilePathArguments();
                this.TryDirectoryPathArguments();
                this.TryEnumAttributes();
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
                ArgumentStringBinder<T> binder = new ArgumentStringBinder<T>( this.cakeContext );
                this.DoBind( binder );
            }

            private void TryBooleanArguments()
            {
                ArgumentBooleanBinder<T> binder = new ArgumentBooleanBinder<T>( this.cakeContext );
                this.DoBind( binder );
            }

            private void TryIntegerArguments()
            {
                ArgumentIntegerBinder<T> binder = new ArgumentIntegerBinder<T>( this.cakeContext );
                this.DoBind( binder );
            }

            private void TryFilePathArguments()
            {
                ArgumentFilePathBinder<T> binder = new ArgumentFilePathBinder<T>( this.cakeContext );
                this.DoBind( binder );
            }

            private void TryDirectoryPathArguments()
            {
                ArgumentDirectoryPathBinder<T> binder = new ArgumentDirectoryPathBinder<T>( this.cakeContext );
                this.DoBind( binder );
            }

            private void TryEnumAttributes()
            {
                ArgumentEnumBinder<T> binder = new ArgumentEnumBinder<T>( this.cakeContext );
                DoBind( binder );
            }

            private void DoBind<TAttribute>( BaseBinder<T, TAttribute> binder ) where TAttribute : BaseAttribute
            {
                binder.Bind( this.instance, this.properties );
                this.exceptions.AddRange( binder.Exceptions );
            }
        }
    }
}
