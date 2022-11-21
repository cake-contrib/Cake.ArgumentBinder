//
// Copyright Seth Hendrick 2019-2022.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.Collections.Generic;
using System.Reflection;
using Cake.Core;

namespace Cake.ArgumentBinder.Binders
{
    internal abstract class BaseBinder<TInstance, TAttribute>
        where TAttribute : BaseAttribute
    {
        // ---------------- Fields ----------------

        protected readonly ICakeContext cakeContext;

        private readonly List<Exception> exceptions;

        // ---------------- Constructor ----------------

        protected BaseBinder( ICakeContext cakeContext )
        {
            this.cakeContext = cakeContext;
            this.exceptions = new List<Exception>();
            this.Exceptions = this.exceptions.AsReadOnly();
        }

        // ---------------- Properties ----------------

        public IEnumerable<Exception> Exceptions { get; private set; }

        // ---------------- Functions ----------------

        public void Bind( TInstance instance, IEnumerable<PropertyInfo> properties )
        {
            foreach( PropertyInfo info in properties )
            {
                try
                {
                    TAttribute attribute = info.GetCustomAttribute<TAttribute>();
                    if( attribute == null )
                    {
                        continue;
                    }

                    if( attribute.BaseType.IsAssignableFrom( info.PropertyType ) == false )
                    {
                        throw new InvalidPropertyTypeForAttributeException( info, attribute );
                    }

                    string attributeErrors = attribute.TryValidate();
                    if( string.IsNullOrWhiteSpace( attributeErrors ) == false )
                    {
                        throw new AttributeValidationException( info, attributeErrors );
                    }

                    BindInternal( instance, info, attribute );
                }
                catch( Exception e )
                {
                    this.exceptions.Add( e );
                }
            }
        }

        protected bool HasArgument( string argumentName, TAttribute attribute )
        {
            bool hasCommandLine = this.cakeContext.Arguments.HasArgument( argumentName );

            if( attribute.ArgumentSource == ArgumentSource.CommandLine )
            {
                return hasCommandLine;
            }
            else
            {
                bool hasEnvVar = this.HasEnvironmentVariable( argumentName );

                if( attribute.ArgumentSource == ArgumentSource.EnvironmentVariable )
                {
                    return hasEnvVar;
                }
                else if(
                    ( attribute.ArgumentSource == ArgumentSource.CommandLineThenEnvironmentVariable ) ||
                    ( attribute.ArgumentSource == ArgumentSource.EnvironmentVariableThenCommandLine )
                )
                {
                    return hasCommandLine || hasEnvVar;
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Invalid {nameof( attribute.ArgumentSource )}: {attribute.ArgumentSource}."
                    );
                }
            }
        }

        protected string GetArgument( string argumentName, TAttribute attribute )
        {
            if( attribute.ArgumentSource == ArgumentSource.CommandLine )
            {
                return this.cakeContext.Arguments.GetArgument( argumentName );
            }
            else if( attribute.ArgumentSource == ArgumentSource.EnvironmentVariable )
            {
                return this.cakeContext.Environment.GetEnvironmentVariable( argumentName );
            }
            else if( attribute.ArgumentSource == ArgumentSource.CommandLineThenEnvironmentVariable )
            {
                if( this.cakeContext.Arguments.HasArgument( argumentName ) )
                {
                    return this.cakeContext.Arguments.GetArgument( argumentName );
                }
                else
                {
                    return this.cakeContext.Environment.GetEnvironmentVariable( argumentName );
                }
            }
            else if( attribute.ArgumentSource == ArgumentSource.EnvironmentVariableThenCommandLine )
            {
                if( this.HasEnvironmentVariable( argumentName ) )
                {
                    return this.cakeContext.Environment.GetEnvironmentVariable( argumentName );
                }
                else
                {
                    return this.cakeContext.Arguments.GetArgument( argumentName );
                }
            }
            else
            {
                throw new InvalidOperationException(
                    $"Invalid {nameof( attribute.ArgumentSource )}: {attribute.ArgumentSource}."
                );
            }
        }

        private bool HasEnvironmentVariable( string argumentName )
        {
            return string.IsNullOrEmpty( this.cakeContext.Environment.GetEnvironmentVariable( argumentName ) ) == false;
        }

        /// <param name="instance">The instance to bind to.</param>
        /// <param name="propertyInfo">The current property that is being checked.</param>
        /// <param name="attribute">The attribute that contains the argument info.</param>
        protected abstract void BindInternal( TInstance instance, PropertyInfo propertyInfo, TAttribute attribute );
    }
}
