

---

ℹ️ Structured logging has been merged into NLog 4.5 beta

---

# NLog.StructuredEvents [![Build status](https://ci.appveyor.com/api/projects/status/fs0kc13ywvyfcufe/branch/master?svg=true)](https://ci.appveyor.com/project/nlog/yamtp/branch/master) [![codecov](https://codecov.io/gh/nlog/NLog.StructuredEvents/branch/master/graph/badge.svg)](https://codecov.io/gh/nlog/NLog.StructuredEvents)

Parsing and rendering of [message templates](https://messagetemplates.org). Fully backwards-compatible with Serilog's message template format and `String.Format()`.

## Getting started

Most of the time, you'll use _NLog.StructuredEvents_ as an integrated part of NLog 4.5+. There's no additional API required for this: just pass a message template to `ILogger.Info()` and any other method that normally accepts a format string:

```csharp
logger.Info("User {Username} logged in from {IPAddress}", username, ipAddress)
  // -> An event with `Username` and `IPAddress` properties
```

To use _NLog.StructuredEvents_ as a stand-alone parser or renderer, first install the NuGet package:

```powershell
Install-Package NLog.StructuredEvents -Pre
```

### Parsing

The `TemplateParser.Parse()` method converts a message template string into a `Template` object:

```csharp
var template = TemplateParser.Parse("User {Username} logged in from {IPAddress}");
```

You can inspect the individual tokens in the template's `Literals` and `Holes` properties.

### Rendering

To render a template, the values assigned to each `Hole` must be provided positionally:

```csharp
template.Render(CultureInfo.CurrentCulture, "alice", "192.168.10.20");
  // -> User "alice" logged in from "192.168.10.20"  
```

## License

Dual licensed: MIT/BSD (choose one ;))
