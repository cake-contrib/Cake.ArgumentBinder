//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying Directory LICENSE in the root of the repository).
//

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Cake.ArgumentBinder.Tests.UnitTests
{
    [TestFixture]
    public sealed class BooleanArgumentToStringTest
    {
        private BooleanTest uut;

        // ---------------- Setup / Teardown ----------------

        [SetUp]
        public void TestSetup()
        {
            this.uut = new BooleanTest
            {
                HiddenValue = true,
                NotHiddenValue = true
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
        }

        // ---------------- Test Helpers ----------------

        private sealed class BooleanTest
        {
            // ---------------- Fields ----------------

            internal const string NotHiddenArgName = "bool_not_hidden";
            internal const string HiddenArgName = "bool_hidden";

            // ---------------- Properties ----------------

            [BooleanArgument(
                NotHiddenArgName,
                HasSecretValue = false,
                Required = true
            )]
            public bool NotHiddenValue { get; set; }

            [BooleanArgument(
                HiddenArgName,
                HasSecretValue = true,
                Required = true
            )]
            public bool HiddenValue { get; set; }

            // ---------------- Functions ----------------

            public override string ToString()
            {
                return ArgumentBinder.ConfigToStringHelper( this );
            }
        }
    }
}
