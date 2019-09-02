//
// Copyright Seth Hendrick 2019.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

// For intellesense in VS Code
#tool "nuget:?package=Cake.Bakery"

// For binding arguments.
#addin nuget:?package=Cake.ArgumentBinder

#load "./DeleteHelpers.cake"

const string defaultTarget = "default";

string target = Argument( "target", defaultTarget );

Task( defaultTarget )
.Does(
    () =>
    {
        Information( "Hello, World!" );
    }
).Description( "Default target, prints 'Hello, World!'" );

RunTarget( target );
