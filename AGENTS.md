# AGENTS.md

This is an educational project, not a full production solution. Keep cognitive load theory
in mind and do not introduce new technologies, concepts, or libraries without checking
first. If a pattern you are about to write is not described in these instructions, stop
and ask.

## Context

Starter repository for the individual project of a fourth-year software engineering
course (the project for the lower grades, 6 and 7). One student builds the whole
application alone, inside an architecture that is already set up. It is a reduced copy
of the team project's starter: the same layering, conventions and tests, without the
team process.

The student builds every feature in the `Exploration` module, which starts empty.
`Social` holds one finished example (blogs with comments) that shows a use case through
every layer, from the aggregate to the Angular page. It is a reference to pattern-match
against, not a place for the student's features. `Identity` provides registration,
login and roles.

Students know Clean Architecture and .NET. They are weak on frontend. At the project
defense they explain their own code: the client-server interaction, the application
layer, the repositories, the domain and the tests.

This shapes every decision here: the backend is rigorous and enforced by tooling, the
frontend is deliberately plain and enforced by convention. That asymmetry is intentional,
not an oversight. Do not "improve" the frontend by adding architectural layers.

**Domain:** tours led by guides. Guides create tours with key points on a map,
tourists buy, rate them and report problems on them. The project
specification published on the course page is the source of the requirements. Roles
are `tourist` and `guide`.

**Stack:** ASP.NET Core on .NET 10 (backend), Angular (frontend), one repository, one
PostgreSQL database. No Docker in this project; students run PostgreSQL natively.

## Repository layout

```
backend/          .NET solution (Explorer.slnx), see backend/AGENTS.md
frontend/         Angular workspace, see frontend/AGENTS.md
docs/             knowledge base (course material, in Serbian), see below
.config/          dotnet tool manifest (dotnet-ef)
.claude/          shared agent settings and the psw-* skills, see below
.agents/          the same psw-* skills for Codex
.workspace/       per-student working files of the skills (specs, study log);
                  gitignored, never committed
```

Feature modules are Exploration (the student's work) and Social (the example). Module
paths are strictly parallel across the two tiers:

```
backend/Modules/<Name>/
frontend/src/app/modules/<name>/
```

## Rules that apply everywhere

- All of the student's features live in `Exploration`. Do not put them in `Social`,
  and do not add new modules. `Exploration` never reaches into `Social`, and the reverse.
- The parts marked `[platform]` in the tier instructions (host, shared code, Identity,
  build files) are set up once and stay as they are. Adding to the shared kernel or a
  new library is a decision, not a convenience; flag it and ask before doing it.
- Every feature module has an identical structure. Consistency beats local cleverness,
  because students learn by pattern-matching against the example module.
- Modules are pre-scaffolded so students never edit central registration files,
  project files, or the solution. Versions of every library are pinned centrally.

## Where to look next

The instructions are scoped by tier. `backend/AGENTS.md` and `frontend/AGENTS.md` hold
the mandatory patterns for their folder and load automatically once a session touches
files there; read the relevant one before writing code, not after the first file is
open. READMEs placed next to the code they govern hold the finer conventions. When a
change alters a mandatory pattern, updating the affected instructions file or
README is part of that change.

- Backend feature or fix: `backend/AGENTS.md`.
- Frontend feature or fix: `frontend/AGENTS.md`.
- A feature spanning both tiers: both. The seam is the backend's Application DTO, which
  is the wire contract the frontend's `api/` types mirror.
- Understanding a concept or trade-off: the knowledge base, below. Do not load the
  tier instructions for a discussion that changes no code.

## Knowledge base

`docs/knowledge-base/` holds the course material students learn from, written in
Serbian and owned by the course staff. Every document is a lesson: concept teaching
whose examples are deliberately simplified and do NOT follow this project's
conventions. Expect the knowledge base and the real code to differ; that divergence
is by design, not an error to fix. Never copy code from the knowledge base into the
project.

Consult the knowledge base only when the session is about understanding: a student
asking to learn, discuss, or examine a concept or trade-off. Start from
`docs/knowledge-base/INDEX.md`, which lists every document with a one-line
description and its prerequisites, and load only the documents relevant to the
discussion. Do not read `docs/knowledge-base/mapa.html`; it is a human-facing view
of the same material.

For engineering tasks (writing or changing code), do not load knowledge-base
documents; the mandatory patterns live in the tier instructions and READMEs.

When you explain a concept, pattern, or trade-off that no knowledge-base lesson covers,
start that part of the answer on its own line with the exact prefix `Van baze znanja:`.
This holds in every session, with or without a skill. Project conventions from the tier
instructions and facts about the code in front of you are not flagged. If you cannot
tell whether a lesson covers it, flag it.

## Skills

`.claude/skills/` holds the `psw-*` skills, and `.agents/skills/` holds their Codex
versions. A skill runs only when a student invokes it by name. They exist to teach: a skill slows the work down on purpose and asks the student
to reason before it answers, so never shorten its phases to finish sooner. Each skill
names the lessons and tier instructions it reads, and that list overrides the two
loading rules above (no knowledge base for engineering tasks, no tier instructions for
a discussion). The skills keep their working files in `.workspace/`.

## Working style

- Prefer the smallest change that satisfies the request. This is teaching code, so
  readability outranks sophistication.
- When a change would touch a platform-owned file, the `Social` example, a `Contracts`
  project, or a `.csproj`, say so before doing it.
- When a decision is genuinely open, ask rather than guess.
- Prefer a plain class over an abstraction. If you are adding an interface, be able to say
  which dependency arrow it reverses.
