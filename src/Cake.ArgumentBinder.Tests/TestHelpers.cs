//
// Copyright Seth Hendrick 2019-2021.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Cake.ArgumentBinder.Tests
{
    public static class TestHelpers
    {
        public static void EnsureLineExistsFromMultiLineString( string expectedLine, string multiLineString )
        {
            Regex regex = new Regex( $@"\s+{Regex.Escape( expectedLine )}" );

            EnsureLineExistsFromMultiLineString( regex, multiLineString );
        }

        public static void EnsureLineExistsFromMultiLineString( Regex expectedLine, string multiLineString )
        {
            int lineCount = 0;
            StringBuilder foundLines = new StringBuilder();
            using( StringReader reader = new StringReader( multiLineString ) )
            {
                string line = reader.ReadLine();
                while( line != null )
                {
                    if( expectedLine.IsMatch( line ) )
                    {
                        ++lineCount;
                        foundLines.AppendLine( "-" + line );
                    }
                    line = reader.ReadLine();
                }
            }

            if( lineCount == 0 )
            {
                Assert.Fail(
                    $"Found no matching lines for {expectedLine}.{Environment.NewLine}All lines: {Environment.NewLine}{multiLineString}"
                );
            }
            else if( lineCount != 1 )
            {
                Assert.Fail(
                    $"Found too many lines:{Environment.NewLine}{foundLines}"
                );
            }
        }
    }
}
