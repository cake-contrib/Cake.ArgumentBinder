//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using Cake.Core.IO;
using NUnit.Framework;

namespace Cake.ArgumentBinder.Tests.UnitTests
{
    [TestFixture]
    public sealed class FilePathArgumentAttributeShowDescriptionTests
    {
        // ---------------- Fields ----------------

        private const string taskDescription = "Task Description";
        private const string argumentName = "file_argument";
        private const string argDescription = "Some Description";
        private const string defaultValue = "file.txt";
        private const string nullDefaultValue = null;

        // ---------------- Tests ----------------

        [Test]
        public void FilePathArgumentNotHiddenNotRequiredTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<FilePathArgumentNotHiddenNotRequiredNotExisting>( taskDescription );

            // Check

            // -------- Lines that should be there --------

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"--{argumentName}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                argDescription,
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.SourcePrefix}: {BaseAttribute.DefaultArgumentSource}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.TypePrefix}: {typeof( FilePath ).Name}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.DefaultValuePrefix}: {defaultValue}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BasePathAttribute.MustExistPrefix}: {false}",
                actualDescription
            );

            // -------- Lines that should NOT there --------

            TestHelpers.EnsureLineDoesNotExistFromMultiLineString(
                BaseAttribute.RequiredPrefix,
                actualDescription
            );

            TestHelpers.EnsureLineDoesNotExistFromMultiLineString(
                BaseAttribute.ValueIsSecretPrefix,
                actualDescription
            );
        }

        [Test]
        public void FilePathArgumentNotHiddenButRequiredTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<FilePathArgumentNotHiddenButRequiredNotExisting>( taskDescription );

            // Check

            // -------- Lines that should be there --------

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"--{argumentName}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                argDescription,
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.SourcePrefix}: {BaseAttribute.DefaultArgumentSource}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.TypePrefix}: {typeof( FilePath ).Name}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.RequiredPrefix}: {true}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BasePathAttribute.MustExistPrefix}: {false}",
                actualDescription
            );


            // -------- Lines that should NOT there --------

            // Required argument, default value is not needed.
            // Should not print it.
            TestHelpers.EnsureLineDoesNotExistFromMultiLineString(
                BaseAttribute.DefaultValuePrefix,
                actualDescription
            );
        }

        [Test]
        public void FilePathArgumentHiddenNotRequiredTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<FilePathArgumentHiddenNotRequiredMustExist>( taskDescription );

            // Check

            // -------- Lines that should be there --------

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"--{argumentName}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                argDescription,
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.TypePrefix}: {typeof( FilePath ).Name}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.SourcePrefix}: {BaseAttribute.DefaultArgumentSource}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.DefaultValuePrefix}: {ArgumentBinder.HiddenString}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BasePathAttribute.MustExistPrefix}: {true}",
                actualDescription
            );

            // -------- Lines that should NOT there --------

            TestHelpers.EnsureLineDoesNotExistFromMultiLineString(
                BaseAttribute.RequiredPrefix,
                actualDescription
            );
        }

        [Test]
        public void FilePathArgumentHiddenAndRequiredTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<FilePathArgumentHiddenAndRequiredNotExisting>( taskDescription );

            // Check

            // -------- Lines that should be there --------

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"--{argumentName}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                argDescription,
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.SourcePrefix}: {BaseAttribute.DefaultArgumentSource}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.TypePrefix}: {typeof( FilePath ).Name}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.RequiredPrefix}: {true}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BasePathAttribute.MustExistPrefix}: {false}",
                actualDescription
            );

            // -------- Lines that should NOT there --------

            // Required argument, default value is not needed.
            // Should not print it.
            TestHelpers.EnsureLineDoesNotExistFromMultiLineString(
                BaseAttribute.DefaultValuePrefix,
                actualDescription
            );
        }

        [Test]
        public void FilePathArgumentNullDefaultValueTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<FilePathArgumentNullDefaultValueNotExisting>( taskDescription );

            // Check

            // -------- Lines that should be there --------

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"--{argumentName}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                argDescription,
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.SourcePrefix}: {BaseAttribute.DefaultArgumentSource}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.TypePrefix}: {typeof( FilePath ).Name}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.DefaultValuePrefix}: [null]",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BasePathAttribute.MustExistPrefix}: {false}",
                actualDescription
            );

            // -------- Lines that should NOT there --------

            TestHelpers.EnsureLineDoesNotExistFromMultiLineString(
                BaseAttribute.RequiredPrefix,
                actualDescription
            );

            TestHelpers.EnsureLineDoesNotExistFromMultiLineString(
                BaseAttribute.ValueIsSecretPrefix,
                actualDescription
            );
        }

        // ---------------- Helper Classes ----------------

        private class FilePathArgumentNotHiddenNotRequiredNotExisting
        {
            [FilePathArgument(
                argumentName,
                Description = argDescription,
                DefaultValue = defaultValue,
                HasSecretValue = false,
                Required = false,
                MustExist = false
            )]
            public FilePath FilePathArgument { get; set; }
        }

        private class FilePathArgumentNotHiddenButRequiredNotExisting
        {
            [FilePathArgument(
                argumentName,
                Description = argDescription,
                DefaultValue = defaultValue,
                HasSecretValue = false,
                Required = true,
                MustExist = false
            )]
            public FilePath FilePathArgument { get; set; }
        }

        private class FilePathArgumentHiddenNotRequiredMustExist
        {
            [FilePathArgument(
                argumentName,
                Description = argDescription,
                DefaultValue = defaultValue,
                HasSecretValue = true,
                Required = false,
                MustExist = true
            )]
            public FilePath FilePathArgument { get; set; }
        }

        private class FilePathArgumentHiddenAndRequiredNotExisting
        {
            [FilePathArgument(
                argumentName,
                Description = argDescription,
                DefaultValue = defaultValue,
                HasSecretValue = true,
                Required = true,
                MustExist = false
            )]
            public FilePath FilePathArgument { get; set; }
        }

        private class FilePathArgumentNullDefaultValueNotExisting
        {
            [FilePathArgument(
                argumentName,
                Description = argDescription,
                DefaultValue = nullDefaultValue,
                HasSecretValue = false,
                Required = false,
                MustExist = false
            )]
            public FilePath FilePathArgument { get; set; }
        }
    }
}
