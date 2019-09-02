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
    public class IntegerArgumentAttributeTests
    {
        // ---------------- Fields ----------------

        private const int minValue = 1;

        private const int maxValue = 10;

        private const int defaultValue = 5;

        private const string requiredArgName = "required_int_arg";

        private const string optionalArgName = "optional_int_arg";

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
            ).Returns( minValue.ToString() );

            RequiredArgument uut = ArgumentBinder.FromArguments<RequiredArgument>( this.cakeContext.Object );
            Assert.AreEqual( minValue, uut.IntProperty );
        }

        /// <summary>
        /// Ensures that if a required argument IS specified, and is in range,
        /// everything works as expected.
        /// </summary>
        [Test]
        public void InRangeSpecifiedRequiredArgumentTest()
        {
            {
                this.cakeArgs.Setup(
                    m => m.HasArgument( requiredArgName )
                ).Returns( true );

                this.cakeArgs.Setup(
                    m => m.GetArgument( requiredArgName )
                ).Returns( ( minValue + 1 ).ToString() );

                RequiredArgument uut = ArgumentBinder.FromArguments<RequiredArgument>( this.cakeContext.Object );
                Assert.AreEqual( minValue + 1, uut.IntProperty );
            }

            {
                this.cakeArgs.Setup(
                    m => m.HasArgument( requiredArgName )
                ).Returns( true );

                this.cakeArgs.Setup(
                    m => m.GetArgument( requiredArgName )
                ).Returns( ( maxValue - 1 ).ToString() );

                RequiredArgument uut = ArgumentBinder.FromArguments<RequiredArgument>( this.cakeContext.Object );
                Assert.AreEqual( maxValue - 1, uut.IntProperty );
            } 
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
            ).Returns( maxValue.ToString() );

            OptionalArgument uut = ArgumentBinder.FromArguments<OptionalArgument>( this.cakeContext.Object );
            Assert.AreEqual( maxValue, uut.IntProperty );
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
            Assert.AreEqual( defaultValue, uut.IntProperty );
        }

        /// <summary>
        /// Ensures that if we pass in an argument that is not an integer,
        /// we get an exception.
        /// </summary>
        [Test]
        public void FormatExceptionTest()
        {
            this.cakeArgs.Setup(
                m => m.HasArgument( optionalArgName )
            ).Returns( true );

            this.cakeArgs.Setup(
                m => m.GetArgument( optionalArgName )
            ).Returns( "lolImNotAnInt" );

            AggregateException e = Assert.Throws<AggregateException>(
                () => ArgumentBinder.FromArguments<OptionalArgument>( this.cakeContext.Object )
            );

            Assert.AreEqual( 1, e.InnerExceptions.Count );
            Assert.IsTrue( e.InnerExceptions[0] is ArgumentFormatException );
        }

        /// <summary>
        /// Ensures if a given argument is an empty string,
        /// we get an error.
        /// </summary>
        [Test]
        public void EmptyArgumentTest()
        {
            AggregateException e = Assert.Throws<AggregateException>(
                () => ArgumentBinder.FromArguments<EmptyArgument>( this.cakeContext.Object )
            );

            Assert.AreEqual( 1, e.InnerExceptions.Count );
            Assert.IsTrue( e.InnerExceptions[0] is AttributeValidationException );
        }

        /// <summary>
        /// Ensures if an argument is too big, we get an exception.
        /// </summary>
        [Test]
        public void ArgumentTooBigTest()
        {
            this.cakeArgs.Setup(
                m => m.HasArgument( optionalArgName )
            ).Returns( true );

            this.cakeArgs.Setup(
                m => m.GetArgument( optionalArgName )
            ).Returns( ( maxValue + 1 ).ToString() );

            AggregateException e = Assert.Throws<AggregateException>(
                () => ArgumentBinder.FromArguments<OptionalArgument>( this.cakeContext.Object )
            );

            Assert.AreEqual( 1, e.InnerExceptions.Count );
            Assert.IsTrue( e.InnerExceptions[0] is ArgumentTooLargeException );
        }

        /// <summary>
        /// Ensures if an argument is too small, we get an exception.
        /// </summary>
        [Test]
        public void ArgumentTooSmallTest()
        {
            this.cakeArgs.Setup(
                m => m.HasArgument( optionalArgName )
            ).Returns( true );

            this.cakeArgs.Setup(
                m => m.GetArgument( optionalArgName )
            ).Returns( ( minValue - 1 ).ToString() );

            AggregateException e = Assert.Throws<AggregateException>(
                () => ArgumentBinder.FromArguments<OptionalArgument>( this.cakeContext.Object )
            );

            Assert.AreEqual( 1, e.InnerExceptions.Count );
            Assert.IsTrue( e.InnerExceptions[0] is ArgumentTooSmallException );
        }

        // ---------------- Helper Classes ----------------

        private class RequiredArgument
        {
            [IntegerArgument(
                requiredArgName,
                DefaultValue = defaultValue,
                Description = "A required argument",
                Required = true,
                Min = minValue,
                Max = maxValue
            )]
            public int IntProperty { get; set; }
        }

        private class OptionalArgument
        {
            [IntegerArgument(
                optionalArgName,
                DefaultValue = defaultValue,
                Description = "An optional argument",
                Required = false,
                Min = minValue,
                Max = maxValue
            )]
            public int IntProperty { get; set; }
        }

        private class EmptyArgument
        {
            [IntegerArgument(
                "",
                DefaultValue = defaultValue,
                Description = "This shouldnt work.",
                Required = false,
                Min = minValue,
                Max = maxValue
            )]
            public int IntProperty { get; set; }
        }
    }
}
