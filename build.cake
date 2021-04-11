// ---------------- Addins ----------------

#addin "nuget:?package=Cake.LicenseHeaderUpdater&version=0.2.0"

// ---------------- Tools ----------------

#tool "nuget:?package=NUnit.ConsoleRunner&version=3.12.0"
#tool "nuget:?package=OpenCover&version=4.7.922"
#tool "nuget:?package=ReportGenerator&version=4.8.7"

// ---------------- Usings ----------------

using System.Text.RegularExpressions;

// ---------------- Constants ----------------

const string buildTarget = "build";
const string unitTestTarget = "unit_test";
const string buildReleaseTarget = "build_release";
const string nugetPackTarget = "nuget_pack";
const string makeDistTarget = "make_dist";

string target = Argument( "target", buildTarget );
bool runCoverage = Argument<bool>( "code_coverage", false );

FilePath sln = new FilePath( "./src/Cake.ArgumentBinder.sln" );
DirectoryPath distFolder = MakeAbsolute( new DirectoryPath( "./dist" ) );
DirectoryPath nuspecFolder = MakeAbsolute( new DirectoryPath( "./nuspec" ) );
DirectoryPath coverageFolder = MakeAbsolute( new DirectoryPath( "./CodeCoverage" ) );
DirectoryPath testResultFolder = MakeAbsolute( new DirectoryPath( "./TestResults" ) );

// This is the version of this software,
// update before making a new release.
const string version = "0.5.0";

DotNetCoreMSBuildSettings msBuildSettings = new DotNetCoreMSBuildSettings();

// Sets filesing's assembly version.
msBuildSettings.WithProperty( "Version", version )
    .WithProperty( "AssemblyVersion", version )
    .SetMaxCpuCount( System.Environment.ProcessorCount )
    .WithProperty( "FileVersion", version );

// ---------------- Tasks ----------------

Task( buildTarget )
.Does(
    () =>
    {
        msBuildSettings.SetConfiguration( "Debug" );
        DotNetCoreBuildSettings settings = new DotNetCoreBuildSettings
        {
            MSBuildSettings = msBuildSettings
        };
        DotNetCoreBuild( sln.ToString(), settings );
    }
).Description( "Builds the Debug target of Cake.ArgumentBinder" );

Task( unitTestTarget )
.Does(
    ( context ) =>
    {
        if( runCoverage )
        {
            EnsureDirectoryExists( coverageFolder );
            CleanDirectory( coverageFolder );

            OpenCoverSettings settings = new OpenCoverSettings
            {
                Register = "user",
                ReturnTargetCodeOffset = 0,
                OldStyle = true // This is needed or MissingMethodExceptions get thrown everywhere for some reason.
            };
            settings.WithFilter( "+[Cake.ArgumentBinder]*" );

            FilePath output = coverageFolder.CombineWithFilePath( "coverage.xml" );

            OpenCover( c => RunUnitTests( c ), output, settings );

            ReportGenerator( output, coverageFolder );
        }
        else
        {
            RunUnitTests( context );
        }
    }
).Description( "Runs all Unit Tests.  --code_coverage=true to run coverage" )
.IsDependentOn( buildTarget );

private void RunUnitTests( ICakeContext context )
{
    DotNetCoreTestSettings settings = new DotNetCoreTestSettings
    {
        NoBuild = true,
        NoRestore = true,
        Configuration = "Debug"
    };

    context.DotNetCoreTest( "./src/Cake.ArgumentBinder.Tests/Cake.ArgumentBinder.Tests.csproj", settings );
}

Task( buildReleaseTarget )
.Does(
    () =>
    {
        msBuildSettings.SetConfiguration( "Release" );
        DotNetCoreBuildSettings settings = new DotNetCoreBuildSettings
        {
            MSBuildSettings = msBuildSettings
        };
        DotNetCoreBuild( sln.ToString(), settings );
    }
).Description( "Builds with the Release Configuration." )
.IsDependentOn( unitTestTarget );

