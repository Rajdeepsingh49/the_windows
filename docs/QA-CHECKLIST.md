# Windows QA checklist — SEB Open-Book

This is the A-to-Z verification plan for the open-book fork. It has two layers:

- **Automated (runs on Windows CI, no human needed):** compile of the whole codebase + the
  full MSTest unit-test suite, on every push. See `.github/workflows/openbook-build.yml`.
- **Manual (needs a real Windows session):** install, launch, shortcuts, kiosk/lock behaviour.
  SEB is a Win32 kiosk app; this layer **cannot** be done on macOS or in a headless CI runner —
  it needs a Windows 10/11 machine (a lab PC, a spare laptop, or a cloud Windows VM). This is the
  layer a tester (or a friend with a Windows PC) runs; start with the **Quick smoke test** below.

Legend: ☐ = to verify · **[NOW]** = applies to the current build. Every open-book feature has now
landed, so all items are [NOW].

---

## 0. Get a build

- **Easiest (any tester):** download the installer from the rolling release —
  <https://github.com/Rajdeepsingh49/the_windows/releases/download/openbook-latest-x64/Setup.msi>
  (~187 MB, 64-bit, rebuilt on every push). The repo is public, so no login is needed.
- **From CI artifacts:** repo **Actions** tab → latest green run → **Artifacts** →
  `SEB-OpenBook-x64-Setup` (the `Setup.msi`) and `test-results-x64` (the `.trx` report).
- **Locally on Windows:** `nuget restore SafeExamBrowser.sln` then
  `msbuild SafeExamBrowser.sln /p:Configuration=Release /p:Platform=x64`; installer at
  `Setup/bin/x64/Release/Setup.msi`.

Prerequisites on the test machine: .NET Framework 4.8 runtime (ships with Windows 10 1903+ and
Windows 11) and the Visual C++ 2015–2022 x64 redistributable. The MSI is **unsigned**, so Windows
SmartScreen will warn on first launch — choose **More info → Run anyway**, then accept the UAC prompt.

## 1. Quick smoke test (first run / hand to a tester)

A five-minute pass that exercises every open-book feature. The lock shortcut is **`Ctrl+Shift+L`**
(tell the tester out-of-band — it is deliberately not shown on screen).

- ☐ **Installs & launches** on the Windows PC/VM (proves the VM + remote-session gates are relaxed —
  if you see "virtual machine not allowed" or "remote session not allowed", report it).
- ☐ **Start page:** SEB opens on a page with a single box. Type `example.com`, press **Start exam** /
  Enter — the page loads. The address bar at the top also works.
- ☐ **Starts locked:** before touching the shortcut, Alt+Tab and the ⊞ Windows key do nothing (you
  cannot leave SEB).
- ☐ **Unlock:** press **`Ctrl+Shift+L`** → a green 🔓 **Unlocked** badge flashes centre-screen. Now
  Alt+Tab and ⊞ Win work; open Notepad or another browser — SEB does **not** close or kill it.
- ☐ **Clipboard across apps:** copy text from the exam page, paste it into Notepad / ChatGPT — it
  pastes (clipboard is unrestricted). And the reverse (copy from notes → paste into exam).
- ☐ **Screenshot:** take a screenshot of the exam window (PrtSc / Snipping Tool) — it captures the
  real content, not a black rectangle.
- ☐ **Re-lock:** press **`Ctrl+Shift+L`** again → a red 🔒 **Locked** badge flashes, SEB jumps back to
  the front, and app switching is blocked again (you are not stranded in the other app).
- ☐ **Quit:** `Ctrl+Q` (no quit password set by default).

If all of the above pass, the build is functionally good; the sections below are the thorough pass.

## 2. Automated test layer (confirm first)

- ☐ CI run is green for both `x64` and `x86`.
- ☐ `test-results-*.trx` shows **0 failed**; confirm `Perform_MustDisableProctoringForOpenBook`
  (Runtime.UnitTests) is present and passing.
- ☐ `Setup.msi` artifact exists / the rolling release updated.

## 3. Install & launch

