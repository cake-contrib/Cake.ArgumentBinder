//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using NUnit.Framework;

namespace Cake.ArgumentBinder.Tests.UnitTests
{
    [TestFixture]
    public class IntegerArgumentAttributeShowDescriptionTests
    {
        // ---------------- Fields ----------------

        private const string taskDescription = "Task Description";
        private const string argumentName = "int_argument";
        private const string description = "Some Description";
        private const int defaultValue = 1;
        private const int minValue = -1;
        private const int maxValue = 3;

        // ---------------- Tests ----------------

        [Test]
        public void IntegerArgumentNotHiddenTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<IntegerArgumentNotHidden>( taskDescription );

            // Check

            // Should include the argument name.
            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"--{argumentName}",
                actualDescription
            );

            // Should include the description.
            TestHelpers.EnsureLineExistsFromMultiLineString(
                description,
                actualDescription
            );

            // Should include the type.
            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"Type: {typeof( int ).Name}",
                actualDescription
            );

            // Should include the min value.
            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"Minimum Value: {minValue}",
                actualDescription
            );

            // Should include the max value.
            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"Maximum Value: {maxValue}",
                actualDescription
            );
        }

        // ---------------- Helper Classes ----------------

        public class IntegerArgumentNotHidden
        {
            [IntegerArgument(
                argumentName,
                Description = description,
                DefaultValue = defaultValue,
                Min = minValue,
                Max = maxValue,
                HasSecretValue = false,
                Required = true
            )]
            public int IntegerArgument { get; set; }
        }
    }
}
