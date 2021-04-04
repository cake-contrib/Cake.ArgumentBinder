//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.IO;
using Cake.Core;
using Cake.Core.IO;
using Moq;
using NUnit.Framework;

namespace Cake.ArgumentBinder.Tests.UnitTests
{
    [TestFixture]
    public class FileArgumentAttributeTests
    {
        // ---------------- Fields ----------------

        private const string requiredArgName = "required_str_arg";

        private const string optionalArgName = "optional_str_arg";

        private const string testFileName = "Somewhere.txt";

        private Mock<IFileSystem> cakeFileSystem;
        private Mock<ICakeContext> cakeContext;
        private Mock<ICakeArguments> cakeArgs;

        // ---------------- Setup / Teardown ----------------

        [OneTimeSetUp]
        public void TestFixtureSetup()
        {
        }

        [OneTimeTearDown]
        public void TestFixtureTeardown()
        {
        }

        [SetUp]
        public void TestSetup()
        {
            this.cakeFileSystem = new Mock<IFileSystem>( MockBehavior.Strict );
            this.cakeArgs = new Mock<ICakeArguments>( MockBehavior.Strict );

            this.cakeContext = new Mock<ICakeContext>( MockBehavior.Strict );

            this.cakeContext.Setup(
                m => m.Arguments
            ).Returns( this.cakeArgs.Object );

            this.cakeContext.Setup(
                m => m.FileSystem
            ).Returns( this.cakeFileSystem.Object );
        }

        [TearDown]
        public void TestTeardown()
        {
        }

        // ---------------- Tests ----------------

        /// <summary>
        /// Ensures that if a required argument IS specified,
        /// everything works as expected.
        /// </summary>
        [Test]
        public void HasRequiredArgumentTest()
        {
            // Setup
            const string value = "somefile.txt";

            this.cakeArgs.Setup(
                m => m.HasArgument( requiredArgName )
            ).Returns( true );

            this.cakeArgs.SetupGetArgumentSingle(
                requiredArgName,
                value
            );

            // Act
            RequiredArgument uut = ArgumentBinderAliases.CreateFromArguments<RequiredArgument>( this.cakeContext.Object );

            // Check
            Assert.AreEqual( value, uut.FilePathProperty.ToString() );
        }

        /// <summary>
        /// Ensures that if a required argument is NOT specified,
        /// we get an exception.
        /// </summary>
        [Test]
        public void DoesNotHaveRequiredArgumentTest()
        {
            // Setup
            this.cakeArgs.Setup(
                m => m.HasArgument( requiredArgName )
            ).Returns( false );

            AggregateException e = Assert.Throws<AggregateException>(
                () => ArgumentBinderAliases.CreateFromArguments<RequiredArgument>( this.cakeContext.Object )
            );

            // Act
            Assert.AreEqual( 1, e.InnerExceptions.Count );

            // Check
            Assert.IsTrue( e.InnerExceptions[0] is MissingRequiredArgumentException );
        }

        /// <summary>
        /// Ensures that if we specify an optional argument,
        /// it gets used, not the default value.
        /// </summary>
        [Test]
        public void SpecifiedOptionalArgumentTest()
        {
            // Setup
            const string value = "somefile.txt";

            this.cakeArgs.Setup(
                m => m.HasArgument( optionalArgName )
            ).Returns( true );

            this.cakeArgs.SetupGetArgumentSingle(
                optionalArgName,
                value
            );

            // Act
            OptionalArgument uut = ArgumentBinderAliases.CreateFromArguments<OptionalArgument>( this.cakeContext.Object );

            // Check
            Assert.AreEqual( value, uut.FilePathProperty.ToString() );
        }

        /// <summary>
        /// Ensures that if we do NOT specify an optional argument,
        /// the default value gets used.
        /// </summary>
        [Test]
        public void UnspecifiedOptionalArgumentTest()
        {
            // Setup
            this.cakeArgs.Setup(
                m => m.HasArgument( optionalArgName )
            ).Returns( false );

            // Act
            OptionalArgument uut = ArgumentBinderAliases.CreateFromArguments<OptionalArgument>( this.cakeContext.Object );

            // Check
            Assert.AreEqual( testFileName, uut.FilePathProperty.ToString() );
        }

        /// <summary>
        /// Ensures that if we pass in an argument that is an empty string,
        /// we do not get an exception.
        /// </summary>
        [Test]
        public void EmptyStringTest()
        {
            // Setup
            this.cakeArgs.Setup(
                m => m.HasArgument( optionalArgName )
            ).Returns( true );

            this.cakeArgs.SetupGetArgumentSingle(
                optionalArgName,
                string.Empty
            );

            // Act
            OptionalArgument uut = ArgumentBinderAliases.CreateFromArguments<OptionalArgument>( this.cakeContext.Object );
            
            // Check
            Assert.AreEqual( string.Empty, uut.FilePathProperty.ToString() );
        }

        /// <summary>
        /// Ensures if a given argument is an empty string,
        /// we get an error.
        /// </summary>
        [Test]
        public void EmptyArgumentTest()
        {
            // Act
            AggregateException e = Assert.Throws<AggregateException>(
                () => ArgumentBinderAliases.CreateFromArguments<EmptyArgument>( this.cakeContext.Object )
            );

            // Check
            Assert.AreEqual( 1, e.InnerExceptions.Count );
            Assert.IsTrue( e.InnerExceptions[0] is AttributeValidationException );
        }

