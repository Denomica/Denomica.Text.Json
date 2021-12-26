# Merging JSON Documents

One of the main drivers behind Denomica.Text.Json was to implement merging functionality for JSON documents. That seems to be an increasingly common requirement in applications processing JSON data.

Merging two JSON documents is a tree-step process.

1. [Crete dictionary](./dictionary) objects for both your source and target.
2. Merge the source and target dictionaries to a merged dictionary instance.
3. Deserialize the merged dictionary into a [`JsonDocument`](https://docs.microsoft.com/dotnet/api/system.text.json.jsondocument) object.

All of these steps are supported by extension methods defined in the [`ExtensionMethods`](https://github.com/Denomica/Denomica.Text.Json/blob/main/Denomica.Text.Json/ExtensionMethods.cs) class.

> Please note that the [`JsonDocument`](https://docs.microsoft.com/dotnet/api/system.text.json.jsondocument) implements the [`IDisposable`](https://docs.microsoft.com/dotnet/api/system.idisposable), so you must handle it accordingly.