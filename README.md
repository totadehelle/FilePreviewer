# Computer Science Final Task

For caching Microsoft.Extensions.Caching.Memory is used. Logger is custom, modelled on Serilog.

Patterns used here:

Main app:
- MVVM as base pattern

Core (Models):
- Decorator for cache using - so we can use or not caching just changing implementation in DI-container

Logging:
- Builder for logger configuring, as logger should be configured once and has some number of options. Method chaining is used there for better readability
- Ambient context - for the possibility of replacing the logger with a "null logger" and not using Singleton
- Plug-in modules for free adding output modules (to console, to file, to database etc.) without changes in main logger project

DAL, Caching:
- something repository-like