        /// <summary>
        /// Ensures if a given argument is an empty string,
        /// we get an error.
        /// </summary>
        [Test]
        public void MisconfiguredArgumentTest()
        {
            // Act
            AggregateException e = Assert.Throws<AggregateException>(
                () => ArgumentBinderAliases.CreateFromArguments<InvalidMustExistArgument>( this.cakeContext.Object )
            );

            // Check
            Assert.AreEqual( 1, e.InnerExceptions.Count );
            Assert.IsTrue( e.InnerExceptions[0] is AttributeValidationException );
        }

        [Test]
        public void ArgumentMustExistAndDoesTest()
        {
            // Setup
            FilePath path = new FilePath( testFileName );

            this.cakeArgs.Setup(
                m => m.HasArgument( requiredArgName )
            ).Returns( true );

            this.cakeArgs.SetupGetArgumentSingle(
                requiredArgName,
                testFileName
            );

            Mock<IFile> file = new Mock<IFile>( MockBehavior.Strict );
            file.Setup(
                m => m.Exists
            ).Returns( true );

            this.cakeFileSystem.Setup(
                // Can't pass in the path above, as FilePath does not implement Equals(),
                // so it does a reference equals by default, which is a problem
                // since we create a new instance of the class.
                m => m.GetFile( It.IsAny<FilePath>() )
            ).Returns( file.Object );

            // Act
            MustExistArgument uut = ArgumentBinderAliases.CreateFromArguments<MustExistArgument>( this.cakeContext.Object );

            // Check
            Assert.AreEqual( testFileName, uut.FilePathProperty.ToString() );
        }

        [Test]
        public void ArgumentMustExistAndDoesNotTest()
        {
            // Setup
            FilePath path = new FilePath( testFileName );

            this.cakeArgs.Setup(
                m => m.HasArgument( requiredArgName )
            ).Returns( true );

            this.cakeArgs.SetupGetArgumentSingle(
                requiredArgName,
                testFileName
            );

            Mock<IFile> file = new Mock<IFile>( MockBehavior.Strict );
            file.Setup(
                m => m.Exists
            ).Returns( false );

            this.cakeFileSystem.Setup(
                // Can't pass in the path above, as FilePath does not implement Equals(),
                // so it does a reference equals by default, which is a problem
                // since we create a new instance of the class.
                m => m.GetFile( It.IsAny<FilePath>() )
            ).Returns( file.Object );

            // Act
            AggregateException e = Assert.Throws<AggregateException>(
                () => ArgumentBinderAliases.CreateFromArguments<MustExistArgument>( this.cakeContext.Object )
            );

            // Check
            Assert.AreEqual( 1, e.InnerExceptions.Count );
            Assert.IsTrue( e.InnerExceptions[0] is FileNotFoundException );
        }

        [Test]
        public void ArgumentMustExistAndDoesNotSinceItsNullTest()
        {
            // Setup
            FilePath path = new FilePath( testFileName );

            this.cakeArgs.Setup(
                m => m.HasArgument( requiredArgName )
            ).Returns( true );

            this.cakeArgs.SetupGetArgumentSingle(
                requiredArgName,
                testFileName
            );

            this.cakeFileSystem.Setup(
                // Can't pass in the path above, as FilePath does not implement Equals(),
                // so it does a reference equals by default, which is a problem
                // since we create a new instance of the class.
                m => m.GetFile( It.IsAny<FilePath>() )
            ).Returns( default( IFile ) );

            // Act
            AggregateException e = Assert.Throws<AggregateException>(
                () => ArgumentBinderAliases.CreateFromArguments<MustExistArgument>( this.cakeContext.Object )
            );

            // Check
            Assert.AreEqual( 1, e.InnerExceptions.Count );
            Assert.IsTrue( e.InnerExceptions[0] is FileNotFoundException );
        }

        [Test]
        public void WrongTypeTest()
        {
            AggregateException e = Assert.Throws<AggregateException>(
               () => ArgumentBinderAliases.CreateFromArguments<MismatchedTypeArgument>( this.cakeContext.Object )
           );

            Assert.AreEqual( 1, e.InnerExceptions.Count );
            Assert.IsTrue( e.InnerExceptions[0] is InvalidPropertyTypeForAttributeException );
        }

        // ---------------- Helper Classes ----------------

        private class RequiredArgument
        {
            [FilePathArgument(
                requiredArgName,
                DefaultValue = null,
                Description = "A required " + nameof( FilePath ) +" argument",
                Required = true
            )]
            public FilePath FilePathProperty { get; set; }
        }

        private class OptionalArgument
        {
            [FilePathArgument(
                optionalArgName,
                DefaultValue = testFileName,
                Description = "An optional argument",
                Required = false
            )]
            public FilePath FilePathProperty { get; set; }
        }

        private class EmptyArgument
        {
            [FilePathArgument(
                "",
                DefaultValue = "Should not work",
                Description = "This shouldnt work.",
                Required = false
            )]
            public FilePath FilePathProperty { get; set; }
        }

        private class MustExistArgument
        {
            [FilePathArgument(
                requiredArgName,
                Description = "This file must exist before binding the arguments",
                MustExist = true,
                Required = true
            )]
            public FilePath FilePathProperty { get; set; }
        }

        private class InvalidMustExistArgument
        {
            [FilePathArgument(
                optionalArgName,
                Description = "This is mis-configured.",
                MustExist = true,
                Required = false,
                DefaultValue = null
            )]
            public FilePath FilePathProperty { get; set; }
        }

        private class MismatchedTypeArgument
        {
            [FilePathArgument(
                requiredArgName
            )]
            public string MismatchedType { get; set; }
        }
    }
}