- ☐ `Setup.msi` installs without error on Windows 10 (1903+) and Windows 11.
- ☐ SEB launches; splash screen then the exam shell/taskbar appears.
- ☐ SEB launches by opening a `.seb` config file.
- ☐ SEB launches from a `seb://` / `sebs://` link.

## 4. Privacy guarantees (the whole point of this fork)

- ☐ Load a `.seb` config that **requests screen proctoring**. Expected: proctoring **never starts** —
  no proctoring taskbar indicator, no "proctoring" notification, no screenshots cached or sent. The
  runtime log line "Open-book build: remote and screen proctoring are disabled" appears.
- ☐ Load a config that references a **SEB Server**. Expected: no connection attempt; no
  logs/pings/screenshots leave the machine; the session runs as a normal local session, and the
  runtime log shows "proctoring and SEB Server are disabled".
- ☐ Logs are written only locally (under the SEB app data / log folder); nothing is uploaded.
- ☐ During an exam (running phase), the client log records no browsing URLs, clipboard, downloads,
  or app/window switching; start-up and shutdown entries are still present (run an exam, visit a few
  pages, switch apps, copy/paste, quit, then inspect the client log). App/window switching is never
  logged at all — the application monitor does not run under the open-book default `KioskMode.None`.
- ☐ The Chromium **engine** log is disabled: in the SEB log folder there is no CEF/`browser` engine
  log containing visited or failed URLs (the file is absent or empty). This is the second on-device
  log the running-phase mask cannot reach, so it is turned off at the source.
- ☐ Capture network traffic (e.g. Fiddler/Wireshark) during an exam: no traffic to any
  proctoring/SEB-Server endpoint; only the exam/browser URLs the student visits.

## 5. Browser & standard shortcuts (stock SEB behaviour — must still work)

Verify against the **active `.seb` config's** keyboard/mouse settings (some keys are intentionally
disabled by a config; that is expected and must be respected):

- ☐ Page loads; links navigate; downloads/uploads behave per config.
- ☐ Reload (F5), zoom in/out (Ctrl + `+` / `-`), zoom reset.
- ☐ Back / forward (enabled by default in the open-book build).
- ☐ **Clipboard** copy/cut/paste/select-all work, **including to and from other applications**
  (open-book default `ClipboardPolicy.Allow`; a stricter `.seb` policy is still respected).
- ☐ **Screen capture:** a screenshot / screen recording of the exam window shows real content, not a
  black frame (open-book default `AllowWindowCapture = true`; a `.seb` with `allowScreenSharing=false`
  re-blanks it).
- ☐ Action Center / taskbar / task-view activators open as configured.
- ☐ Reconfiguration via an allowed resource works (if the config allows it).

## 6. Open-book core — app switching & exam lock

- ☐ SEB starts **locked** (strict kiosk) by default.
- ☐ **`Ctrl+Shift+L`** toggles the lock, with a visible on-screen 🔒/🔓 indicator that fades after
  ~1 s and does not steal focus.
- ☐ When **unlocked**, the student can switch to any other app (ChatGPT, Claude, notes, Google) and
  back, and can launch new apps; SEB does not terminate them.
- ☐ When **locked**, app/window switching is blocked (Alt+Tab, Alt+Esc, ⊞ Win, Ctrl+Esc, menu key).
- ☐ On re-lock, SEB returns to the foreground (the student is not stranded in another app).
- ☐ If the `.seb` config genuinely forbids a key or forces a stricter kiosk mode, unlocking still
  respects that restriction (boundary 3).
- ☐ The exam start page accepts any web address, a `seb(s)://` link, or a link to a `.seb` file.
- ☐ A loaded `.seb` with a real start URL loads that URL directly (the start page is skipped).

## 7. Quit / termination

- ☐ Quit via the configured method (button / quit password / quit URL) works.
- ☐ On quit, the desktop/explorer shell is restored cleanly; no leftover kiosk desktop.
- ☐ A web page cannot trap or block quitting.

---

### Reporting results

For any failure, capture: the step number, the exact `.seb` config used (if any), the SEB log file,
and a screenshot. Attach to the issue so it can be fixed and the relevant unit test added.
