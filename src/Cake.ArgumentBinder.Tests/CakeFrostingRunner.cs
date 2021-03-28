//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using Cake.Core.IO;
using Cake.Frosting;
using NUnit.Framework;

namespace Cake.ArgumentBinder.Tests
{
    public static class CakeFrostingRunner
    {
        public static int TryRunCake( string[] args )
        {
            DirectoryPath testDir = new DirectoryPath( TestContext.CurrentContext.TestDirectory );
            DirectoryPath testRoot = testDir.Combine(
                new DirectoryPath(
                    "../" + // app
                    "../" + // Debug
                    ".."    // bin
                )
            );

            int exitCode = new CakeHost()
                .UseWorkingDirectory( testRoot )
                .AddAssembly( typeof( CakeFrostingRunner ).Assembly )
                .Run( args );

            return exitCode;
        }

        public static void RunCake( string[] args )
        {
            int exitCode = TryRunCake( args );
            Assert.Zero( exitCode );
        }
    }
}
