//
// Copyright Seth Hendrick 2019-2022.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using Cake.Core;
using Moq;
using NUnit.Framework;

namespace Cake.ArgumentBinder.Tests.UnitTests
{
    [TestFixture]
    public sealed class EnumArgumentAttributeTests
    {
        // ---------------- Fields ----------------

        private const string requiredArgName = "required_enum_arg";

        private const string optionalArgName = "optional_enum_arg";

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
            const Enum1 expectedValue = Enum1.Value2;

            this.cakeArgs.Setup(
                m => m.HasArgument( requiredArgName )
            ).Returns( true );

            this.cakeArgs.SetupGetArgumentSingle(
                requiredArgName,
                expectedValue.ToString()
            );

            RequiredArgument uut = ArgumentBinderAliases.CreateFromArguments<RequiredArgument>( this.cakeContext.Object );
            Assert.AreEqual( expectedValue, uut.Enum1Property );
        }

        [Test]
        public void HasRequiredArgumentIgnoreCaseTest()
        {
            const Enum1 expectedValue = Enum1.Value2;

            this.cakeArgs.Setup(
                m => m.HasArgument( requiredArgName )
            ).Returns( true );

            this.cakeArgs.SetupGetArgumentSingle(
                requiredArgName,
                expectedValue.ToString().ToLowerInvariant()
            );

            RequiredArgumentIgnoreCase uut = ArgumentBinderAliases.CreateFromArguments<RequiredArgumentIgnoreCase>( this.cakeContext.Object );
            Assert.AreEqual( expectedValue, uut.Enum1Property );
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
            const Enum2 expectedValue = Enum2.Value5;

            this.cakeArgs.Setup(
                m => m.HasArgument( optionalArgName )
            ).Returns( true );

            this.cakeArgs.SetupGetArgumentSingle(
                optionalArgName,
                expectedValue.ToString()
            );

            OptionalArgument2 uut = ArgumentBinderAliases.CreateFromArguments<OptionalArgument2>( this.cakeContext.Object );
            Assert.AreEqual( expectedValue, uut.Enum2Property );
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

            OptionalArgument2 uut = ArgumentBinderAliases.CreateFromArguments<OptionalArgument2>( this.cakeContext.Object );

            // Should be the enum set to 0
            Assert.AreEqual( Enum2.Value4, uut.Enum2Property );
        }

        /// <summary>
        /// Ensures that if we do NOT specify an optional argument,
        /// the default value gets used.
        /// </summary>
        [Test]
        public void UnspecifiedOptionalArgumentTest2()
        {
            this.cakeArgs.Setup(
                m => m.HasArgument( optionalArgName )
            ).Returns( false );

            OptionalArgument3 uut = ArgumentBinderAliases.CreateFromArguments<OptionalArgument3>( this.cakeContext.Object );

            // Should be the enum set to 0.
            Assert.AreEqual( Enum3.Value8, uut.Enum3Property );
        }

