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

### v1.0.1

- Fixed a serialization / deserialization issue that caused nested objects to be deserialized into a [`JsonElement`](https://docs.microsoft.com/dotnet/api/system.text.json.jsonelement) object instead of a dictionary.