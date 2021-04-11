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
    public sealed class DirectoryPathArgumentAttributeShowDescriptionTests
    {
        // ---------------- Fields ----------------

        private const string taskDescription = "Task Description";
        private const string argumentName = "directory_argument";
        private const string argDescription = "Some Description";
        private const string defaultValue = "directory";
        private const string nullDefaultValue = null;

        // ---------------- Tests ----------------

        [Test]
        public void DirectoryPathArgumentNotHiddenNotRequiredTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<DirectoryPathArgumentNotHiddenNotRequiredNotExisting>( taskDescription );

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
                $"{BaseAttribute.TypePrefix}: {typeof( DirectoryPath ).Name}",
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
        public void DirectoryPathArgumentNotHiddenButRequiredTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<DirectoryPathArgumentNotHiddenButRequiredNotExisting>( taskDescription );

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
                $"{BaseAttribute.TypePrefix}: {typeof( DirectoryPath ).Name}",
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
        public void DirectoryPathArgumentHiddenNotRequiredTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<DirectoryPathArgumentHiddenNotRequiredMustExist>( taskDescription );

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
                $"{BaseAttribute.TypePrefix}: {typeof( DirectoryPath ).Name}",
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
        public void DirectoryPathArgumentHiddenAndRequiredTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<DirectoryPathArgumentHiddenAndRequiredNotExisting>( taskDescription );

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
                $"{BaseAttribute.TypePrefix}: {typeof( DirectoryPath ).Name}",
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
        public void DirectoryPathArgumentNullDefaultValueTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<DirectoryPathArgumentNullDefaultValueNotExisting>( taskDescription );

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
                $"{BaseAttribute.TypePrefix}: {typeof( DirectoryPath ).Name}",
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

        private class DirectoryPathArgumentNotHiddenNotRequiredNotExisting
        {
            [DirectoryPathArgument(
                argumentName,
                Description = argDescription,
                DefaultValue = defaultValue,
                HasSecretValue = false,
                Required = false,
                MustExist = false
            )]
            public DirectoryPath DirectoryPathArgument { get; set; }
        }

        private class DirectoryPathArgumentNotHiddenButRequiredNotExisting
        {
            [DirectoryPathArgument(
                argumentName,
                Description = argDescription,
                DefaultValue = defaultValue,
                HasSecretValue = false,
                Required = true,
                MustExist = false
            )]
            public DirectoryPath DirectoryPathArgument { get; set; }
        }

        private class DirectoryPathArgumentHiddenNotRequiredMustExist
        {
            [DirectoryPathArgument(
                argumentName,
                Description = argDescription,
                DefaultValue = defaultValue,
                HasSecretValue = true,
                Required = false,
                MustExist = true
            )]
            public DirectoryPath DirectoryPathArgument { get; set; }
        }

        private class DirectoryPathArgumentHiddenAndRequiredNotExisting
        {
            [DirectoryPathArgument(
                argumentName,
                Description = argDescription,
                DefaultValue = defaultValue,
                HasSecretValue = true,
                Required = true,
                MustExist = false
            )]
            public DirectoryPath DirectoryPathArgument { get; set; }
        }

        private class DirectoryPathArgumentNullDefaultValueNotExisting
        {
            [DirectoryPathArgument(
                argumentName,
                Description = argDescription,
                DefaultValue = nullDefaultValue,
                HasSecretValue = false,
                Required = false,
                MustExist = false
            )]
            public DirectoryPath DirectoryPathArgument { get; set; }
        }
    }
}
