# Convert to Dictionary

Sometimes it is more appropriate to handle data from a JSON document as a dictionary. For instance the [merging functionality](./merge) makes use of dictionary objects.

Converting a [`JsonDocument`](https://docs.microsoft.com/dotnet/api/system.text.json.jsondocument) or [`JsonElement`](https://docs.microsoft.com/dotnet/api/system.text.json.jsonelement) object into a dictionary is implemented in the [`ExtensionMethods`](https://github.com/Denomica/Denomica.Text.Json/blob/main/Denomica.Text.Json/ExtensionMethods.cs) class, in the `ToDictionary` methods.

The method requires that the JSON element being converted represents an object, as shown below.

``` C#
JsonElement.ValueKind == JsonValueKind.Object
```

The method then enumerates the object's properties adding each property as an item in the dictionary. The value of each property is handled differently depending on the type of value. The method supports all different types defined in the [`JsonValueKind`](https://docs.microsoft.com/dotnet/api/system.text.json.jsonvaluekind) enumeration.

- `Array`: The value is added as an array to the resulting dictionary using the [`ValueList`](https://github.com/Denomica/Denomica.Text.Json/blob/main/Denomica.Text.Json/ValueList.cs) class.
- `False`/`True`: The value is added as a boolean value.
- `Null`/`Undefined`: The value is added as `null`.
- `Number`: The value is added as a number. The method uses three different numeric types when attempting to parse a number. Parsing is attempted in the following order:
  1. [`int`](https://docs.microsoft.com/dotnet/api/system.int32)
  2. [`double`](https://docs.microsoft.com/dotnet/api/system.double)
  3. [`decimal`](https://docs.microsoft.com/dotnet/api/system.decimal)
- `Object`: The value is converted into another [`ValueDictionary`](https://github.com/Denomica/Denomica.Text.Json/blob/main/Denomica.Text.Json/ValueDictionary.cs) instance and added to the resulting dictionary.
- `String`: The value is added as a string.