        [Test]
        public void InvalidStringTest()
        {
            this.cakeArgs.Setup(
                m => m.HasArgument( requiredArgName )
            ).Returns( true );

            this.cakeArgs.SetupGetArgumentSingle(
                requiredArgName,
                "not_an_enum"
            );

            AggregateException e = Assert.Throws<AggregateException>(
                () => ArgumentBinderAliases.CreateFromArguments<RequiredArgument>( this.cakeContext.Object )
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

        [Test]
        public void WrongEnumTest()
        {
            AggregateException e = Assert.Throws<AggregateException>(
               () => ArgumentBinderAliases.CreateFromArguments<MismatchedEnumTypeArgument>( this.cakeContext.Object )
           );

            Assert.AreEqual( 1, e.InnerExceptions.Count );
            Assert.IsTrue( e.InnerExceptions[0] is InvalidPropertyTypeForAttributeException );
        }

        [Test]
        public void EmptyEnumTest()
        {
            AggregateException e = Assert.Throws<AggregateException>(
               () => ArgumentBinderAliases.CreateFromArguments<EmptyEnumTypeArgument>( this.cakeContext.Object )
           );

            Assert.AreEqual( 1, e.InnerExceptions.Count );
            Assert.IsTrue( e.InnerExceptions[0] is AttributeValidationException );
        }

        [Test]
        public void WrongPassedInTypeTest()
        {
            AggregateException e = Assert.Throws<AggregateException>(
               () => ArgumentBinderAliases.CreateFromArguments<WrongPassedInTypeArgument>( this.cakeContext.Object )
           );

            Assert.AreEqual( 1, e.InnerExceptions.Count );
            Assert.IsTrue( e.InnerExceptions[0] is ArgumentException );
        }

        [Test]
        public void RequiredEnumHasNoDefaultValueTest()
        {
            const NoDefaultValueEnum expectedValue = NoDefaultValueEnum.Value9;

            this.cakeArgs.Setup(
                m => m.HasArgument( requiredArgName )
            ).Returns( true );

            this.cakeArgs.SetupGetArgumentSingle(
                requiredArgName,
                expectedValue.ToString()
            );

            NoDefaultValueRequiredArgument uut = ArgumentBinderAliases.CreateFromArguments<NoDefaultValueRequiredArgument>( this.cakeContext.Object );
            Assert.AreEqual( expectedValue, uut.NoDefaultProperty );
        }

        [Test]
        public void OptionalEnumHasNoDefaultValueTest()
        {
            AggregateException e = Assert.Throws<AggregateException>(
                () => ArgumentBinderAliases.CreateFromArguments<NoDefaultValueOptionalArgument>( this.cakeContext.Object )
            );

            Assert.AreEqual( 1, e.InnerExceptions.Count );
            Assert.IsTrue( e.InnerExceptions[0] is AttributeValidationException );
        }

        [Test]
        public void CasingConflictTest()
        {
            AggregateException e = Assert.Throws<AggregateException>(
                () => ArgumentBinderAliases.CreateFromArguments<CasingConflictsRequiredArgument>( this.cakeContext.Object )
            );

            Assert.AreEqual( 1, e.InnerExceptions.Count );
            Assert.IsTrue( e.InnerExceptions[0] is AttributeValidationException );
        }

        // ---------------- Helper Classes ----------------

        private class RequiredArgument
        {
            [EnumArgument(
                typeof( Enum1 ),
                requiredArgName,
                Description = "A required Enum",
                HasSecretValue = false,
                Required = true
            )]
            public Enum1 Enum1Property { get; set; }
        }

        private class RequiredArgumentIgnoreCase
        {
            [EnumArgument(
                typeof( Enum1 ),
                requiredArgName,
                Description = "A required Enum that ignores casing",
                HasSecretValue = false,
                Required = true,
                IgnoreCase = true
            )]
            public Enum1 Enum1Property { get; set; }
        }

        private class OptionalArgument2
        {
            [EnumArgument(
                typeof( Enum2 ),
                optionalArgName,
                Description = "An optional Enum",
                HasSecretValue = false,
                Required = false
            )]
            public Enum2 Enum2Property { get; set; }
        }

        private class OptionalArgument3
        {
            [EnumArgument(
                typeof( Enum3 ),
                optionalArgName,
                Description = "An optional Enum",
                HasSecretValue = false,
                Required = false
            )]
            public Enum3 Enum3Property { get; set; }
        }

        private class EmptyArgument
        {
            [EnumArgument(
                typeof( Enum1 ),
                "",
                Description = "Should not work",
                Required = false,
                HasSecretValue = false
            )]
            public Enum1 Enum1Property { get; set; }
        }

        private class MismatchedTypeArgument
        {
            [EnumArgument(
                typeof(Enum1),
                requiredArgName
            )]
            public int IntegeryProperty { get; set; }
        }

        private class MismatchedEnumTypeArgument
        {
            [EnumArgument(
                typeof( Enum1 ),
                requiredArgName
            )]
            public Enum2 Enum2Property { get; set; }
        }

        private class EmptyEnumTypeArgument
        {
            [EnumArgument(
                typeof( EmptyEnum ),
                requiredArgName
            )]
            public EmptyEnum EmptyEnumProperty { get; set; }
        }

        private class WrongPassedInTypeArgument
        {
            [EnumArgument(
                typeof( int ),
                requiredArgName
            )]
            public Enum1 Enum1Property { get; set; }
        }

        private class NoDefaultValueRequiredArgument
        {
            [EnumArgument(
                typeof( NoDefaultValueEnum ),
                requiredArgName,
                Required = true
            )]
            public NoDefaultValueEnum NoDefaultProperty { get; set; }
        }

        private class NoDefaultValueOptionalArgument
        {
            [EnumArgument(
                typeof( NoDefaultValueEnum ),
                optionalArgName,
                Required = false
            )]
            public NoDefaultValueEnum NoDefaultProperty { get; set; }
        }

        private class CasingConflictsRequiredArgument
        {
            [EnumArgument(
                typeof( CasingConflictsEnum ),
                requiredArgName,
                Required = true,
                IgnoreCase = true
            )]
            public CasingConflictsEnum CaseConflictsEnum { get; set; }
        }

        // ---------------- Helper Enums ----------------

        private enum Enum1
        {
            Value1,

            Value2
        }

        private enum Enum2
        {
            Value3 = -2,

            Value4 = 0,

            Value5 = 2
        }

        private enum Enum3
        {
            Value6 = 2,

            Value7 = -1,

            Value8 = 0
        }

        private enum EmptyEnum
        {
        }

        private enum NoDefaultValueEnum
        {
            Value9 = -1,

            Value10 = 1
        }

        private enum CasingConflictsEnum
        {
            VaLuE11,
            value11
        }
    }
}
