//
// Copyright Seth Hendrick 2019-2022.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using NUnit.Framework;

namespace Cake.ArgumentBinder.Tests.UnitTests
{
    [TestFixture]
    public sealed class IntegerArgumentAttributeShowDescriptionTests
    {
        // ---------------- Fields ----------------

        private const string taskDescription = "Task Description";
        private const string argumentName = "int_argument";
        private const string argDescription = "Some Description";
        private const int defaultValue = 1;
        private const int minValue = -1;
        private const int maxValue = 3;

        // ---------------- Tests ----------------

        [Test]
        public void IntegerArgumentNotHiddenNotRequiredTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<IntegerArgumentNotHiddenNotRequired>( taskDescription );

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
                $"{BaseAttribute.TypePrefix}: {typeof( int ).Name}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.DefaultValuePrefix}: {defaultValue}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{IntegerArgumentAttribute.MinValuePrefix}: {minValue}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{IntegerArgumentAttribute.MaxValuePrefix}: {maxValue}",
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
        public void IntegerArgumentNotHiddenButRequiredTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<IntegerArgumentNotHiddenButRequired>( taskDescription );

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
                $"{BaseAttribute.TypePrefix}: {typeof( int ).Name}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{IntegerArgumentAttribute.MinValuePrefix}: {minValue}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{IntegerArgumentAttribute.MaxValuePrefix}: {maxValue}",
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

            TestHelpers.EnsureLineDoesNotExistFromMultiLineString(
                BaseAttribute.ValueIsSecretPrefix,
                actualDescription
            );
        }

        [Test]
        public void IntegerArgumentHiddenNotRequiredTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<IntegerArgumentHiddenNotRequired>( taskDescription );

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
                $"{BaseAttribute.TypePrefix}: {typeof( int ).Name}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.DefaultValuePrefix}: {ArgumentBinder.HiddenString}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{IntegerArgumentAttribute.MinValuePrefix}: {ArgumentBinder.HiddenString}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{IntegerArgumentAttribute.MaxValuePrefix}: {ArgumentBinder.HiddenString}",
                actualDescription
            );

            // -------- Lines that should NOT there --------

            TestHelpers.EnsureLineDoesNotExistFromMultiLineString(
                BaseAttribute.RequiredPrefix,
                actualDescription
            );
        }

        [Test]
        public void IntegerArgumentHiddenAndRequiredTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<IntegerArgumentHiddenAndRequired>( taskDescription );

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
                $"{BaseAttribute.TypePrefix}: {typeof( int ).Name}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{IntegerArgumentAttribute.MinValuePrefix}: {ArgumentBinder.HiddenString}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{IntegerArgumentAttribute.MaxValuePrefix}: {ArgumentBinder.HiddenString}",
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

        // ---------------- Helper Classes ----------------

        private class IntegerArgumentNotHiddenNotRequired
        {
            [IntegerArgument(
                argumentName,
                Description = argDescription,
                DefaultValue = defaultValue,
                Min = minValue,
                Max = maxValue,
                HasSecretValue = false,
                Required = false
            )]
            public int IntegerArgument { get; set; }
        }

        private class IntegerArgumentNotHiddenButRequired
        {
            [IntegerArgument(
                argumentName,
                Description = argDescription,
                DefaultValue = defaultValue,
                Min = minValue,
                Max = maxValue,
                HasSecretValue = false,
                Required = true
            )]
            public int IntegerArgument { get; set; }
        }

        private class IntegerArgumentHiddenNotRequired
        {
            [IntegerArgument(
                argumentName,
                Description = argDescription,
                DefaultValue = defaultValue,
                Min = minValue,
                Max = maxValue,
                HasSecretValue = true,
                Required = false
            )]
            public int IntegerArgument { get; set; }
        }

        private class IntegerArgumentHiddenAndRequired
        {
            [IntegerArgument(
                argumentName,
                Description = argDescription,
                DefaultValue = defaultValue,
                Min = minValue,
                Max = maxValue,
                HasSecretValue = true,
                Required = true
            )]
            public int IntegerArgument { get; set; }
        }
    }
}
