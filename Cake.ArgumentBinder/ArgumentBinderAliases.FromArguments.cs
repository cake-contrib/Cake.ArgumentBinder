//
// Copyright Seth Hendrick 2019.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using Cake.Core;
using Cake.Core.Annotations;

namespace Cake.ArgumentBinder
{
    public static partial class ArgumentBinderAliases
    {
        /// <summary>
        /// Creates an instance of a class that can bind arguments to.
        /// </summary>
        /// <typeparam name="T">The type of class to bind arguments to.</typeparam>
        /// <param name="constructorArgs">Any constructor arguments for the config class, if any.</param>
        /// <returns>
        /// A new object of type T that has attributes binded to.
        /// </returns>
        /// <example>
        /// <para>
        /// Create a MyConfig object based on the passed in command line arguments.
        /// </para>
        /// <code>
        /// <![CDATA[
        ///     public class MyConfig
        ///     {
        ///         [BooleanArgument(
        ///             "dry_run",
        ///             Description = "Set to 'true' to not do anything.",
        ///             DefaultValue = false
        ///         )]
        ///         public bool DryRun { get; set; }
        ///     }
        ///     
        ///     MyConfig config = CreateFromArguments<MyConfig>();
        ///     
        ///     // If --dry_run=true is was passed into the command line, config.DryRun will be set to true,
        ///     // otherwise it will be set to false.
        /// ]]>
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory( "FromArguments" )]
        [CakeNamespaceImport( "Cake.ArgumentBinder" )]
        public static T CreateFromArguments<T>( this ICakeContext context, params object[] constructorArgs )
        {
            return ArgumentBinder.FromArguments<T>( context, constructorArgs );
        }
    }
}
