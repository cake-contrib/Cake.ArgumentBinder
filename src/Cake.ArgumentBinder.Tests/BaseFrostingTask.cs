//
// Copyright Seth Hendrick 2019-2022.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.Runtime.ExceptionServices;
using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Frosting;

namespace Cake.ArgumentBinder.Tests
{
    public abstract class BaseFrostingTask : FrostingTask
    {
        public override void OnError( Exception exception, ICakeContext context )
        {
            // We want the stack trace to print out when all is said and done.
            // The way to do this is to set the verbosity to the maximum,
            // and then re-throw the exception.  Use the weird DispatchInfo
            // class so we don't get a new stack trace.
            context.DiagnosticVerbosity();
            ExceptionDispatchInfo.Capture( exception ).Throw();
        }
    }
}
