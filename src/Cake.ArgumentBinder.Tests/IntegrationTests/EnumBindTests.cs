//
// Copyright Seth Hendrick 2019-2022.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Frosting;
using NUnit.Framework;

namespace Cake.ArgumentBinder.Tests.IntegrationTests
{
    [TestFixture]
    public sealed class EnumBindTests
    {
        // ---------------- Fields ----------------

        private static EnumBind actualBind;
        private static Exception foundException;

        // ---------------- Setup / Teardown ----------------

        [SetUp]
        public void TestSetup()
        {
            actualBind = null;
            foundException = null;
        }

        [TearDown]
        public void TestTeardown()
        {
            actualBind = null;
            foundException = null;
        }

        // ---------------- Tests ----------------

        [Test]
        public void DoEnumBindTest()
        {
            const TestEnum requiredArgValue = TestEnum.Value2;
            const TestEnum optionalArgValue = TestEnum.Value3;

            // Setup
            string[] arguments = new string[]
            {
                $"--target={nameof( EnumBindTest )}",
                $"--{EnumBind.RequiredArgName}={requiredArgValue}",
                $"--{EnumBind.OptionalArgName}={optionalArgValue}"
            };

            // Act
            CakeFrostingRunner.RunCake( arguments );

            // Check
            Assert.NotNull( actualBind );
            Assert.AreEqual( requiredArgValue, actualBind.RequiredArg );
            Assert.AreEqual( optionalArgValue, actualBind.OptionalArg );
        }

        [Test]
        public void DoEnumBindWithMissingOptionalTest()
        {
            const TestEnum requiredArgValue = TestEnum.Value2;
            const TestEnum optionalArgValue = default;

            // Setup
            string[] arguments = new string[]
            {
                $"--target={nameof( EnumBindTest )}",
                $"--{EnumBind.RequiredArgName}={requiredArgValue.ToString().ToLower()}"
            };

            // Act
            CakeFrostingRunner.RunCake( arguments );

            // Check
            Assert.NotNull( actualBind );
            Assert.AreEqual( requiredArgValue, actualBind.RequiredArg );
            Assert.AreEqual( optionalArgValue, actualBind.OptionalArg );
        }

        [Test]
        public void DoEnumBindRequiredArgMissingTest()
        {
            // Setup
            string[] arguments = new string[]
            {
                $"--target={nameof( EnumBindTest )}",
            };

            // Act
            int exitCode = CakeFrostingRunner.TryRunCake( arguments );

            // Check
            Assert.NotZero( exitCode );
            Assert.NotNull( foundException );
            Assert.IsTrue( foundException is AggregateException );

            AggregateException ex = (AggregateException)foundException;
            Assert.AreEqual( 1, ex.InnerExceptions.Count );
            Assert.IsTrue( ex.InnerExceptions[0] is MissingRequiredArgumentException );
        }

        // ---------------- Helper Classes ----------------

        private sealed class EnumBind
        {
            // ---------------- Fields ----------------

            internal const string RequiredArgName = "enum_required";
            private const string requiredArgDescription = "A Required Enum";

            internal const string OptionalArgName = "enum_optional";
            private const string optionalArgDescription = "An optional Enum";

            // ---------------- Properties ----------------

            [EnumArgument(
                typeof( TestEnum ),
                RequiredArgName,
                Description = requiredArgDescription,
                HasSecretValue = false,
                Required = true,
                IgnoreCase = true
            )]
            public TestEnum RequiredArg { get; set; }

            [EnumArgument(
                typeof( TestEnum ),
                OptionalArgName,
                Description = optionalArgDescription,
                HasSecretValue = false,
                Required = false,
                IgnoreCase = false
            )]
            public TestEnum OptionalArg { get; set; }

            // ---------------- Functions ----------------

            public override string ToString()
            {
                return ArgumentBinder.ConfigToStringHelper( this );
            }
        }

        /// <remarks>
        /// Must be public so <see cref="Frosting"/> can find it.
        /// </remarks>
        [TaskName( nameof( EnumBindTest ) )]
        public sealed class EnumBindTest : BaseFrostingTask
        {
            public override void Run( ICakeContext context )
            {
                actualBind = context.CreateFromArguments<EnumBind>();
                context.Information( actualBind.ToString() );
            }

            public override void OnError( Exception exception, ICakeContext context )
            {
                EnumBindTests.foundException = exception;
                base.OnError( exception, context );
            }
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
