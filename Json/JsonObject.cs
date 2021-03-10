#region License
//=============================================================================
// Iridium-Core - Portable .NET Productivity Library 
//
// Copyright (c) 2008-2017 Philippe Leybaert
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//=============================================================================
#endregion

using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace Iridium.Json
{
    public class JsonObject
    {
        private object _value;
        private JsonObjectType _type;

        public JsonObject()
        {
            _type = JsonObjectType.Undefined;
        }

        private JsonObject(List<JsonObject> array)
        {
            _value = array.ToArray();
            _type = JsonObjectType.Array;
        }

        private JsonObject(Dictionary<string, JsonObject> obj)
        {
            _value = obj;
            _type = JsonObjectType.Object;
        }

        private JsonObject(JsonObjectType type, object value = null)
        {
            _type = type;

            switch (type)
            {
                case JsonObjectType.Value:
                    _value = value;
                    break;
                case JsonObjectType.Array:
                    _value = new JsonObject[0];
                    break;
                case JsonObjectType.Object:
                    _value = new Dictionary<string, JsonObject>();
                    break;
            }
        }

        public bool IsObject => _type == JsonObjectType.Object;
        public bool IsArray => _type == JsonObjectType.Array;
        public bool IsValue => _type == JsonObjectType.Value;
        public bool IsNull => _value == null && _type != JsonObjectType.Undefined;
        public bool IsNullOrUndefined => _value == null;
        public object Value => IsValue || IsArray || IsObject ? _value : null;

        public static JsonObject Undefined() => new JsonObject(JsonObjectType.Undefined);
        public static JsonObject EmptyObject() => new JsonObject(JsonObjectType.Object);
        public static JsonObject EmptyArray() => new JsonObject(JsonObjectType.Array);
        public static JsonObject Null() => new JsonObject(JsonObjectType.Value, null);

        internal static JsonObject FromValue(object value) => new JsonObject(JsonObjectType.Value, value);
        internal static JsonObject FromArray(List<JsonObject> array) => new JsonObject(array);
        internal static JsonObject FromDictionary(Dictionary<string,JsonObject> dictionary) => new JsonObject(dictionary);
    }
}