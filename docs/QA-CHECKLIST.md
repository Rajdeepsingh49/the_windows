# Windows QA checklist — SEB Open-Book

This is the A-to-Z verification plan for the open-book fork. It has two layers:

- **Automated (runs on Windows CI, no human needed):** compile of the whole codebase + the
  full MSTest unit-test suite, on every push. See `.github/workflows/openbook-build.yml`.
- **Manual (needs a real Windows session):** install, launch, shortcuts, kiosk/lock behaviour.
  SEB is a Win32 kiosk app; this layer **cannot** be done on macOS or in a headless CI runner —
  it needs a Windows 10/11 machine (a lab PC, a spare laptop, or a cloud Windows VM).

Legend: ☐ = to verify · **[NOW]** = applies to the current build · **[PENDING]** = lands with a
later port stage (don't test yet).

---

## 0. Get a build

- **From CI:** open the repo's **Actions** tab → latest green run → **Artifacts** →
  `SEB-OpenBook-x64-Setup` (the `Setup.msi`) and `test-results-x64` (the test report `.trx`).
- **Locally on Windows:** `nuget restore SafeExamBrowser.sln` then
  `msbuild SafeExamBrowser.sln /p:Configuration=Release /p:Platform=x64`; installer at
  `Setup/bin/x64/Release/Setup.msi`.

Prerequisites on the test machine: .NET Framework 4.8 runtime, Visual C++ 2015–2022 redistributable
(the SetupBundle installs these automatically).

## 1. Automated test layer (confirm first)

- ☐ **[NOW]** CI run is green for both `x64` and `x86`.
- ☐ **[NOW]** `test-results-*.trx` shows **0 failed**; confirm `Perform_MustDisableProctoringForOpenBook`
  (Runtime.UnitTests) is present and passing.
- ☐ **[NOW]** `Setup.msi` artifact exists.

## 2. Install & launch

- ☐ **[NOW]** `Setup.msi` installs without error on Windows 10 (1803+) and Windows 11.
- ☐ **[NOW]** SEB launches; splash screen then the exam shell/taskbar appears.
- ☐ **[NOW]** SEB launches by opening a `.seb` config file.
- ☐ **[NOW]** SEB launches from a `seb://` / `sebs://` link.

## 3. Privacy guarantees (the whole point of this fork)

- ☐ **[NOW]** Load a `.seb` config that **requests screen proctoring**. Expected: proctoring
  **never starts** — no proctoring taskbar indicator, no "proctoring" notification, no screenshots
  cached or sent. The runtime log line "Open-book build: remote and screen proctoring are disabled"
  appears.
- ☐ **[PENDING]** Load a config that references a **SEB Server**. Expected: no connection attempt;
  no logs/pings/screenshots leave the machine. *(Server neutralization is a later stage.)*
- ☐ **[NOW]** Logs are written only locally (under the SEB app data / log folder); nothing is uploaded.
- ☐ **[NOW]** Capture network traffic (e.g. Fiddler/Wireshark) during an exam: no traffic to any
  proctoring/SEB-Server endpoint; only the exam/browser URLs the student visits.

## 4. Browser & standard shortcuts (stock SEB behaviour — must still work)

Verify against the **active `.seb` config's** keyboard/mouse settings (some keys are intentionally
disabled by a config; that is expected and must be respected):

- ☐ **[NOW]** Page loads; links navigate; downloads/uploads behave per config.
- ☐ **[NOW]** Reload (F5), zoom in/out (Ctrl + `+` / `-`), zoom reset.
- ☐ **[NOW]** Back / forward (if enabled by config).
- ☐ **[NOW]** Clipboard copy/cut/paste/select-all behave per the config's clipboard policy.
- ☐ **[NOW]** Action Center / taskbar / task-view activators open as configured.
- ☐ **[NOW]** Reconfiguration via an allowed resource works (if the config allows it).

## 5. Open-book core — app switching & exam lock  **[PENDING — later stages]**

Do **not** test until the "app switching + exam lock" stage lands. Target behaviour to verify then:

- ☐ **[PENDING]** SEB starts **locked** (strict kiosk) by default.
- ☐ **[PENDING]** The lock/unlock shortcut toggles the lock, with a visible on-screen indicator.
- ☐ **[PENDING]** When **unlocked**, the student can switch to any other app (ChatGPT, Claude,
  notes, Google) and back.
- ☐ **[PENDING]** When **locked**, app/window switching is blocked (strict kiosk).
- ☐ **[PENDING]** If the `.seb` config genuinely forbids app switching, unlocking still respects
  that restriction (boundary 3).
- ☐ **[PENDING]** The exam start page accepts any address, `seb(s)://` link, or `.seb` file.

## 6. Quit / termination

- ☐ **[NOW]** Quit via the configured method (button / quit password / quit URL) works.
- ☐ **[NOW]** On quit, the desktop/explorer shell is restored cleanly; no leftover kiosk desktop.
- ☐ **[NOW]** A web page cannot trap or block quitting.

---

### Reporting results

For any failure, capture: the step number, the exact `.seb` config used, the SEB log file, and a
screenshot. Attach to the issue so it can be fixed and the relevant unit test added.
