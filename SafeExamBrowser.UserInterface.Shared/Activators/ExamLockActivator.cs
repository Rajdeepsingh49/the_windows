/*
 * Copyright (c) 2026 ETH Zürich, IT Services
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System.Threading.Tasks;
using System.Windows.Input;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Shell;
using SafeExamBrowser.UserInterface.Contracts.Shell.Events;
using SafeExamBrowser.WindowsApi.Contracts;
using SafeExamBrowser.WindowsApi.Contracts.Events;

namespace SafeExamBrowser.UserInterface.Shared.Activators
{
	/// <summary>
	/// Open-book build: detects the Ctrl+Shift+L shortcut used to lock / unlock the exam, i.e. to toggle
	/// whether the student may switch to other applications (see docs/OPEN-BOOK.md). It is the Windows
	/// counterpart of the macOS fork's Cmd+Shift+L toggle. Shift and the letter key are not reported as
	/// <see cref="KeyModifier"/> flags, so they are tracked by their own key-down / key-up transitions
	/// (the same approach <see cref="TerminationActivator"/> uses for Ctrl+Q).
	/// </summary>
	public class ExamLockActivator : KeyboardActivator, IExamLockActivator
	{
		private readonly ILogger logger;

		private bool L, LeftCtrl, RightCtrl, LeftShift, RightShift;

		public event ActivatorEventHandler Activated;

		public ExamLockActivator(ILogger logger, INativeMethods nativeMethods) : base(nativeMethods)
		{
			this.logger = logger;
		}

		protected override void OnBeforeResume()
		{
			L = false;
			LeftCtrl = false;
			RightCtrl = false;
			LeftShift = false;
			RightShift = false;
		}

		protected override bool Process(Key key, KeyModifier modifier, KeyState state)
		{
			var changed = false;
			var pressed = state == KeyState.Pressed;

			switch (key)
			{
				case Key.L:
					changed = L != pressed;
					L = pressed;
					break;
				case Key.LeftCtrl:
					changed = LeftCtrl != pressed;
					LeftCtrl = pressed;
					break;
				case Key.RightCtrl:
					changed = RightCtrl != pressed;
					RightCtrl = pressed;
					break;
				case Key.LeftShift:
					changed = LeftShift != pressed;
					LeftShift = pressed;
					break;
				case Key.RightShift:
					changed = RightShift != pressed;
					RightShift = pressed;
					break;
			}

			if (L && (LeftCtrl || RightCtrl) && (LeftShift || RightShift) && changed && !modifier.HasFlag(KeyModifier.Alt))
			{
				Task.Run(() =>
				{
					logger.Debug("Detected exam lock toggle sequence.");
					Activated?.Invoke();
				});

				return true;
			}

			return false;
		}
	}
}
