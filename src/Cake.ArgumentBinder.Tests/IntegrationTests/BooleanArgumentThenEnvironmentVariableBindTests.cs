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
    public class BooleanArgumentThenEnvironmentVariableBindTests
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

            Environment.SetEnvironmentVariable( BooleanBind.RequiredArgName, null );
            Environment.SetEnvironmentVariable( BooleanBind.OptionalArgName, null );
        }

        [TearDown]
        public void TestTeardown()
        {
            Environment.SetEnvironmentVariable( BooleanBind.RequiredArgName, null );
            Environment.SetEnvironmentVariable( BooleanBind.OptionalArgName, null );

            actualBind = null;
            foundException = null;
        }

        // ---------------- Tests ----------------

        /// <summary>
        /// Ensures if both command line and environment variables are specified,
        /// command line takes priority.
        /// </summary>
        [Test]
        public void DoCommandLineOverridesEnvVarBindTest()
        {
            // Setup
            string[] arguments = new string[]
            {
                $"--target={nameof( BooleanArgumentThenEnvironmentVariableBindTask )}",
                $"--{BooleanBind.RequiredArgName}={true}",
                $"--{BooleanBind.OptionalArgName}={true}"
            };

            Environment.SetEnvironmentVariable( BooleanBind.RequiredArgName, false.ToString() );
            Environment.SetEnvironmentVariable( BooleanBind.OptionalArgName, false.ToString() );

            // Act
            CakeFrostingRunner.RunCake( arguments );

            // Check
            Assert.NotNull( actualBind );
            Assert.IsTrue( actualBind.RequiredArg );
            Assert.IsTrue( actualBind.OptionalArg );
        }

        /// <summary>
        /// Ensures if no command line arguments exist, but environment
        /// variables do, the environment variable values get bounded.
        /// </summary>
        [Test]
        public void DoEnvironmentVariableBackupTest()
        {
            // Setup
            string[] arguments = new string[]
            {
                $"--target={nameof( BooleanArgumentThenEnvironmentVariableBindTask )}",
            };

            Environment.SetEnvironmentVariable( BooleanBind.RequiredArgName, true.ToString() );
            Environment.SetEnvironmentVariable( BooleanBind.OptionalArgName, true.ToString() );

            // Act
            CakeFrostingRunner.RunCake( arguments );

            // Check
            Assert.NotNull( actualBind );
            Assert.IsTrue( actualBind.RequiredArg );
            Assert.IsTrue( actualBind.OptionalArg );
        }

        /// <summary>
        /// If optional values are not specified by either
        /// the command line or environment variable,
        /// the default value is used.
        /// </summary>
        [Test]
        public void DoBooleanBindNoOptionalValuesTest()
        {
            // Setup
            string[] arguments = new string[]
            {
                $"--target={nameof( BooleanArgumentThenEnvironmentVariableBindTask )}",
                $"--{BooleanBind.RequiredArgName}={false}"
            };

            Environment.SetEnvironmentVariable( BooleanBind.RequiredArgName, true.ToString() );

            // Act
            CakeFrostingRunner.RunCake( arguments );

            // Check
            Assert.NotNull( actualBind );
            Assert.IsFalse( actualBind.RequiredArg );
            Assert.AreEqual( BooleanBind.DefaultValue, actualBind.OptionalArg );
        }

        /// <summary>
        /// Ensures if a required argument is missing from both the command
        /// line and environment variable, we get an error.
        /// </summary>
        [Test]
        public void DoBooleanBindBindRequiredArgMissingTest()
        {
            // Setup
            string[] arguments = new string[]
            {
                $"--target={nameof( BooleanArgumentThenEnvironmentVariableBindTask )}",
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
                ArgumentSource = ArgumentSource.CommandLineThenEnvironmentVariable
            )]
            public bool RequiredArg { get; set; }

            [BooleanArgument(
                OptionalArgName,
                Description = optionalArgDescription,
                HasSecretValue = false,
                DefaultValue = DefaultValue,
                Required = false,
                ArgumentSource = ArgumentSource.CommandLineThenEnvironmentVariable
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
        [TaskName( nameof( BooleanArgumentThenEnvironmentVariableBindTask ) )]
        public class BooleanArgumentThenEnvironmentVariableBindTask : BaseFrostingTask
        {
            public override void Run( ICakeContext context )
            {
                actualBind = context.CreateFromArguments<BooleanBind>();
                context.Information( actualBind.ToString() );
            }

            public override void OnError( Exception exception, ICakeContext context )
            {
                BooleanArgumentThenEnvironmentVariableBindTests.foundException = exception;
                base.OnError( exception, context );
            }
        }
    }
}
