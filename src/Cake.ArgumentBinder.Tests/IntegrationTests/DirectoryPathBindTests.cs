//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.IO;
using System.Reflection;
using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using NUnit.Framework;

namespace Cake.ArgumentBinder.Tests.IntegrationTests
{
    [TestFixture]
    public class DirectoryPathBindTests
    {
        // ---------------- Fields ----------------

        private DirectoryPath exeFolder;

        private static DirectoryPathBind actualBind;
        private static Exception foundException;

        // ---------------- Setup / Teardown ----------------

        [SetUp]
        public void TestSetup()
        {
            FilePath exePath = new FilePath( Assembly.GetExecutingAssembly().Location );
            this.exeFolder = exePath.GetDirectory();
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
        public void DoDirectoryPathBindTest()
        {
            // Setup
            const string requiredArgValue = "somewhere";
            const string optionalArgValue = "someplace";
            const string nullArgValue = "a dir";
            string[] arguments = new string[]
            {
                $"--target={nameof( DirectoryPathBindTask )}",
                $"--{DirectoryPathBind.RequiredArgName}=\"{requiredArgValue}\"",
                $"--{DirectoryPathBind.OptionalArgName}=\"{optionalArgValue}\"",
                $"--{DirectoryPathBind.NullArgName}={nullArgValue}",
                $"--{DirectoryPathBind.MustExistArgName}=\"{exeFolder}\""
            };

            // Act
            CakeFrostingRunner.RunCake( arguments );

            // Check
            Assert.NotNull( actualBind );
            Assert.Null( foundException );
            Assert.AreEqual( requiredArgValue, actualBind.RequiredArg.ToString() );
            Assert.AreEqual( optionalArgValue, actualBind.OptionalArg.ToString() );
            Assert.AreEqual( nullArgValue, actualBind.NullDefaultArg.ToString() );
            Assert.AreEqual( exeFolder.ToString(), actualBind.DirMustExistArg.ToString() );
        }

        [Test]
        public void DoDirectoryPathBindNoOptionalValuesTest()
        {
            // Setup
            const string requiredArgValue = "HelloDir";
            string[] arguments = new string[]
            {
                $"--target={nameof( DirectoryPathBindTask )}",
                $"--{DirectoryPathBind.RequiredArgName}={requiredArgValue}",
                $"--{DirectoryPathBind.MustExistArgName}=\"{exeFolder}\""
            };

            // Act
            CakeFrostingRunner.RunCake( arguments );

            // Check
            Assert.NotNull( actualBind );
            Assert.Null( foundException );
            Assert.AreEqual( requiredArgValue, actualBind.RequiredArg.ToString() );
            Assert.AreEqual( DirectoryPathBind.DefaultValue.ToString(), actualBind.OptionalArg.ToString() );
            Assert.AreEqual( DirectoryPathBind.NullDefaultValue, actualBind.NullDefaultArg );
            Assert.AreEqual( exeFolder.ToString(), actualBind.DirMustExistArg.ToString() );
        }

        [Test]
        public void DoDirectoryPathBindRequiredArgMissingTest()
        {
            // Setup
            string[] arguments = new string[]
            {
                $"--target={nameof( DirectoryPathBindTask )}",
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

        [Test]
        public void DoDirectoryPathBindDirDoesNotExistTest()
        {
            // Setup
            const string requiredArgValue = "somewhere";
            const string optionalArgValue = "someplace";
            const string nullArgValue = "a dir";
            string[] arguments = new string[]
            {
                $"--target={nameof( DirectoryPathBindTask )}",
                $"--{DirectoryPathBind.RequiredArgName}=\"{requiredArgValue}\"",
                $"--{DirectoryPathBind.OptionalArgName}=\"{optionalArgValue}\"",
                $"--{DirectoryPathBind.NullArgName}={nullArgValue}",
                $"--{DirectoryPathBind.MustExistArgName}=\"{exeFolder}.txt\"" // <-.txt to exe makes it not exist.
            };

            // Act
            int exitCode = CakeFrostingRunner.TryRunCake( arguments );

            // Check
            Assert.NotZero( exitCode );
            Assert.NotNull( foundException );
            Assert.IsTrue( foundException is AggregateException );

            AggregateException ex = (AggregateException)foundException;
            Assert.AreEqual( 1, ex.InnerExceptions.Count );
            Assert.IsTrue( ex.InnerExceptions[0] is DirectoryNotFoundException );
        }

        // ---------------- Helper Classes ----------------

        public class DirectoryPathBind
        {
            // ---------------- Fields ----------------

            internal const string RequiredArgName = "dir_required";
            private const string requiredArgDescription = "A Required directory";

            internal const string OptionalArgName = "dir_optional";
            private const string optionalArgDescription = "An optional directory";
            internal const string DefaultValue = "Hello";

            internal const string NullArgName = "dir_nullarg";
            private const string nullArgDescription = "A directory whose default value is null";
            internal const string NullDefaultValue = null;

            internal const string MustExistArgName = "dir_mustexist";
            private const string mustExistArgDescription = "Must exist optional directory";

            // ---------------- Properties ----------------

            [DirectoryPathArgument(
                RequiredArgName,
                Description = requiredArgDescription,
                HasSecretValue = false,
                Required = true,
                MustExist = false
            )]
            public DirectoryPath RequiredArg { get; set; }

            [DirectoryPathArgument(
                OptionalArgName,
                Description = optionalArgDescription,
                HasSecretValue = false,
                DefaultValue = DefaultValue,
                Required = false,
                MustExist = false
            )]
            public DirectoryPath OptionalArg { get; set; }

            [DirectoryPathArgument(
                NullArgName,
                Description = nullArgDescription,
                HasSecretValue = false,
                DefaultValue = NullDefaultValue,
                Required = false,
                MustExist = false
            )]
            public DirectoryPath NullDefaultArg { get; set; }

            [DirectoryPathArgument(
                MustExistArgName,
                Description = mustExistArgDescription,
                HasSecretValue = false,
                Required = true,
                MustExist = true
            )]
            public DirectoryPath DirMustExistArg { get; set; }

            // ---------------- Functions ----------------

            public override string ToString()
            {
                return ArgumentBinder.ConfigToStringHelper( this );
            }
        }

        [TaskName( nameof( DirectoryPathBindTask ) )]
        public class DirectoryPathBindTask : BaseFrostingTask
        {
            public override void Run( ICakeContext context )
            {
                actualBind = context.CreateFromArguments<DirectoryPathBind>();
                context.Information( actualBind.ToString() );
            }

            public override void OnError( Exception exception, ICakeContext context )
            {
                DirectoryPathBindTests.foundException = exception;
                base.OnError( exception, context );
            }
        }
    }
}
