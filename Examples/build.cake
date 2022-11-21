//
// Copyright Seth Hendrick 2019-2022.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

#load "./DeleteHelpers.cake"
#load "./Figlet.cake"

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
