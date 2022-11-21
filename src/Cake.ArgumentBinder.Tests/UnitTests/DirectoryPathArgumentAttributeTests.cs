//
// Copyright Seth Hendrick 2019-2022.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying Directory LICENSE in the root of the repository).
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
    public sealed class DirectoryPathArgumentAttributeTests
    {
        // ---------------- Fields ----------------

        private const string requiredArgName = "required_str_arg";

        private const string optionalArgName = "optional_str_arg";

        private const string testDirectoryName = "somedir";

        private Mock<IFileSystem> cakeDirectorySystem;
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
            this.cakeDirectorySystem = new Mock<IFileSystem>( MockBehavior.Strict );
            this.cakeArgs = new Mock<ICakeArguments>( MockBehavior.Strict );

            this.cakeContext = new Mock<ICakeContext>( MockBehavior.Strict );

            this.cakeContext.Setup(
                m => m.Arguments
            ).Returns( this.cakeArgs.Object );

            this.cakeContext.Setup(
                m => m.FileSystem
            ).Returns( this.cakeDirectorySystem.Object );
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
            const string value = "somedir";

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
            Assert.AreEqual( value, uut.DirectoryPathProperty.ToString() );
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
            const string value = "somedir";

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
            Assert.AreEqual( value, uut.DirectoryPathProperty.ToString() );
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
            Assert.AreEqual( testDirectoryName, uut.DirectoryPathProperty.ToString() );
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
            Assert.AreEqual( string.Empty, uut.DirectoryPathProperty.ToString() );
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
            DirectoryPath path = new DirectoryPath( testDirectoryName );

            this.cakeArgs.Setup(
                m => m.HasArgument( requiredArgName )
            ).Returns( true );

            this.cakeArgs.SetupGetArgumentSingle(
                requiredArgName,
                testDirectoryName
            );

            Mock<IDirectory> Directory = new Mock<IDirectory>( MockBehavior.Strict );
            Directory.Setup(
                m => m.Exists
            ).Returns( true );

            this.cakeDirectorySystem.Setup(
                // Can't pass in the path above, as DirectoryPath does not implement Equals(),
                // so it does a reference equals by default, which is a problem
                // since we create a new instance of the class.
                m => m.GetDirectory( It.IsAny<DirectoryPath>() )
            ).Returns( Directory.Object );

            // Act
            MustExistArgument uut = ArgumentBinderAliases.CreateFromArguments<MustExistArgument>( this.cakeContext.Object );

            // Check
            Assert.AreEqual( testDirectoryName, uut.DirectoryPathProperty.ToString() );
        }

        [Test]
        public void ArgumentMustExistAndDoesNotTest()
        {
            // Setup
            DirectoryPath path = new DirectoryPath( testDirectoryName );

            this.cakeArgs.Setup(
                m => m.HasArgument( requiredArgName )
            ).Returns( true );

            this.cakeArgs.SetupGetArgumentSingle(
                requiredArgName,
                testDirectoryName
            );

            Mock<IDirectory> Directory = new Mock<IDirectory>( MockBehavior.Strict );
            Directory.Setup(
                m => m.Exists
            ).Returns( false );

            this.cakeDirectorySystem.Setup(
                // Can't pass in the path above, as DirectoryPath does not implement Equals(),
                // so it does a reference equals by default, which is a problem
                // since we create a new instance of the class.
                m => m.GetDirectory( It.IsAny<DirectoryPath>() )
            ).Returns( Directory.Object );

            // Act
            AggregateException e = Assert.Throws<AggregateException>(
                () => ArgumentBinderAliases.CreateFromArguments<MustExistArgument>( this.cakeContext.Object )
            );

            // Check
            Assert.AreEqual( 1, e.InnerExceptions.Count );
            Assert.IsTrue( e.InnerExceptions[0] is DirectoryNotFoundException );
        }

        [Test]
        public void ArgumentMustExistAndDoesNotSinceItsNullTest()
        {
            // Setup
            DirectoryPath path = new DirectoryPath( testDirectoryName );

            this.cakeArgs.Setup(
                m => m.HasArgument( requiredArgName )
            ).Returns( true );

            this.cakeArgs.SetupGetArgumentSingle(
                requiredArgName,
                testDirectoryName
            );

            this.cakeDirectorySystem.Setup(
                // Can't pass in the path above, as DirectoryPath does not implement Equals(),
                // so it does a reference equals by default, which is a problem
                // since we create a new instance of the class.
                m => m.GetDirectory( It.IsAny<DirectoryPath>() )
            ).Returns( default( IDirectory ) );

            // Act
            AggregateException e = Assert.Throws<AggregateException>(
                () => ArgumentBinderAliases.CreateFromArguments<MustExistArgument>( this.cakeContext.Object )
            );

            // Check
            Assert.AreEqual( 1, e.InnerExceptions.Count );
            Assert.IsTrue( e.InnerExceptions[0] is DirectoryNotFoundException );
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
            [DirectoryPathArgument(
                requiredArgName,
                DefaultValue = null,
                Description = "A required " + nameof( DirectoryPath ) +" argument",
                Required = true
            )]
            public DirectoryPath DirectoryPathProperty { get; set; }
        }

        private class OptionalArgument
        {
            [DirectoryPathArgument(
                optionalArgName,
                DefaultValue = testDirectoryName,
                Description = "An optional argument",
                Required = false
            )]
            public DirectoryPath DirectoryPathProperty { get; set; }
        }

        private class EmptyArgument
        {
            [DirectoryPathArgument(
                "",
                DefaultValue = "Should not work",
                Description = "This shouldnt work.",
                Required = false
            )]
            public DirectoryPath DirectoryPathProperty { get; set; }
        }

        private class MustExistArgument
        {
            [DirectoryPathArgument(
                requiredArgName,
                Description = "This Directory must exist before binding the arguments",
                MustExist = true,
                Required = true
            )]
            public DirectoryPath DirectoryPathProperty { get; set; }
        }

        private class InvalidMustExistArgument
        {
            [DirectoryPathArgument(
                optionalArgName,
                Description = "This is mis-configured.",
                MustExist = true,
                Required = false,
                DefaultValue = null
            )]
            public DirectoryPath DirectoryPathProperty { get; set; }
        }

        private class MismatchedTypeArgument
        {
            [DirectoryPathArgument(
                requiredArgName
            )]
            public string MismatchedType { get; set; }
        }
    }
}
