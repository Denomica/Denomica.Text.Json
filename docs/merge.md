# Merging JSON Data

One of the main drivers behind Denomica.Text.Json was to implement merging functionality for JSON data. That seems to be an increasingly common requirement in applications processing JSON data.

## JSON Documents or JSON Elements

Merging two JSON documents or elements is a tree-step process.

1. [Crete dictionary](./dictionary.md) objects for both your source and target.
2. Merge the source and target dictionaries to a merged dictionary instance.
3. Deserialize the merged dictionary into a [`JsonDocument`](https://docs.microsoft.com/dotnet/api/system.text.json.jsondocument) object.

All of these steps are supported by extension methods defined in the [`ExtensionMethods`](https://github.com/Denomica/Denomica.Text.Json/blob/main/Denomica.Text.Json/ExtensionMethods.cs) class.

> Please note that the [`JsonDocument`](https://docs.microsoft.com/dotnet/api/system.text.json.jsondocument) implements the [`IDisposable`](https://docs.microsoft.com/dotnet/api/system.idisposable), so you must handle it accordingly.

If a value with the same key exists in both source and target, then the following rules apply.

1. If both values are [dictionary objects](https://github.com/Denomica/Denomica.Text.Json/blob/main/Denomica.Text.Json/ValueDictionary.cs), they will be merged as dictionary objects and added to the resulting dictionary.
2. If both values are [arrays](https://github.com/Denomica/Denomica.Text.Json/blob/main/Denomica.Text.Json/ValueList.cs), then will be merged as arrays and added to the resulting dictionary.
3. The target value will be overwritten with the value from the source.

## JSON Arrays

Merging JSON arrays is primarily used when merging JSON documents or JSON elements.

The following rules apply when merging arrays.

1. Primitive types (string, boolean, number) are added to the resulting array only if the array does not already contain that value.
2. Objects and arrays are always added to the resulting array.
