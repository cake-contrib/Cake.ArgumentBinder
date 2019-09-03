//
// Copyright Seth Hendrick 2019.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

// This class helps delete files after a build,
// while allowing one to keep some files around if desired.

#load "./Includes.cake"

// ---------------- Classes ----------------

public class DeleteHelpersConfig
{
    // ---------------- Constructor ----------------

    public DeleteHelpersConfig()
    {
    }

    // ---------------- Properties ----------------

    /// <summary>
    /// The directory to delete things from.
    /// </summary>
    [StringArgument(
        "path",
        Description = "The path to delete from.",
        Required = true
    )]
    public string Directory { get; set; }

    /// <summary>
    /// The number of files or directories 
    /// to keep that match the given pattern.
    /// Defaulted to 0.
    /// Can not be negative.
    /// </summary>
    [IntegerArgument(
        "num_to_keep",
        Description = "The number of the most recent files/directories to keep that match the pattern.",
        DefaultValue = 0,
        Min = 0,
        Max = 255
    )]
    public int NumberOfFilesToKeep { get; set; }

    /// <summary>
    /// The glob of the deletion pattern to use.
    /// </summary>
    [StringArgument(
        "pattern",
        Description = "The glob pattern to delete files/directories from.",
        DefaultValue = "*"
    )]
    public string DeletionPattern { get; set; }

    [BooleanArgument(
        "dry_run",
        Description = "Set to 'true' to not delete any files, this will simply print what files will be deleted.",
        DefaultValue = false
    )]
    public bool DryRun { get; set; }

    public DirectoryPath FullDirectory
    {
        get
        {
            DirectoryPath baseDir = new DirectoryPath( this.Directory );
            DirectoryPath globPath = new DirectoryPath( this.DeletionPattern );

            return baseDir.Combine( globPath );
        }
    }

    // ---------------- Functions ----------------
}

public static class DeleteHelpers
{
    // ---------------- Functions ----------------

    public static void DeleteDirectories( ICakeContext cakeContext, DeleteHelpersConfig config )
    {
        DirectoryPathCollection dirs = cakeContext.GetDirectories( config.FullDirectory.ToString() );
        List<DirectoryPath> orderedDirs = dirs.OrderBy( f => System.IO.Directory.GetCreationTime( f.ToString() ) ).ToList();
        
        while( orderedDirs.Count > config.NumberOfFilesToKeep )
        {
            DirectoryPath dir = orderedDirs[0];
            cakeContext.Information( $"Deleting '{dir.ToString()}'" );

            if( config.DryRun == false )
            {
                DeleteDirectorySettings dirSettings = new DeleteDirectorySettings
                {
                    Force = true,
                    Recursive = true
                };
                
                cakeContext.DeleteDirectory( dir, dirSettings );
            }
            orderedDirs.RemoveAt( 0 );
        }
    }

    public static void DeleteFiles( ICakeContext cakeContext, DeleteHelpersConfig config )
    {
        FilePathCollection files = cakeContext.GetFiles( config.FullDirectory.ToString() );
        List<FilePath> orderedFiles = files.OrderBy( f => System.IO.File.GetCreationTime( f.ToString() ) ).ToList();
        
        while( orderedFiles.Count > config.NumberOfFilesToKeep )
        {
            FilePath file = orderedFiles[0];
            cakeContext.Information( $"Deleting '{file.ToString()}'" );
            if( config.DryRun == false )
            {
                cakeContext.DeleteFile( file );
            }
            orderedFiles.RemoveAt( 0 );
        }
    }
}

// ---------------- Tasks ----------------

Task( "delete_files" )
.Does(
    ( context ) =>
    {
        DeleteHelpersConfig config = ArgumentBinder.FromArguments<DeleteHelpersConfig>( context );
        context.Information( ArgumentBinder.ConfigToStringHelper( config ) );
        DeleteHelpers.DeleteFiles( context, config );
    }
)
.Description( ArgumentBinder.GetDescription<DeleteHelpersConfig>( "Deletes specified files." ) );

Task( "delete_dirs" )
.Does(
    ( context ) =>
    {
        DeleteHelpersConfig config = ArgumentBinder.FromArguments<DeleteHelpersConfig>( context );
        context.Information( ArgumentBinder.ConfigToStringHelper( config ) );
        DeleteHelpers.DeleteDirectories( context, config );
    }
)
.Description( ArgumentBinder.GetDescription<DeleteHelpersConfig>( "Deletes specified directories." ) );
