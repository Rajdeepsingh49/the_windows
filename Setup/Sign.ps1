#
# Copyright (c) 2026 ETH Zurich, IT Services
#
# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at http://mozilla.org/MPL/2.0/.
#

# Open-book fork: this is an internal build and is NOT code-signed. The official Safe Exam Browser
# build signs every binary and the installer with ETH Zurich's certificate, which this fork neither
# has nor should use. Signing is therefore skipped so the installer can be built in CI without a
# certificate. To produce a signed build, restore the upstream Sign.ps1 and provide a certificate.

[CmdletBinding()]
param
(
	[Parameter(Mandatory = $false)]
	[string] $Directory
)

Write-Host "Open-book build: skipping code signing for '$Directory' (unsigned internal build)."

exit 0
