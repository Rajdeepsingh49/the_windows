/*
 * Copyright (c) 2026 ETH Zürich, IT Services
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using SafeExamBrowser.UserInterface.Contracts.Windows;

namespace SafeExamBrowser.UserInterface.Shared
{
	/// <summary>
	/// Open-book build: a small, borderless overlay that flashes a lock / unlock indicator for about a
	/// second when the student presses Ctrl+Shift+L (see docs/OPEN-BOOK.md). It is built entirely in
	/// code (no XAML) and shows without activating, so it never steals focus from whatever the student
	/// is doing while unlocked. All members marshal onto the dispatcher, so it is safe to call from the
	/// keyboard-hook thread.
	/// </summary>
	public class ExamLockIndicator : Window, IExamLockIndicator
	{
		private readonly Border card;
		private readonly TextBlock label;
		private readonly DispatcherTimer timer;

		public ExamLockIndicator()
		{
			WindowStyle = WindowStyle.None;
			AllowsTransparency = true;
			Background = Brushes.Transparent;
			ShowActivated = false;
			ShowInTaskbar = false;
			Topmost = true;
			ResizeMode = ResizeMode.NoResize;
			SizeToContent = SizeToContent.WidthAndHeight;
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
			IsHitTestVisible = false;

			label = new TextBlock
			{
				FontSize = 40,
				FontWeight = FontWeights.SemiBold,
				Foreground = Brushes.White,
				TextAlignment = TextAlignment.Center
			};

			card = new Border
			{
				CornerRadius = new CornerRadius(16),
				Padding = new Thickness(40, 26, 40, 26),
				Child = label
			};

			Content = card;

			timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.1) };
			timer.Tick += OnTimerTick;
		}

		public void Flash(bool locked)
		{
			Dispatcher.Invoke(() =>
			{
				card.Background = new SolidColorBrush(locked
					? Color.FromArgb(0xF0, 0x9A, 0x1F, 0x1F)
					: Color.FromArgb(0xF0, 0x1C, 0x6B, 0x2E));
				label.Text = locked ? "🔒  Locked" : "🔓  Unlocked";

				timer.Stop();
				Show();
				timer.Start();
			});
		}

		private void OnTimerTick(object sender, EventArgs e)
		{
			timer.Stop();
			Hide();
		}
	}
}
