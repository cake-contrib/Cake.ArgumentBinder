//
// Copyright Seth Hendrick 2019-2022.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying Directory LICENSE in the root of the repository).
//

using NUnit.Framework;

namespace Cake.ArgumentBinder.Tests.UnitTests
{
    [TestFixture]
    public sealed class BooleanArgumentAttributeShowDescriptionTests
    {
        // ---------------- Fields ----------------

        private const string taskDescription = "Task Description";
        private const string argumentName = "bool_argument";
        private const string argDescription = "Some Description";
        private const bool defaultValue = true;

        // ---------------- Tests ----------------

        [Test]
        public void BooleanArgumentNotHiddenNotRequiredTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<BooleanArgumentNotHiddenNotRequired>( taskDescription );

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
                $"{BaseAttribute.TypePrefix}: {typeof( bool ).Name}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.DefaultValuePrefix}: {defaultValue}",
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
        public void BooleanArgumentNotHiddenButRequiredTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<BooleanArgumentNotHiddenButRequired>( taskDescription );

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
                $"{BaseAttribute.TypePrefix}: {typeof( bool ).Name}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.RequiredPrefix}: {true}",
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
        public void BooleanArgumentHiddenNotRequiredTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<BooleanArgumentHiddenNotRequired>( taskDescription );

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
                $"{BaseAttribute.TypePrefix}: {typeof( bool ).Name}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.DefaultValuePrefix}: {ArgumentBinder.HiddenString}",
                actualDescription
            );

            // -------- Lines that should NOT there --------

            TestHelpers.EnsureLineDoesNotExistFromMultiLineString(
                BaseAttribute.RequiredPrefix,
                actualDescription
            );
        }

        [Test]
        public void BooleanArgumentHiddenAndRequiredTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<BooleanArgumentHiddenAndRequired>( taskDescription );

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
                $"{BaseAttribute.TypePrefix}: {typeof( bool ).Name}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.RequiredPrefix}: {true}",
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

        // ---------------- Test Helpers ----------------

        private class BooleanArgumentNotHiddenNotRequired
        {
            [BooleanArgument(
                argumentName,
                Description = argDescription,
                DefaultValue = defaultValue,
                HasSecretValue = false,
                Required = false
            )]
            public bool BooleanArgument { get; set; }
        }

        private class BooleanArgumentNotHiddenButRequired
        {
            [BooleanArgument(
                argumentName,
                Description = argDescription,
                DefaultValue = defaultValue,
                HasSecretValue = false,
                Required = true
            )]
            public bool BooleanArgument { get; set; }
        }

        private class BooleanArgumentHiddenNotRequired
        {
            [BooleanArgument(
                argumentName,
                Description = argDescription,
                DefaultValue = defaultValue,
                HasSecretValue = true,
                Required = false
            )]
            public bool BooleanArgument { get; set; }
        }

        private class BooleanArgumentHiddenAndRequired
        {
            [BooleanArgument(
                argumentName,
                Description = argDescription,
                DefaultValue = defaultValue,
                HasSecretValue = true,
                Required = true
            )]
            public bool BooleanArgument { get; set; }
        }
    }
}
