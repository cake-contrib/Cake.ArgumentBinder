//
// Copyright Seth Hendrick 2019-2022.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using NUnit.Framework;

namespace Cake.ArgumentBinder.Tests.UnitTests
{
    [TestFixture]
    public sealed class StringArgumentToStringTest
    {
        private StringTest uut;

        // ---------------- Setup / Teardown ----------------

        [SetUp]
        public void TestSetup()
        {
            this.uut = new StringTest
            {
                HiddenValue = "Hello",
                NotHiddenValue = "World",
                NullValue = null
            };
        }

        [TearDown]
        public void TestTeardown()
        {
        }

        // ---------------- Tests ----------------

        [Test]
        public void ToStringTest()
        {
            string str = this.uut.ToString();

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"- {nameof( this.uut.NotHiddenValue )}: {this.uut.NotHiddenValue}",
                str
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"- {nameof( this.uut.HiddenValue )}: {ArgumentBinder.HiddenString}",
                str
            );

            TestHelpers.EnsureLineExistsFromMultiLineString(
                $"- {nameof( this.uut.NullValue )}: {ArgumentBinder.NullString}",
                str
            );
        }

        // ---------------- Test Helpers ----------------

        private sealed class StringTest
        {
            // ---------------- Fields ----------------

            internal const string NotHiddenArgName = "string_not_hidden";
            internal const string HiddenArgName = "string_hidden";
            internal const string NullArgName = "string_null";

            // ---------------- Properties ----------------

            [StringArgument(
                NotHiddenArgName,
                HasSecretValue = false,
                Required = true
            )]
            public string NotHiddenValue { get; set; }

            [StringArgument(
                HiddenArgName,
                HasSecretValue = true,
                Required = true
            )]
            public string HiddenValue { get; set; }

            [StringArgument(
                NullArgName,
                HasSecretValue = false,
                DefaultValue = null
            )]
            public string NullValue { get; set; }

            // ---------------- Functions ----------------

            public override string ToString()
            {
                return ArgumentBinder.ConfigToStringHelper( this );
            }
        }
    }
}
