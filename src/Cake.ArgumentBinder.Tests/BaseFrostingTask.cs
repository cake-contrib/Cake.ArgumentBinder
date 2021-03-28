//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Frosting;

namespace Cake.ArgumentBinder.Tests
{
    public abstract class BaseFrostingTask : FrostingTask
    {
        public sealed override void OnError( Exception exception, ICakeContext context )
        {
            base.OnError( exception, context );
            context.Error( exception.ToString() );
        }
    }
}
