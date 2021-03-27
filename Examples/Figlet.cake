//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

// This file uses Cake.Figlet (https://github.com/cake-contrib/Cake.Figlet)
// to generate an ASCII banner.

#load "./Includes.cake"
#addin "nuget:?package=Cake.Figlet&version=2.0.1"

public class FigletConfig
{
    // ---------------- Properties ----------------

    [StringArgument(
        "text",
        Description = "The text to expand to an ASCII banner",
        Required = true
    )]
    public string Text{ get; set; }
}

Task( "banner" )
.Does(
    () =>
    {
        FigletConfig config = CreateFromArguments<FigletConfig>();
        Information( ArgumentConfigToString( config ) );
        Information( string.Empty );
        Information( Figlet( config.Text ) );
    }
).DescriptionFromArguments<FigletConfig>( "Prints a banner to Console.Out" );