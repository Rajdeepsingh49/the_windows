/*
 * Copyright (c) 2026 ETH Zürich, IT Services
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using SafeExamBrowser.UserInterface.Contracts.Shell.Events;

namespace SafeExamBrowser.UserInterface.Contracts.Shell
{
	/// <summary>
	/// Open-book build: a module which observes user input and indicates when the student would like to
	/// lock or unlock the exam, i.e. toggle whether switching to other applications is allowed (see
	/// docs/OPEN-BOOK.md).
	/// </summary>
	public interface IExamLockActivator : IActivator
	{
		/// <summary>
		/// Fired when the lock/unlock shortcut has been detected.
		/// </summary>
		event ActivatorEventHandler Activated;
	}
}
