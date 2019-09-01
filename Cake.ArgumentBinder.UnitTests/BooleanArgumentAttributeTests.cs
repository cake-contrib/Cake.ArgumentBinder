//
// Copyright Seth Hendrick 2019.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using Cake.Core;
using Moq;
using NUnit.Framework;

namespace Cake.ArgumentBinder.UnitTests
{
    [TestFixture]
    public class BooleanArgumentAttributeTests
    {
        // ---------------- Fields ----------------

        private const string requiredArgName = "required_bool_arg";

        private const string optionalArgName = "optional_bool_arg";

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
            this.cakeArgs.Setup(
                m => m.HasArgument( requiredArgName )
            ).Returns( true );

            this.cakeArgs.Setup(
                m => m.GetArgument( requiredArgName )
            ).Returns( "true" );

            RequiredArgument uut = ArgumentBinder.FromArguments<RequiredArgument>( this.cakeContext.Object );
            Assert.IsTrue( uut.BoolProperty );
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
                () => ArgumentBinder.FromArguments<RequiredArgument>( this.cakeContext.Object )
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
            this.cakeArgs.Setup(
                m => m.HasArgument( optionalArgName )
            ).Returns( true );

            this.cakeArgs.Setup(
                m => m.GetArgument( optionalArgName )
            ).Returns( "false" );

            OptionalArgument uut = ArgumentBinder.FromArguments<OptionalArgument>( this.cakeContext.Object );
            Assert.IsFalse( uut.BoolProperty );
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

            OptionalArgument uut = ArgumentBinder.FromArguments<OptionalArgument>( this.cakeContext.Object );
            Assert.IsTrue( uut.BoolProperty );
        }

        // ---------------- Helper Classes ----------------

        private class RequiredArgument
        {
            [BooleanArgument( requiredArgName, DefaultValue = true, Description = "A required boolean argument", Required = true )]
            public bool BoolProperty { get; set; }
        }

        private class OptionalArgument
        {
            [BooleanArgument( optionalArgName, DefaultValue = true, Description = "An optional argument", Required = false )]
            public bool BoolProperty { get; set; }
        }
    }
}