Task( makeDistTarget )
.Does(
    () =>
    {
        EnsureDirectoryExists( distFolder );
        CleanDirectory( distFolder );

        DotNetCorePublishSettings settings = new DotNetCorePublishSettings
        {
            OutputDirectory = distFolder,
            NoBuild = true,
            NoRestore = true,
            Configuration = "Release"
        };

        DotNetCorePublish( "./src/Cake.ArgumentBinder/Cake.ArgumentBinder.csproj", settings );
        CopyFile( "./LICENSE", System.IO.Path.Combine( distFolder.ToString(), "License.txt" ) );
        CopyFileToDirectory( "./Readme.md", distFolder );
    }
).Description( "Moves the files into directory so it can be distributed." )
.IsDependentOn( buildReleaseTarget );

Task( nugetPackTarget )
.Does(
    () =>
    {
        List<NuSpecContent> files = new List<NuSpecContent>();

        files.Add(
            new NuSpecContent
            { 
                Source = System.IO.Path.Combine( distFolder.ToString(), "Cake.ArgumentBinder.dll" ),
                Target = "lib/netstandard2.0" 
            }
        );

        files.Add(
            new NuSpecContent
            { 
                Source = System.IO.Path.Combine( distFolder.ToString(), "Cake.ArgumentBinder.pdb" ),
                Target = "lib/netstandard2.0" 
            }
        );

        files.Add(
            new NuSpecContent
            { 
                Source = System.IO.Path.Combine( distFolder.ToString(), "Cake.ArgumentBinder.xml" ),
                Target = "lib/netstandard2.0" 
            }
        );

        files.Add(
            new NuSpecContent
            { 
                Source = System.IO.Path.Combine( distFolder.ToString(), "License.txt" ),
                Target = "License.txt"
            }
        );

        files.Add(
            new NuSpecContent
            { 
                Source = System.IO.Path.Combine( distFolder.ToString(), "Readme.md" ),
                Target = "Readme.md"
            }
        );

        files.Add(
            new NuSpecContent
            {
                Source = System.IO.Path.Combine( nuspecFolder.ToString(), "icon.png" ),
                Target = string.Empty
            }
        );

        NuGetPackSettings settings = new NuGetPackSettings
        {
            Version = version,
            BasePath = distFolder,
            OutputDirectory = distFolder,
            Symbols = false,
            NoPackageAnalysis = false,
            Files = files
        };

        FilePath nuspec = nuspecFolder.CombineWithFilePath( "Cake.ArgumentBinder.nuspec" );

        NuGetPack( nuspec.ToString(), settings );
    }
).Description( "Builds the nuget package." )
.IsDependentOn( makeDistTarget );

Task( "update_license" )
.Does(
    () =>
    {
        const string currentLicense =
@"//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

";
        const string oldLicense = 
@"//
//\s*Copyright\s+Seth\s+Hendrick\s+\d+-?\d*\.?
//\s*Distributed\s+under\s+the\s+MIT\s+License\.?
//\s*\(See\s+accompanying\s+file\s+LICENSE\s+in\s+the\s+root\s+of\s+the\s+repository\)\.?
//[\n\r\s]*";

        CakeLicenseHeaderUpdaterSettings settings = new CakeLicenseHeaderUpdaterSettings
        {
            LicenseString = currentLicense,
            Threads = 0,
        };

        settings.OldHeaderRegexPatterns.Add( oldLicense );

        settings.FileFilter = delegate ( FilePath path )
        {
            if( Regex.IsMatch( path.ToString(), @"[/\\]obj[/\\]" ) )
            {
                return false;
            }
            if( Regex.IsMatch( path.ToString(), @"[/\\]bin[/\\]" ) )
            {
                return false;
            }
            else
            {
                return true;
            }
        };

        List<FilePath> files = new List<FilePath>();

        SolutionParserResult slnResult = ParseSolution( sln );
        foreach( SolutionProject proj in slnResult.Projects )
        {
            if( proj.Path.ToString().EndsWith( ".csproj" ) )
            {
                string glob = proj.Path.GetDirectory() + "/**/*.cs";
                files.AddRange( GetFiles( glob ) );
            }
        }

        files.AddRange( GetFiles( "Examples/*.cake" ) );
        UpdateLicenseHeaders( files, settings );
    }
);

Task( "appveyor" )
.Description( "Runs all of the tasks needed for AppVeyor" )
.IsDependentOn( nugetPackTarget );

RunTarget( target );