using System;
using System.Collections.Generic;
using System.Text;

namespace Denomica.Text.Json
{
    /// <summary>
    /// Represents a JSON object.
    /// </summary>
    public class ValueDictionary : Dictionary<string, object?>
    {
        /// <summary>
        /// Returns the value with the given key as a <see cref="ValueList"/> type, if a value with
        /// the given key exists and the given value is a <see cref="ValueList"/> type.
        /// </summary>
        /// <param name="key">The key to return as a <see cref="ValueList"/>.</param>
        public ValueList? GetValueList(string key)
        {
            if(this.ContainsKey(key) && this[key] is ValueList)
            {
                return (ValueList?)this[key];
            }
            return null;
        }

        /// <summary>
        /// Returns the value with the given key as a <see cref="ValueDictionary"/> type, if a value with
        /// the given key exists and the given value is a <see cref="ValueDictionary"/> type.
        /// </summary>
        /// <param name="key">The key to return as a <see cref="ValueDictionary"/>.</param>
        public ValueDictionary? GetValueDictionary(string key)
        {
            if(this.ContainsKey(key) && this[key] is ValueDictionary)
            {
                return (ValueDictionary?)this[key];
            }
            return null;
        }
    }
}
