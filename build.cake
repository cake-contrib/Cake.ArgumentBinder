#tool "nuget:?package=NUnit.ConsoleRunner"
#tool "nuget:?package=OpenCover"
#tool "nuget:?package=ReportGenerator"

#module nuget:?package=Cake.DotNetTool.Module
#tool "dotnet:?package=dotnet-format"

const string buildTarget = "build";
const string unitTestTarget = "unit_test";
const string buildReleaseTarget = "build_release";
const string nugetPackTarget = "nuget_pack";
const string makeDistTarget = "make_dist";

string target = Argument( "target", buildTarget );
bool runCoverage = Argument<bool>( "code_coverage", false );

FilePath sln = new FilePath( "./Cake.ArgumentBinder.sln" );
DirectoryPath distFolder = MakeAbsolute( new DirectoryPath( "./dist" ) );
DirectoryPath nuspecFolder = MakeAbsolute( new DirectoryPath( "./nuspec" ) );
DirectoryPath coverageFolder = MakeAbsolute( new DirectoryPath( "./CodeCoverage" ) );
DirectoryPath testResultFolder = MakeAbsolute( new DirectoryPath( "./TestResults" ) );

// This is the version of this software,
// update before making a new release.
const string version = "0.2.1";

DotNetCoreMSBuildSettings msBuildSettings = new DotNetCoreMSBuildSettings();

// Sets filesing's assembly version.
msBuildSettings.WithProperty( "Version", version )
    .WithProperty( "AssemblyVersion", version )
    .SetMaxCpuCount( System.Environment.ProcessorCount )
    .WithProperty( "FileVersion", version );

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
).Description( "Runs all Unit Tests" )
.IsDependentOn( buildTarget );

private void RunUnitTests( ICakeContext context )
{
    DotNetCoreTestSettings settings = new DotNetCoreTestSettings
    {
        NoBuild = true,
        NoRestore = true,
        Configuration = "Debug"
    };

    context.DotNetCoreTest( "./Cake.ArgumentBinder.UnitTests/Cake.ArgumentBinder.UnitTests.csproj", settings );
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

        DotNetCorePublish( "./Cake.ArgumentBinder/Cake.ArgumentBinder.csproj", settings );
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

Task( "appveyor" )
.Description( "Runs all of the tasks needed for AppVeyor" )
.IsDependentOn( nugetPackTarget );

Task( "format" )
.Does(
    () =>
    {
        DotNetCoreToolSettings settings = new DotNetCoreToolSettings
        {
            ToolPath = File( "./tools/dotnet-format.exe" )
        };
        DotNetCoreTool( sln, "dotnet-format", string.Empty, settings );
    }
).Description( "Formats all of the code to standards" );

RunTarget( target );