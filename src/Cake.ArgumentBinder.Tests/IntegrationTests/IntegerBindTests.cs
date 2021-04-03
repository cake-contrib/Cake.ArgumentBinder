//
// Copyright Seth Hendrick 2019-2021.
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
    public sealed class IntegerBindTests
    {
        // ---------------- Fields ----------------

        private static IntegerBind actualBind;
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
        public void DoIntegerBindTest()
        {
            int requiredArgValue = IntegerBind.RequiredArgMaxValue;
            const int optionalArgValue = 2;

            // Setup
            string[] arguments = new string[]
            {
                $"--target={nameof( IntegerBindTest )}",
                $"--{IntegerBind.RequiredArgName}={requiredArgValue}",
                $"--{IntegerBind.OptionalArgName}={optionalArgValue}"
            };

            // Act
            CakeFrostingRunner.RunCake( arguments );

            // Check
            Assert.NotNull( actualBind );
            Assert.AreEqual( requiredArgValue, actualBind.RequiredArg );
            Assert.AreEqual( optionalArgValue, actualBind.OptionalArg );
        }

        [Test]
        public void DoIntegerBindNoOptionalValuesTest()
        {
            const int requiredArgValue = IntegerBind.RequiredArgMinValue;

            // Setup
            string[] arguments = new string[]
            {
                $"--target={nameof( IntegerBindTest )}",
                $"--{IntegerBind.RequiredArgName}={requiredArgValue}",
            };

            // Act
            CakeFrostingRunner.RunCake( arguments );

            // Check
            Assert.NotNull( actualBind );
            Assert.AreEqual( requiredArgValue, actualBind.RequiredArg );
            Assert.AreEqual( IntegerBind.DefaultValue, actualBind.OptionalArg );
        }

        [Test]
        public void DoIntegerBindRequiredArgMissingTest()
        {
            // Setup
            string[] arguments = new string[]
            {
                $"--target={nameof( IntegerBindTest )}",
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

        [Test]
        public void DoIntegerBindRequiredBelowMinTest()
        {
            // Setup
            string[] arguments = new string[]
            {
                $"--target={nameof( IntegerBindTest )}",
                $"--{IntegerBind.RequiredArgName}={IntegerBind.RequiredArgMinValue - 1}",
            };

            // Act
            int exitCode = CakeFrostingRunner.TryRunCake( arguments );

            // Check
            Assert.NotZero( exitCode );
            Assert.NotNull( foundException );
            Assert.IsTrue( foundException is AggregateException );

            AggregateException ex = (AggregateException)foundException;
            Assert.AreEqual( 1, ex.InnerExceptions.Count );
            Assert.IsTrue( ex.InnerExceptions[0] is ArgumentTooSmallException );
        }

        [Test]
        public void DoIntegerBindRequiredAboveMaxTest()
        {
            // Setup
            string[] arguments = new string[]
            {
                $"--target={nameof( IntegerBindTest )}",
                $"--{IntegerBind.RequiredArgName}={IntegerBind.RequiredArgMaxValue + 1}",
            };

            // Act
            int exitCode = CakeFrostingRunner.TryRunCake( arguments );

            // Check
            Assert.NotZero( exitCode );
            Assert.NotNull( foundException );
            Assert.IsTrue( foundException is AggregateException );

            AggregateException ex = (AggregateException)foundException;
            Assert.AreEqual( 1, ex.InnerExceptions.Count );
            Assert.IsTrue( ex.InnerExceptions[0] is ArgumentTooLargeException );
        }

        // ---------------- Helper Classes ----------------

        public sealed class IntegerBind
        {
            // ---------------- Fields ----------------

            internal const int RequiredArgMinValue = -5;
            internal const int RequiredArgMaxValue = 100;
            internal const string RequiredArgName = "int_required";
            private const string requiredArgDescription = "A Required Integer";

            internal const string OptionalArgName = "int_optional";
            private const string optionalArgDescription = "An optional Integer";
            internal const int DefaultValue = 10;

            // ---------------- Properties ----------------

            [IntegerArgument(
                RequiredArgName,
                Description = requiredArgDescription,
                HasSecretValue = false,
                Required = true,
                Min = RequiredArgMinValue,
                Max = RequiredArgMaxValue
            )]
            public int RequiredArg { get; set; }

            [IntegerArgument(
                OptionalArgName,
                Description = optionalArgDescription,
                HasSecretValue = false,
                DefaultValue = DefaultValue,
                Required = false
            )]
            public int OptionalArg { get; set; }

            // ---------------- Functions ----------------

            public override string ToString()
            {
                return ArgumentBinder.ConfigToStringHelper( this );
            }
        }

        [TaskName( nameof( IntegerBindTest ) )]
        public sealed class IntegerBindTest : BaseFrostingTask
        {
            public override void Run( ICakeContext context )
            {
                actualBind = context.CreateFromArguments<IntegerBind>();
                context.Information( actualBind.ToString() );
            }

            public override void OnError( Exception exception, ICakeContext context )
            {
                IntegerBindTests.foundException = exception;
                base.OnError( exception, context );
            }
        }
    }
}
