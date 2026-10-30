/*
 * Copyright (c) 2026 ETH Zürich, IT Services
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace SafeExamBrowser.UserInterface.Contracts.Windows
{
	/// <summary>
	/// Open-book build: a small overlay that briefly flashes the current exam lock state when the
	/// student toggles it with Ctrl+Shift+L (see docs/OPEN-BOOK.md).
	/// </summary>
	public interface IExamLockIndicator
	{
		/// <summary>
		/// Shows the indicator for the given state (locked / unlocked) and hides it again after a moment.
		/// May be called from any thread.
		/// </summary>
		void Flash(bool locked);
	}
}
