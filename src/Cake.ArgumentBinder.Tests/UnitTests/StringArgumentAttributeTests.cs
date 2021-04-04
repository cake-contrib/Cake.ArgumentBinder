//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using Cake.Core;
using Cake.Core.IO;
using Moq;
using NUnit.Framework;

namespace Cake.ArgumentBinder.Tests.UnitTests
{
    [TestFixture]
    public class StringArgumentAttributeTests
    {
        // ---------------- Fields ----------------

        private const string requiredArgName = "required_str_arg";

        private const string optionalArgName = "optional_str_arg";

        private const string defaultValue = "My String";

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
            this.cakeArgs = new Mock<ICakeArguments>( MockBehavior.Strict );

            this.cakeContext = new Mock<ICakeContext>( MockBehavior.Strict );
            this.cakeContext.Setup(
                m => m.Arguments
            ).Returns( this.cakeArgs.Object );
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
            const string value = "Arg Value";

            this.cakeArgs.Setup(
                m => m.HasArgument( requiredArgName )
            ).Returns( true );

            this.cakeArgs.SetupGetArgumentSingle(
                requiredArgName,
                value
            );

            RequiredArgument uut = ArgumentBinderAliases.CreateFromArguments<RequiredArgument>( this.cakeContext.Object );
            Assert.AreEqual( value, uut.StringProperty );
        }

        /// <summary>
        /// Ensures that if a required argument is NOT specified,
        /// we get an exception.
        /// </summary>
        [Test]
        public void DoesNotHaveRequiredArgumentTest()
        {
            this.cakeArgs.Setup(
                m => m.HasArgument( requiredArgName )
            ).Returns( false );

            AggregateException e = Assert.Throws<AggregateException>(
                () => ArgumentBinderAliases.CreateFromArguments<RequiredArgument>( this.cakeContext.Object )
            );

            Assert.AreEqual( 1, e.InnerExceptions.Count );
            Assert.IsTrue( e.InnerExceptions[0] is MissingRequiredArgumentException );
        }

        /// <summary>
        /// Ensures that if we specify an optional argument,
        /// it gets used, not the default value.
        /// </summary>
        [Test]
        public void SpecifiedOptionalArgumentTest()
        {
            const string value = "My Value";

            this.cakeArgs.Setup(
                m => m.HasArgument( optionalArgName )
            ).Returns( true );

            this.cakeArgs.SetupGetArgumentSingle(
                optionalArgName,
                value
            );

            OptionalArgument uut = ArgumentBinderAliases.CreateFromArguments<OptionalArgument>( this.cakeContext.Object );
            Assert.AreEqual( value, uut.StringProperty );
        }

        /// <summary>
        /// Ensures that if we do NOT specify an optional argument,
        /// the default value gets used.
        /// </summary>
        [Test]
        public void UnspecifiedOptionalArgumentTest()
        {
            this.cakeArgs.Setup(
                m => m.HasArgument( optionalArgName )
            ).Returns( false );

            OptionalArgument uut = ArgumentBinderAliases.CreateFromArguments<OptionalArgument>( this.cakeContext.Object );
            Assert.AreEqual( defaultValue, uut.StringProperty );
        }

        /// <summary>
        /// Ensures that if we pass in an argument that is an empt string,
        /// we do not get an exception.
        /// </summary>
        [Test]
        public void EmptyStringTest()
        {
            this.cakeArgs.Setup(
                m => m.HasArgument( optionalArgName )
            ).Returns( true );

            this.cakeArgs.SetupGetArgumentSingle(
                optionalArgName,
                string.Empty
            );

            OptionalArgument uut = ArgumentBinderAliases.CreateFromArguments<OptionalArgument>( this.cakeContext.Object );
            Assert.AreEqual( string.Empty, uut.StringProperty );
        }

        /// <summary>
        /// Ensures if a given argument is an empty string,
        /// we get an error.
        /// </summary>
        [Test]
        public void EmptyArgumentTest()
        {
            AggregateException e = Assert.Throws<AggregateException>(
                () => ArgumentBinderAliases.CreateFromArguments<EmptyArgument>( this.cakeContext.Object )
            );

            Assert.AreEqual( 1, e.InnerExceptions.Count );
            Assert.IsTrue( e.InnerExceptions[0] is AttributeValidationException );
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
            [StringArgument( requiredArgName, DefaultValue = defaultValue, Description = "A required boolean argument", Required = true )]
            public string StringProperty { get; set; }
        }

        private class OptionalArgument
        {
            [StringArgument( optionalArgName, DefaultValue = defaultValue, Description = "An optional argument", Required = false )]
            public string StringProperty { get; set; }
        }

        private class EmptyArgument
        {
            [StringArgument( "", DefaultValue = "Should not work", Description = "This shouldnt work.", Required = false )]
            public string StringProperty { get; set; }
        }

        private class MismatchedTypeArgument
        {
            [StringArgument( requiredArgName )]
            public FilePath FilePathProperty { get; set; }
        }
    }
}
