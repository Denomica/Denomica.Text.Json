using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Denomica.Text.Json
{
    /// <summary>
    /// Represents a JSON object.
    /// </summary>
    public class ValueDictionary : Dictionary<string, object?>
    {

        /// <summary>
        /// Creates a new <see cref="ValueDictionary"/> object from the given object.
        /// </summary>
        /// <param name="obj">The object to convert to a <see cref="ValueDictionary"/> object.</param>
        public static ValueDictionary Create(object obj, JsonSerializerOptions serializerOptions = null)
        {
            if(null == obj) throw new ArgumentNullException(nameof(obj));

            var elem = JsonSerializer.SerializeToElement(obj, options: serializerOptions);
            return elem.ToDictionary();
        }

    }
}
