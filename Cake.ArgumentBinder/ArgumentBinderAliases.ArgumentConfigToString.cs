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
        /// Takes in an object that has arguments binded to it,
        /// and creates a string-representation of the class.
        /// </summary>
        /// <remarks>
        /// This is an extension of <see cref="ICakeContext"/>, which means unless your config object
        /// has a reference to a <see cref="ICakeContext"/>, you can't call this function in a config object's
        /// ToString() function.  If you *want* to do that, call <see cref="ArgumentBinder.ConfigToStringHelper{T}(T)"/>
        /// directly (though you may need a "using Cake.ArgumentBinder" statement for that).
        /// 
        /// Also, obviously, if the given object has no arguments binded to it, this isn't going
        /// to be helpful.
        /// </remarks>
        /// <typeparam name="T">The type of class that has arguments bounded to it.</typeparam>
        /// <param name="obj">The type object to get the string of.</param>
        /// <returns>A string that shows what the user passed into the class.</returns>
        /// <example>
        /// <para>
        /// This prints the values for each property that has a <see cref="IReadOnlyArgumentAttribute"/> bound to it that
        /// the user passed in.
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
        ///     Information( ArgumentConfigToString( config ) );
        ///     
        ///     // If cake --dry_run is set to true, this is what will be printed out:
        ///     //
        ///     // MyConfig's Configuration:
        ///     //      -dry_run: True
        /// ]]>
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory( "ToString" )]
        [CakeNamespaceImport( "Cake.ArgumentBinder" )]
        public static string ArgumentConfigToString<T>( this ICakeContext context, T obj )
        {
            return ArgumentBinder.ConfigToStringHelper( obj );
        }
    }
}
