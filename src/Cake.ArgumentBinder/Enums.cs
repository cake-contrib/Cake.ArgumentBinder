//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

namespace Cake.ArgumentBinder
{
    /// <summary>
    /// Where the source of the argument's value is.
    /// </summary>
    public enum ArgumentSource
    {
        /// <summary>
        /// The argument's value is from the command line only.
        /// This is the default behavior.
        /// </summary>
        CommandLine = 0,

        /// <summary>
        /// The argument's value comes from an environment variable only.
        /// </summary>
        EnvironmentVariable,

        /// <summary>
        /// The argument's value comes from the command line.
        /// However, if the value does not exist on the command line,
        /// it will then come from an environment variable instead.
        /// </summary>
        CommandLineThenEnvironmentVariable,

        /// <summary>
        /// The argument's value comes from an environment variable.
        /// However, if the value does not exist as an environment variable,
        /// it will then come from the command line as a backup.
        /// </summary>
        EnvironmentVariableThenCommandLine,
    }
}
