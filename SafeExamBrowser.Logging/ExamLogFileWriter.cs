/*
 * Copyright (c) 2026 ETH Zürich, IT Services
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System.IO;
using System.Text;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Logging
{
	/// <summary>
	/// <see cref="ILogObserver"/> for the open-book build. Writes log content to disk exactly like
	/// <see cref="LogFileWriter"/>, with identical layout and wording, except that content logged
	/// during the exam's running phase is dropped according to the shared <see cref="ExamLogGate"/>.
	/// It only decides which of the original lines are written; it never rewrites them.
	/// </summary>
	public class ExamLogFileWriter : ILogObserver
	{
		private readonly object @lock = new object();
		private readonly string filePath;
		private readonly ILogContentFormatter formatter;
		private readonly ExamLogGate gate;

		public ExamLogFileWriter(ILogContentFormatter formatter, string filePath, ExamLogGate gate)
		{
			this.filePath = filePath;
			this.formatter = formatter;
			this.gate = gate;
		}

		public void Initialize()
		{
			var directory = Path.GetDirectoryName(filePath);

			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}
		}

		public void Notify(ILogContent content)
		{
			if (gate != null && !gate.ShouldWrite(content))
			{
				return;
			}

			lock (@lock)
			{
				var raw = formatter.Format(content);

				using (var stream = new StreamWriter(filePath, true, Encoding.UTF8))
				{
					stream.WriteLine(raw);
				}
			}
		}
	}
}
