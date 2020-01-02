//
// Copyright Seth Hendrick 2019.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using Cake.Core;
using Cake.Core.Annotations;

namespace Cake.ArgumentBinder
{
    /// <summary>
    /// Extensions to the <see cref="CakeTaskBuilder"/> class that we use to bind arguments.
    /// </summary>
    public static class CakeTaskBuilderExtensions
    {
        /// <summary>
        /// Sets the Task's description with the output generated from <see cref="ArgumentBinder.GetDescription{T}(string)"/>.
        /// That is, a description of each of the arguments a task takes in.
        /// 
        /// Note, calling Description or this function again will overwrite what was there before.
        /// </summary>
        /// <typeparam name="T">The class that has arguments binded to that we wish to get the description of.</typeparam>
        /// <param name="taskDescription">Top-level description of the task.</param>
        /// <returns>
        /// A <see cref="CakeTaskBuilder"/> to continue being built up.
        /// </returns>
        /// <example>
        /// <para>
        /// If cake --showdescription is passed in, this will print information about all the arguments
        /// for a task.
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
        ///     Task( "my_task" )
        ///     .Does(
        ///         () =>
        ///         {
        ///             Information( "Hello" );
        ///         }
        ///     ).DescriptionFromArguments( "Does my thing" );
        ///     
        ///     // Output of cake --showdescription will be:
        ///     // 
        ///     // my_task              Does my thing
        ///     // - Arguments:
        ///     //      -- dry_run
        ///     //          Set to 'true' to not do anything.
        ///     //          Type: Boolean.
        ///     //          Default Value: 'False'.
        ///     //          Value is Secret: False.
        /// ]]>
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory( "Description" )]
        [CakeNamespaceImport( "Cake.ArgumentBinder" )]
        public static CakeTaskBuilder DescriptionFromArguments<T>( this CakeTaskBuilder builder, string description )
        {
            return builder.Description( ArgumentBinder.GetDescription<T>( description ) );
        }
    }
}
