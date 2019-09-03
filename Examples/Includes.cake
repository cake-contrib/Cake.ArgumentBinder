//
// Copyright Seth Hendrick 2019.
// Distributed under the MIT License.
// (See accompanying file LICENSE in the root of the repository).
//

// This file only loads common usings and addins once (like C++ does).
// This is done so we have better intellisense when writing cake files.

#if (PACKAGE)
 // Do nothing, since its already defined.
#else

// For binding arguments.
#addin nuget:?package=Cake.ArgumentBinder

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Cake.ArgumentBinder;

#define PACKAGE
#endif
