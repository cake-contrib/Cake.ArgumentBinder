//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Cake.ArgumentBinder.Tests.UnitTests
{
    [TestFixture]
    public sealed class EnumArgumentAttributeShowDescriptionTests
    {
        // ---------------- Fields ----------------

        private const string taskDescription = "Task Description";
        private const string argumentName = "enum_argument";
        private const string argDescription = "Some Description";

        // ---------------- Tests ----------------

        [Test]
        public void EnumArugmentNotHiddenNotRequiredDontIgnoreCaseTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<EnumArgumentNotHiddenNotRequiredDontIgnoreCase>( taskDescription );

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
                $"{BaseAttribute.TypePrefix}: {typeof( TestEnum ).Name}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.DefaultValuePrefix}: {default( TestEnum )}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseEnumAttribute.CasingIgnorePrefix}: {false}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                BaseEnumAttribute.PossibleValuePrefix,
                actualDescription
            );

            foreach( Enum value in Enum.GetValues( typeof( TestEnum ) ) )
            {
                TestHelpers.EnsureLineExistsFromMultiLineString(
                    $"- {value}",
                    actualDescription
                );
            }

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
        public void EnumArugmentNotHiddenRequiredIgnoreCaseTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<EnumArgumentNotHiddenButRequiredIgnoreCase>( taskDescription );

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
                $"{BaseAttribute.TypePrefix}: {typeof( TestEnum ).Name}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseEnumAttribute.CasingIgnorePrefix}: {true}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                BaseEnumAttribute.PossibleValuePrefix,
                actualDescription
            );

            foreach( Enum value in Enum.GetValues( typeof( TestEnum ) ) )
            {
                TestHelpers.EnsureLineExistsFromMultiLineString(
                    $"- {value}",
                    actualDescription
                );
            }

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.RequiredPrefix}: {true}",
                actualDescription
            );

            // -------- Lines that should NOT there --------

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
        public void EnumArugmentHiddenNotRequiredIgnoreCaseTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<EnumArgumentHiddenNotRequiredIgnoreCase>( taskDescription );

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
                $"{BaseAttribute.TypePrefix}: {typeof( TestEnum ).Name}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.ValueIsSecretPrefix}: {true}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.DefaultValuePrefix}: {ArgumentBinder.HiddenString}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseEnumAttribute.CasingIgnorePrefix}: {true}",
                actualDescription
            );

            // -------- Lines that should NOT there --------

            TestHelpers.EnsureLineDoesNotExistFromMultiLineString(
                BaseAttribute.RequiredPrefix,
                actualDescription
            );

            TestHelpers.EnsureLineDoesNotExistFromMultiLineString(
                BaseEnumAttribute.PossibleValuePrefix,
                actualDescription
            );

            foreach( Enum value in Enum.GetValues( typeof( TestEnum ) ) )
            {
                TestHelpers.EnsureLineDoesNotExistFromMultiLineString(
                    value.ToString(),
                    actualDescription
                );
            }
        }

        [Test]
        public void EnumArgumentHiddenAndRequiredDontIgnoreCaseTest()
        {
            // Act
            string actualDescription = ArgumentBinder.GetDescription<EnumArgumentHiddenAndRequiredDontIgnoreCase>( taskDescription );

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
                $"{BaseAttribute.TypePrefix}: {typeof( TestEnum ).Name}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.ValueIsSecretPrefix}: {true}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseEnumAttribute.CasingIgnorePrefix}: {false}",
                actualDescription
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"{BaseAttribute.RequiredPrefix}: {true}",
                actualDescription
            );

            // -------- Lines that should NOT there --------

            TestHelpers.EnsureLineDoesNotExistFromMultiLineString(
                BaseAttribute.DefaultValuePrefix,
                actualDescription
            );

            TestHelpers.EnsureLineDoesNotExistFromMultiLineString(
                BaseEnumAttribute.PossibleValuePrefix,
                actualDescription
            );

            foreach( Enum value in Enum.GetValues( typeof( TestEnum ) ) )
            {
                TestHelpers.EnsureLineDoesNotExistFromMultiLineString(
                    value.ToString(),
                    actualDescription
                );
            }
        }

        // ---------------- Helper Classes ----------------

        private class EnumArgumentNotHiddenNotRequiredDontIgnoreCase
        {
            [EnumArgument(
                typeof( TestEnum ),
                argumentName,
                Description = argDescription,
                HasSecretValue = false,
                Required = false,
                IgnoreCase = false
            )]
            public TestEnum EnumArgument { get; set; }
        }

        private class EnumArgumentNotHiddenButRequiredIgnoreCase
        {
            [EnumArgument(
                typeof( TestEnum ),
                argumentName,
                Description = argDescription,
                HasSecretValue = false,
                Required = true,
                IgnoreCase = true
            )]
            public TestEnum EnumArgument { get; set; }
        }

        private class EnumArgumentHiddenNotRequiredIgnoreCase
        {
            [EnumArgument(
                typeof( TestEnum ),
                argumentName,
                Description = argDescription,
                HasSecretValue = true,
                Required = false,
                IgnoreCase = true
            )]
            public TestEnum EnumArgument { get; set; }
        }

        private class EnumArgumentHiddenAndRequiredDontIgnoreCase
        {
            [EnumArgument(
                typeof( TestEnum ),
                argumentName,
                Description = argDescription,
                HasSecretValue = true,
                Required = true,
                IgnoreCase = false
            )]
            public TestEnum EnumArgument { get; set; }
        }

        // ---------------- Helper Enums ----------------

        private enum TestEnum
        {
            Value1,

            Value2,

            Value3
        }
    }
}
