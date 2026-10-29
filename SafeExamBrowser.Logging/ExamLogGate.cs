/*
 * Copyright (c) 2026 ETH Zürich, IT Services
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Logging
{
	/// <summary>
	/// Open-book build: shared gate that decides whether log content reaches the local log file,
	/// based on the current <see cref="LogPhase"/>. During the running phase nothing is written,
	/// mirroring the macOS open-book fork; the start-up and ending phases are logged normally.
	///
	/// This affects only the local, on-device log file. It is not a channel to any examiner: remote
	/// and screen proctoring and the SEB Server are removed from this build, so nothing is ever
	/// transmitted. It does not alter the layout or wording of the lines that are written, and it
	/// does not hide start-up, shutdown, error or tamper information - it only drops the activity
	/// that occurs while the exam is running (see docs/OPEN-BOOK.md, boundary 1).
	/// </summary>
	public class ExamLogGate
	{
		private readonly object @lock = new object();
		private LogPhase phase = LogPhase.Startup;

		/// <summary>
		/// The current exam phase. Defaults to <see cref="LogPhase.Startup"/>.
		/// </summary>
		public LogPhase Phase
		{
			get { lock (@lock) { return phase; } }
			set { lock (@lock) { phase = value; } }
		}

		/// <summary>
		/// Whether the given content should be written to the local log in the current phase.
		/// </summary>
		public bool ShouldWrite(ILogContent content)
		{
			return Phase != LogPhase.Running;
		}
	}
}
