/*
 * Copyright (c) 2026 ETH Zürich, IT Services
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SafeExamBrowser.Logging.UnitTests
{
	[TestClass]
	public class ExamLogFileWriterTests
	{
		private string filePath;
		private ExamLogGate gate;
		private ExamLogFileWriter sut;

		[TestInitialize]
		public void Initialize()
		{
			filePath = Path.Combine(Path.GetTempPath(), $"seb-openbook-{Guid.NewGuid()}.log");
			gate = new ExamLogGate();
			sut = new ExamLogFileWriter(new DefaultLogFormatter(), filePath, gate);
			sut.Initialize();
		}

		[TestCleanup]
		public void Cleanup()
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}

		private string ReadFile()
		{
			return File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;
		}

		[TestMethod]
		public void MustWriteDuringStartupPhase()
		{
			gate.Phase = LogPhase.Startup;

			sut.Notify(new LogText("startup-line"));

			StringAssert.Contains(ReadFile(), "startup-line");
		}

		[TestMethod]
		public void MustSuppressActivityDuringRunningPhase()
		{
			gate.Phase = LogPhase.Running;

			sut.Notify(new LogText("secret-activity-url"));

			Assert.IsFalse(ReadFile().Contains("secret-activity-url"), "Running-phase activity must not reach the local log.");
		}

		[TestMethod]
		public void MustWriteAgainDuringEndingPhase()
		{
			gate.Phase = LogPhase.Running;
			sut.Notify(new LogText("running-line"));

			gate.Phase = LogPhase.Ending;
			sut.Notify(new LogText("ending-line"));

			var content = ReadFile();

			Assert.IsFalse(content.Contains("running-line"), "The running-phase line must have been dropped.");
			StringAssert.Contains(content, "ending-line");
		}
	}
}
