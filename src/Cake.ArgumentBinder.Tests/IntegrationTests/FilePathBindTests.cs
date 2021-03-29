//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.Reflection;
using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using NUnit.Framework;

namespace Cake.ArgumentBinder.Tests.IntegrationTests
{
    [TestFixture]
    public class FilePathBindTests
    {
        // ---------------- Fields ----------------

        private FilePath exePath;

        private static FilePathBind actualBind;
        private static Exception foundException;

        // ---------------- Setup / Teardown ----------------

        [SetUp]
        public void TestSetup()
        {
            this.exePath = new FilePath( Assembly.GetExecutingAssembly().Location );
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
        public void DoFilePathBindTest()
        {
            // Setup
            const string requiredArgValue = "somewhere.txt";
            const string optionalArgValue = "someplace.txt";
            const string nullArgValue = "a thing.txt";
            string[] arguments = new string[]
            {
                $"--target={nameof( FilePathBindTask )}",
                $"--{FilePathBind.RequiredArgName}=\"{requiredArgValue}\"",
                $"--{FilePathBind.OptionalArgName}=\"{optionalArgValue}\"",
                $"--{FilePathBind.NullArgName}={nullArgValue}",
                $"--{FilePathBind.MustExistArgName}=\"{exePath}\""
            };

            // Act
            CakeFrostingRunner.RunCake( arguments );

            // Check
            Assert.NotNull( actualBind );
            Assert.Null( foundException );
            Assert.AreEqual( requiredArgValue, actualBind.RequiredArg.ToString() );
            Assert.AreEqual( optionalArgValue, actualBind.OptionalArg.ToString() );
            Assert.AreEqual( nullArgValue, actualBind.NullDefaultArg.ToString() );
            Assert.AreEqual( exePath.ToString(), actualBind.FileMustExistArg.ToString() );
        }

        [Test]
        public void DoFilePathBindNoOptionalValuesTest()
        {
            // Setup
            const string requiredArgValue = "HelloWorld.txt";
            string[] arguments = new string[]
            {
                $"--target={nameof( FilePathBindTask )}",
                $"--{FilePathBind.RequiredArgName}={requiredArgValue}",
                $"--{FilePathBind.MustExistArgName}=\"{exePath}\""
            };

            // Act
            CakeFrostingRunner.RunCake( arguments );

            // Check
            Assert.NotNull( actualBind );
            Assert.Null( foundException );
            Assert.AreEqual( requiredArgValue, actualBind.RequiredArg.ToString() );
            Assert.AreEqual( FilePathBind.DefaultValue.ToString(), actualBind.OptionalArg.ToString() );
            Assert.AreEqual( FilePathBind.NullDefaultValue, actualBind.NullDefaultArg );
            Assert.AreEqual( exePath.ToString(), actualBind.FileMustExistArg.ToString() );
        }

        [Test]
        public void DoFilePathBindRequiredArgMissingTest()
        {
            // Setup
            string[] arguments = new string[]
            {
                $"--target={nameof( FilePathBindTask )}",
            };

            // Act
            int exitCode = CakeFrostingRunner.TryRunCake( arguments );

            // Check
            Assert.NotZero( exitCode );
            Assert.NotNull( foundException );
            Assert.IsTrue( foundException is AggregateException );

            AggregateException ex = (AggregateException)foundException;
            // 2 Missing arguments should result in 2 exceptions.
            Assert.AreEqual( 2, ex.InnerExceptions.Count );
            Assert.IsTrue( ex.InnerExceptions[0] is MissingRequiredArgumentException );
            Assert.IsTrue( ex.InnerExceptions[1] is MissingRequiredArgumentException );
        }

        // ---------------- Helper Classes ----------------

        public class FilePathBind
        {
            // ---------------- Fields ----------------

            internal const string RequiredArgName = "file_required";
            private const string requiredArgDescription = "A Required file";

            internal const string OptionalArgName = "file_optional";
            private const string optionalArgDescription = "An optional file";
            internal const string DefaultValue = "Hello";

            internal const string NullArgName = "file_nullarg";
            private const string nullArgDescription = "A file whose default value is null";
            internal const string NullDefaultValue = null;

            internal const string MustExistArgName = "file_mustexist";
            private const string mustExistArgDescription = "Must exist optional file";

            // ---------------- Properties ----------------

            [FilePathArgument(
                RequiredArgName,
                Description = requiredArgDescription,
                HasSecretValue = false,
                Required = true,
                MustExist = false
            )]
            public FilePath RequiredArg { get; set; }

            [FilePathArgument(
                OptionalArgName,
                Description = optionalArgDescription,
                HasSecretValue = false,
                DefaultValue = DefaultValue,
                Required = false,
                MustExist = false
            )]
            public FilePath OptionalArg { get; set; }

            [FilePathArgument(
                NullArgName,
                Description = nullArgDescription,
                HasSecretValue = false,
                DefaultValue = NullDefaultValue,
                Required = false,
                MustExist = false
            )]
            public FilePath NullDefaultArg { get; set; }

            [FilePathArgument(
                MustExistArgName,
                Description = mustExistArgDescription,
                HasSecretValue = false,
                Required = true,
                MustExist = true
            )]
            public FilePath FileMustExistArg { get; set; }

            // ---------------- Functions ----------------

            public override string ToString()
            {
                return ArgumentBinder.ConfigToStringHelper( this );
            }
        }

        [TaskName( nameof( FilePathBindTask ) )]
        public class FilePathBindTask : BaseFrostingTask
        {
            public override void Run( ICakeContext context )
            {
                actualBind = context.CreateFromArguments<FilePathBind>();
                context.Information( actualBind.ToString() );
            }

            public override void OnError( Exception exception, ICakeContext context )
            {
                FilePathBindTests.foundException = exception;
                base.OnError( exception, context );
            }
        }
    }
}
