# Crayon version 2.1.1
==============================================

Please go to http://crayonlang.org for documentation, tutorials, demos, and other resources.

[![Build Status](https://travis-ci.org/blakeohare/crayon.svg?branch=master)](https://travis-ci.org/blakeohare/crayon)

# Copyright
Copyright 2019 crayonlang.org, All rights reserved.

# Licenses
Crayon is released under the MIT license.
OpenTK & Tao license information can be found at http://www.opentk.com/project/license

See the LICENSE.txt file included with this release for further information.

# Reporting Bugs
Please report any issues you may find to the GitHub issue tracker located at https://github.com/blakeohare/crayon/issues
It may be helpful to check the IRC channel first to make sure any issue you find is actually a bug or for workarounds.

# Community
The official Crayon IRC channel is #crayon on FreeNode.net. Feel free to ask any questions there.
Google Mailing list/forum: https://groups.google.com/forum/#!forum/crayon-lang
Use the stackoverflow tag "crayon" for any issues. This tag is monitored.

# New in 2.1.1
The following have been added since 2.1.0

* Added a library for encoding and decoding Base64
* Added a library for accessing environment variables
* Added a library for launching processes
* Added a library for reading zip files
* Bug fixes

# Setting up and Running Crayon
If you have downloaded Crayon from crayonlang.org/download (recommended) please read the instructions on the site (linked from download
page). If you are compiling Crayon from source code directly (power users) run the release Python script in the Release
directory to create a release package for your OS, then follow the same instructions as if you downloaded it from the site.
If you are trying to run Crayon from the debugger, open `Compiler/CrayonWindows.sln` or `Compiler/CrayonOSX.sln`. Create a
file called `DEBUG_ARGS.txt` in the directory of your `%CRAYON_HOME%` environment variable. The last line of this file will be
used as the command line arguments. Note: Debug builds will not catch compiler error exceptions.
