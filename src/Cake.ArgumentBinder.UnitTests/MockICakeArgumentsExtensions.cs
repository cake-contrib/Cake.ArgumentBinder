//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System.Collections.Generic;
using Cake.Core;
using Moq;

namespace Cake.ArgumentBinder.UnitTests
{
    public static class MockICakeArgumentsExtensions
    {
        // ---------------- Functions ----------------

        public static void SetupGetArgumentSingle(
            this Mock<ICakeArguments> args,
            string argName,
            string returnValue
        )
        {
            args.Setup(
                a => a.GetArguments( argName )
            ).Returns( new List<string>{ returnValue } );
        }
    }
}
