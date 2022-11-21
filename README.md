Cake.ArgumentBinder
=========
A way to bind arguments passed into cake to configuration classes.

[![Build status](https://ci.appveyor.com/api/projects/status/p8qx0ee022gy9r9i?svg=true)](https://ci.appveyor.com/project/cakecontrib/cake-argumentbinder)

About
--------
This addin to [Cake](https://github.com/cake-build/cake), a C# build system, allows one to bind arguments passed into Cake
to C# classes without having to do it manually.

Honestly, for most users of Cake, this addin is probably overkill (even the cakefile for this very project doesn't use it).
If was designed for complex, flexible, build environments with multiple Tasks and multiple configurations passed in from the command-line.

Packages
--------
[![NuGet](https://img.shields.io/nuget/v/Cake.ArgumentBinder.svg)](https://www.nuget.org/packages/Cake.ArgumentBinder/) 

How it Works
--------
Let's say when invoking a target defined in Cake, the target requires several arguments from the user before it can execute.  In this example, it is deleting files from a directory.
One option is to just parse the arguments using the Arguments Function:

```C#
string path = Argument( "path", string.Empty );
string pattern = Argument( "pattern", string.Empty );
int filesToKeep = Argument<int>( "num_to_keep", 0 );
bool dryRun = Argument<bool>( "dry_run", false );
```

This is great and all, but now one should probably do validation to ensure the passed in arguments
are not an empty string, and the number of files to keep isn't negative.

Don't forget to add to the description what each argument does, which arguments are required, etc!  That gets REALLY old REALLY fast, especially in an environment with multiple tasks.
Wouldn't it be nice if we could take these arguments and bind them to a class we can then use without any issue?

With this addin, you now can, with the power of attributes.  Now instead of grabbing the arguments manually, just create a configuration class
and tag the corresponding properties with the proper Attributes.

```C#
public class DeleteHelpersConfig
{
    // ---------------- Properties ----------------

    /// <summary>
    /// The directory to delete things from.
    /// </summary>
    [StringArgument(
        "path",
        Description = "The path to delete from.",
        Required = true,
        ArgumentSource = ArgumentSource.CommandLineThenEnvironmentVariable
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
        Max = 255,
        ArgumentSource = ArgumentSource.CommandLineThenEnvironmentVariable
    )]
    public int NumberOfFilesToKeep { get; set; }

    /// <summary>
    /// The glob of the deletion pattern to use.
    /// </summary>
    [StringArgument(
        "pattern",
        Description = "The glob pattern to delete files/directories from.",
        DefaultValue = "*",
        ArgumentSource = ArgumentSource.CommandLineThenEnvironmentVariable
    )]
    public string DeletionPattern { get; set; }

    [BooleanArgument(
        "dry_run",
        Description = "Set to 'true' to not delete any files, this will simply print what files will be deleted.",
        DefaultValue = false,
        ArgumentSource = ArgumentSource.CommandLineThenEnvironmentVariable
    )]
    public bool DryRun { get; set; }
}
```

Now in your Task, instead of having to parse and validate several arguments, ArgumentBinder takes care of all of that for you.  You just need to do this in your task:

```C#
Task( "delete_files" )
.Does(
    ( context ) =>
    {
        DeleteHelpersConfig config = CreateFromArguments<DeleteHelpersConfig>();
        DeleteHelpers.DeleteFiles( context, config );
    }
)
.DescriptionFromArguments<DeleteHelpersConfig>( "Deletes specified files." );
```

Now, if you type ```cake --showdescription``` on the command line, you will see a print out of the task and
all the arguments:

```
Task                          Description
================================================================================
delete_files                  Deletes specified files.
- Arguments:
         --path
                The path to delete from.
                Type: String
                This argument is Required.

         --num_to_keep
                The number of the most recent files/directories to keep that match the pattern.
                Type: Integer
                Default Value: 0
                Minimum Value: 0
                Maximum Value: 255

         --pattern
                The glob pattern to delete files/directories from.
                Type: String
                Defaulted to: *

         --dry_run
                Set to 'true' to not delete any files, this will simply print what files will be deleted.
                Type: Boolean
                Default Value: False
```

If you do not specify the required arguments, you will get a helpful error message:

```
cake --target=delete_files

========================================
delete_files
========================================
An error occurred when executing task 'delete_files'.
Error: One or more errors occurred.
        Argument 'path' is required, but was never specified.
```

If you specify an argument that is not valid, you'll also get a helpful error message (in this example, going below the minimum for files to keep):

```
cake --target=delete_files --path="." --dry_run=true --num_to_keep=-1

========================================
delete_files
========================================
An error occurred when executing task 'delete_files'.
Error: One or more errors occurred.
        Value specified in argument 'num_to_keep' is less than the minimum value of '0'.
```

You can also specify the source of the argument by setting the "ArgumentSource" property.  There are four options:
* CommandLine (default) - The value comes from a command-line argument.
* EnvironmentVariable - The value comes from an environment variable.
* CommandLineThenEnvironmentVariable - The value first comes from the command-line, but falls back to an environment variable
                                       if a command-line argument of the given name is not specified.
* EnvironmentVariableThenCommandLine - The value first comes from an environment variable, but falls back to the command line
                                       if an environment variable of the given name is not specified.

Best Practices
--------
* Keep your "Configuration" classes separate from classes/functions that perform any action. 
  Not only does this keep a separation of concerns, but it is consistent with what Cake does.

TroubleShooting
--------
* Ensure that in the classes that you are binding arguments to, you _may_ need to include the using statement ```using Cake.ArgumentBinder;```,
  otherwise the compiler won't be able to find the attributes, and produce a lot of errors.

Discussion
--------
For questions and to discuss ideas & feature requests, use the [GitHub discussions on the Cake GitHub repository](https://github.com/cake-build/cake/discussions), under the [Extension Q&A](https://github.com/cake-build/cake/discussions/categories/extension-q-a) category.

[![Join in the discussion on the Cake repository](https://img.shields.io/badge/GitHub-Discussions-green?logo=github)](https://github.com/cake-build/cake/discussions)

Versioning
--------
Cake.ArgumentBinder's verioning isn't quite semantic verioning.  Rather, the major version (x in x.y.z) is the verion of cake the library was compiled against.  For example, if Cake.ArgumentBinder has a dependency on Cake version 2.0.0, the Cake.ArgumentBinder's version would be 2.y.z.  The minor version (y in x.y.z) will be incremented when a new feature gets added.  The patch (z in x.y.z) will be incremented when a bug fix happens.  We'll try to keep breaking backwards compatiblity to a minimum, and only when a new major version of Cake is released.

License
--------
To be consistent with Cake itself, Cake.ArgumentBinder is released under the MIT license.  This includes the examples.
