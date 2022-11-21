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
    public class StringBindTests
    {
        // ---------------- Fields ----------------

        private static StringBind actualBind;
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
        public void DoStringBindTest()
        {
            // Setup
            const string arg1 = "Hello World";
            const string arg2 = "SomeOtherArg";
            const string arg3 = ":)";
            string[] arguments = new string[]
            {
                $"--target={nameof( StringBindTask )}",
                $"--{StringBind.RequiredArgName}=\"{arg1}\"",
                $"--{StringBind.OptionalArgName}=\"{arg2}\"",
                $"--{StringBind.NullArgName}={arg3}"
            };

            // Act
            CakeFrostingRunner.RunCake( arguments );

            // Check
            Assert.NotNull( actualBind );
            Assert.Null( foundException );
            Assert.AreEqual( arg1, actualBind.RequiredArg );
            Assert.AreEqual( arg2, actualBind.OptionalArg );
            Assert.AreEqual( arg3, actualBind.NullDefaultArg );
        }

        [Test]
        public void DoStringBindNoOptionalValuesTest()
        {
            // Setup
            const string arg1 = "HelloWorld";
            string[] arguments = new string[]
            {
                $"--target={nameof( StringBindTask )}",
                $"--{StringBind.RequiredArgName}={arg1}",
            };

            // Act
            CakeFrostingRunner.RunCake( arguments );

            // Check
            Assert.NotNull( actualBind );
            Assert.Null( foundException );
            Assert.AreEqual( arg1, actualBind.RequiredArg );
            Assert.AreEqual( StringBind.DefaultValue, actualBind.OptionalArg );
            Assert.AreEqual( StringBind.NullDefaultValue, actualBind.NullDefaultArg );
        }

        [Test]
        public void DoStringBindRequiredArgMissingTest()
        {
            // Setup
            string[] arguments = new string[]
            {
                $"--target={nameof( StringBindTask )}",
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

        private sealed class StringBind
        {
            // ---------------- Fields ----------------

            internal const string RequiredArgName = "string_required";
            private const string requiredArgDescription = "A Required string";

            internal const string OptionalArgName = "string_optional";
            private const string optionalArgDescription = "An optional string";
            internal const string DefaultValue = "Hello";

            internal const string NullArgName = "string_nullarg";
            private const string nullArgDescription = "A string show default value is null";
            internal const string NullDefaultValue = null;

            // ---------------- Properties ----------------

            [StringArgument(
                RequiredArgName,
                Description = requiredArgDescription,
                HasSecretValue = false,
                Required = true
            )]
            public string RequiredArg { get; set; }

            [StringArgument(
                OptionalArgName,
                Description = optionalArgDescription,
                HasSecretValue = false,
                DefaultValue = DefaultValue,
                Required = false
            )]
            public string OptionalArg { get; set; }

            [StringArgument(
                NullArgName,
                Description = nullArgDescription,
                HasSecretValue = false,
                DefaultValue = NullDefaultValue,
                Required = false
            )]
            public string NullDefaultArg { get; set; }

            // ---------------- Functions ----------------

            public override string ToString()
            {
                return ArgumentBinder.ConfigToStringHelper( this );
            }
        }

        /// <remarks>
        /// Must be public so <see cref="Frosting"/> can find it.
        /// </remarks>
        [TaskName( nameof( StringBindTask ) )]
        public class StringBindTask : BaseFrostingTask
        {
            public override void Run( ICakeContext context )
            {
                actualBind = context.CreateFromArguments<StringBind>();
                context.Information( actualBind.ToString() );
            }

            public override void OnError( Exception exception, ICakeContext context )
            {
                StringBindTests.foundException = exception;
                base.OnError( exception, context );
            }
        }
    }
}
