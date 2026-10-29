/*
 * Copyright (c) 2026 ETH Zürich, IT Services
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace SafeExamBrowser.Logging
{
	/// <summary>
	/// Open-book build: the phase of the exam, used to decide which of the original log lines reach
	/// the local log file (see <see cref="ExamLogGate"/>).
	/// </summary>
	public enum LogPhase
	{
		/// <summary>
		/// Application start-up (configuration, session initialization). All lines are written.
		/// </summary>
		Startup,

		/// <summary>
		/// The exam is running. Nothing is written to the local log, so the student's activity
		/// (page addresses, clipboard, downloads, application and window switching) is not recorded.
		/// </summary>
		Running,

		/// <summary>
		/// Application shutdown / ending. All lines are written again.
		/// </summary>
		Ending
	}
}
