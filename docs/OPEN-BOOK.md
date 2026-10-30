# Safe Exam Browser — Open-Book build (Windows)

This is an internal fork of [Safe Exam Browser for Windows](https://github.com/SafeExamBrowser/seb-win-refactoring)
for **open-book** exams. In an open-book exam, students may use any software they like
(ChatGPT, Claude, Google, their own notes). The point of this build is simple:

> **Nothing leaves the student's PC and nothing is visible to the examiner.**
> No screen proctoring, no remote proctoring, no SEB Server reporting. Logs stay local.

It is the Windows counterpart of the macOS open-book fork and follows the same intent.

> [!WARNING]
> This is **not** the official Safe Exam Browser. Do not use it for closed-book or
> proctored exams. For anything official, always use the upstream release from
> <https://github.com/SafeExamBrowser/seb-win-refactoring>.

## Editing boundaries

This is a real exam tool, not a cheating tool. Three boundaries hold and must not be crossed:

1. **No exam-evasion log masking.** Local logs may omit the student's private activity
   (this is the open-book privacy design), but do **not** add masking whose purpose is to
   defeat a legitimate examiner's record, and add no line that hides proctorable activity.
2. **No AI-answer tooling.** Never add anything that feeds exam content to an AI or surfaces
   answers.
3. **No overriding the real exam config.** If a `.seb` configuration forbids app switching,
   unlocking must still respect it. Don't neutralize genuine exam-config restrictions on the
   student. (Removing examiner *surveillance* — proctoring/server — is the product's purpose
   and is a separate thing from a restriction on the student.)

## What the fork changes

The transformation mirrors the macOS fork and is being ported in stages.

| Area | Intent | Status |
| --- | --- | --- |
| **Screen / remote proctoring** | Can never run; no screenshots or video ever captured or transmitted. | **Done** — forced off for every configuration (see `Runtime/Operations/Session/ConfigurationOperation.cs`). |
| **SEB Server** | No connection to an LMS/SEB Server; no logs, pings, or screenshots sent off-device. | **Done** — any server session is forced to a normal local session right after config load (`ConfigurationOperation`), so the server operations never connect. |
| **"Raise hand" / proctoring notifications** | Never shown. | **Done** — Raise Hand is only added for a server session, which can no longer occur. |
| **App switching (open-book core)** | The student may use any other application. Controlled locally by an exam lock; genuine `.seb` restrictions are still respected. | **Done** — the exam-default kiosk mode is `None` (no separate desktop, explorer shell left intact, application monitor off), so the student can launch and switch to any software; the lock decides access through the keyboard interceptor (`Monitoring/Keyboard/KeyboardInterceptor.cs` + `ExamLockState`). A stricter `.seb` kiosk/keyboard restriction is still respected (boundary 3). |
| **Exam lock toggle** | A local lock/unlock shortcut; starts locked by default. | **Done** — `Ctrl+Shift+L` toggles the lock (`UserInterface.Shared/Activators/ExamLockActivator.cs`), wired through `Client/CompositionRoot.cs`, `ShellOperation` and `ShellResponsibility`. Starts locked; on lock, SEB is pulled back to the foreground so re-locking can't strand the student in another app. Windows counterpart of the macOS `Cmd+Shift+L`, and flashes a brief on-screen 🔒/🔓 indicator (`UserInterface.Shared/ExamLockIndicator.cs`). |
| **Exam start page** | One field that accepts any address, `seb(s)://` link, or `.seb` file. | **Done** — a local start page (`Browser/Content/ExamStart.html`, served by `Browser/Handlers/ResourceHandler.cs`) with one field; submitting navigates the browser, which loads a web address or hands a `seb(s)://` / `.seb` link to the existing reconfiguration pipeline. The main window also gets an address bar + back/forward. A loaded `.seb` with a real start URL is used directly. |
| **Intrusive startup blockers** | Drop start-up steps that change the student's system or demand permissions beyond what an exam needs. | Planned. |
| **Logging** | Local-only; the student's running-phase activity (page URLs, clipboard, downloads, app/window switching) is not recorded, and nothing is uploaded. | **Done** — `ExamLogGate` + `ExamLogFileWriter` suppress the running phase in the client log; start-up and shutdown lines are kept, layout and wording unchanged (boundary 1). |

### Lock / unlock (`Ctrl+Shift+L`)

The exam **starts locked**. While locked, SEB is a strict kiosk: Alt+Tab, Alt+Esc, the Windows
key, Ctrl+Esc, the menu key and similar app-switching keys are blocked, so the student stays in
the exam. Pressing **`Ctrl+Shift+L`** unlocks it — those keys fall back to the exam configuration
(open-book default = allowed), so the student can switch to and launch any other software: ChatGPT,
notes, a second browser, and so on. Pressing `Ctrl+Shift+L` again re-locks and pulls SEB back to
the front. Each toggle briefly flashes a 🔒 **Locked** (red) or 🔓 **Unlocked** (green) overlay in the
centre of the screen so the student can see the current state; it fades after about a second and
never steals focus. There is no password; it is a single toggle, the Windows counterpart of the
macOS fork's `Cmd+Shift+L`. The toggle is not written to the local log during the exam (running-phase masking,
boundary 1). Students are told the shortcut out-of-band; it is deliberately not shown on the start
page.

> If a genuine `.seb` configuration disables a key or forces a stricter kiosk mode, unlocking does
> **not** re-enable it (boundary 3). For normal open-book use, run the default configuration or a
> `.seb` that does not force `CreateNewDesktop`.

### Exam start page

With no exam configuration loaded, the main window opens on a local **start page** with a single
field: type any web address, a `seb://` / `sebs://` link, or a link to a `.seb` file and press
**Start exam**. A plain web address loads as a page; a `seb(s)://` or `.seb` link is handed to SEB's
normal reconfiguration pipeline. The main window also carries an address bar (with back / forward),
so the student can navigate freely at any point — this is open-book. If a loaded `.seb` configuration
sets a real start URL, that URL is loaded directly and the start page is skipped.

The page is served locally by the browser with no network round-trip (the browser's
`ResourceHandler` answers the internal address `https://seb.openbook/start` from the embedded
`Browser/Content/ExamStart.html`). `.seb` files on disk are opened the usual way (double-click / file
association), not through a file picker on the page.

See the commit history on `master` for the exact, reviewable diff against upstream
(`git diff upstream/master`).

## Building

Safe Exam Browser for Windows builds **only on Windows** (MSBuild / Visual Studio 2022,
.NET Framework 4.8, WiX for the installer). This fork is developed on macOS, so builds run
in the cloud:

- **GitHub Actions** (`.github/workflows/openbook-build.yml`) builds on every push and
  publishes the `Setup.msi` as a build artifact. No account setup required.
- The upstream **AppVeyor** configs (`appveyor-*.yml`) still work if you connect this
  repository at <https://ci.appveyor.com>.

Locally on Windows:

```
nuget restore SafeExamBrowser.sln
msbuild SafeExamBrowser.sln /property:langversion=latest /p:Configuration=Release /p:Platform=x64
```

The installer is produced at `Setup/bin/x64/Release/Setup.msi`.
