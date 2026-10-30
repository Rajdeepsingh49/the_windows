/*
 * Copyright (c) 2026 ETH Zürich, IT Services
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace SafeExamBrowser.Monitoring.Keyboard
{
	/// <summary>
	/// Open-book build: shared, thread-safe state of the exam lock (see docs/OPEN-BOOK.md). The exam
	/// starts locked and the student toggles it with the Ctrl+Shift+L shortcut (the Windows counterpart
	/// of the macOS fork's Cmd+Shift+L). While locked, the <see cref="KeyboardInterceptor"/> blocks the
	/// keys used to switch to other applications (Alt+Tab, the Windows key, Alt+Esc, Ctrl+Esc, ...) so
	/// SEB behaves as a strict kiosk; while unlocked, those keys fall back to the configured keyboard
	/// settings so the student may freely use any other software. The flag is read from the global
	/// keyboard hook callback (a background thread), hence the locking.
	/// </summary>
	public class ExamLockState
	{
		private readonly object @lock = new object();
		private bool isLocked;

		/// <summary>
		/// Whether the exam is currently locked (strict kiosk, no switching to other applications).
		/// </summary>
		public bool IsLocked
		{
			get { lock (@lock) { return isLocked; } }
		}

		public ExamLockState(bool startLocked = true)
		{
			isLocked = startLocked;
		}

		/// <summary>
		/// Flips the lock state and returns the new value (<c>true</c> = now locked).
		/// </summary>
		public bool Toggle()
		{
			lock (@lock)
			{
				isLocked = !isLocked;

				return isLocked;
			}
		}

		/// <summary>
		/// Sets the lock state explicitly.
		/// </summary>
		public void Set(bool locked)
		{
			lock (@lock)
			{
				isLocked = locked;
			}
		}
	}
}
