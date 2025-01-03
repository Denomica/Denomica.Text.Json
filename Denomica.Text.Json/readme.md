# Denomica.Test.Json

A .NET Standard 2.1 library that exposes extension functionality for types defined in [`System.Text.Json`](https://www.nuget.org/packages/System.Text.Json)

## Highlights

The library exposes functionality for:

- Convert JSON documents into dictionary objects
- Convert JSON arrays into lists
- Merge two JSON documents/elements/nodes
- Support for types defined in [`Newtonsoft.Json`](https://www.nuget.org/packages/Newtonsoft.Json/)

## Improvements

The main improvements in the different versions are listed below.

### v1.0.5

- Downgraded [`System.Text.Json`](https://www.nuget.org/packages/System.Text.Json/) to a lower, but still non-vulnerable version. This enables more applications to use this library when applications can more freely determine which version of `System.Text.Json` they use.

### v1.0.4

- Updated vulnerable version of [`System.Text.Json`](https://www.nuget.org/packages/System.Text.Json/).

### v1.0.3

- Added extension methods for hanlding nullable versions of [`JsonElement`](https://learn.microsoft.com/dotnet/api/system.text.json.jsonelement) structures.
- Updated [`Newtonsoft.Json`](https://www.nuget.org/packages/Newtonsoft.Json/) package to a non-vulnerable version.
- Updated [`System.Text.Json`](https://www.nuget.org/packages/System.Text.Json/) package.

### v1.0.2

- Fixed a serialization problem in cased when using the [`JsonUtil.CreateDictionary`](https://github.com/Denomica/Denomica.Text.Json/blob/main/Denomica.Text.Json/JsonUtil.cs) and [`JsonUtil.CreateList`](https://github.com/Denomica/Denomica.Text.Json/blob/main/Denomica.Text.Json/JsonUtil.cs) methods without specifying serialization options explicitly. The default in these cases should be to use the internally defined default serialization options which is now fixed in this version.

### v1.0.1

- Fixed a serialization / deserialization issue that caused nested objects to be deserialized into a [`JsonElement`](https://docs.microsoft.com/dotnet/api/system.text.json.jsonelement) object instead of a dictionary.
