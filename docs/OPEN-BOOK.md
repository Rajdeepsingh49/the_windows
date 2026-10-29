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
| **App switching (open-book core)** | The student may use any other application. Controlled locally by an exam lock; genuine `.seb` restrictions are still respected. | Planned. |
| **Exam lock toggle + indicator** | A local lock/unlock shortcut with an on-screen indicator; starts locked by default. | Planned. |
| **Exam start page** | One field that accepts any address, `seb(s)://` link, or `.seb` file. | Planned. |
| **Intrusive startup blockers** | Drop start-up steps that change the student's system or demand permissions beyond what an exam needs. | Planned. |
| **Logging** | Stays entirely local; nothing is uploaded. | Follows from removing SEB Server. |

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
