using System;
using System.Collections.Generic;
using System.Text;

namespace Denomica.Text.Json
{
    /// <summary>
    /// Represents a JSON array that contains values.
    /// </summary>
    /// <remarks>
    /// Values can be primitive JSON values or arrays represented by <see cref="ValueList"/> objects or JSON objects represented by <see cref="ValueDictionary"/> objects.
    /// </remarks>
    public class ValueList : List<object?>
    {
    }
}
