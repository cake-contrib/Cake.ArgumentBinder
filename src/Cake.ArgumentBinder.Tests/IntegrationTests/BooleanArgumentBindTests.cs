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
    public class BooleanArgumentBindTests
    {
        // ---------------- Fields ----------------

        private static BooleanBind actualBind;
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
        public void DoBooleanBindTest()
        {
            // Setup
            string[] arguments = new string[]
            {
                $"--target={nameof( BooleanArgumentBindTask )}",
                $"--{BooleanBind.RequiredArgName}={true}",
                $"--{BooleanBind.OptionalArgName}={true}"
            };

            // Act
            CakeFrostingRunner.RunCake( arguments );

            // Check
            Assert.NotNull( actualBind );
            Assert.IsTrue( actualBind.RequiredArg );
            Assert.IsTrue( actualBind.OptionalArg );
        }

        [Test]
        public void DoBooleanBindNoOptionalValuesTest()
        {
            // Setup
            string[] arguments = new string[]
            {
                $"--target={nameof( BooleanArgumentBindTask )}",
                $"--{BooleanBind.RequiredArgName}={false}"
            };

            // Act
            CakeFrostingRunner.RunCake( arguments );

            // Check
            Assert.NotNull( actualBind );
            Assert.IsFalse( actualBind.RequiredArg );
            Assert.AreEqual( BooleanBind.DefaultValue, actualBind.OptionalArg );
        }

        [Test]
        public void DoBooleanBindBindRequiredArgMissingTest()
        {
            // Setup
            string[] arguments = new string[]
            {
                $"--target={nameof( BooleanArgumentBindTask )}",
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

        private sealed class BooleanBind
        {
            // ---------------- Fields ----------------

            internal const string RequiredArgName = "bool_required";
            private const string requiredArgDescription = "A Required Boolean";

            internal const string OptionalArgName = "bool_optional";
            private const string optionalArgDescription = "An optional Boolean";
            internal const bool DefaultValue = true;

            // ---------------- Properties ----------------

            [BooleanArgument(
                RequiredArgName,
                Description = requiredArgDescription,
                HasSecretValue = false,
                Required = true,
                ArgumentSource = ArgumentSource.CommandLine
            )]
            public bool RequiredArg { get; set; }

            [BooleanArgument(
                OptionalArgName,
                Description = optionalArgDescription,
                HasSecretValue = false,
                DefaultValue = DefaultValue,
                Required = false,
                ArgumentSource = ArgumentSource.CommandLine
            )]
            public bool OptionalArg { get; set; }

            // ---------------- Functions ----------------

            public override string ToString()
            {
                return ArgumentBinder.ConfigToStringHelper( this );
            }
        }

        /// <remarks>
        /// Must be public so <see cref="Frosting"/> can find it.
        /// </remarks>
        [TaskName( nameof( BooleanArgumentBindTask ) )]
        public class BooleanArgumentBindTask : BaseFrostingTask
        {
            public override void Run( ICakeContext context )
            {
                actualBind = context.CreateFromArguments<BooleanBind>();
                context.Information( actualBind.ToString() );
            }

            public override void OnError( Exception exception, ICakeContext context )
            {
                BooleanArgumentBindTests.foundException = exception;
                base.OnError( exception, context );
            }
        }
    }
}
