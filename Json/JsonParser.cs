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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Iridium.Json
{
    public class JsonParser
    {
        public static JsonObject Parse(string json)
        {
            return new JsonParser(json)._Parse();
        }

        public static JsonObject Parse(Stream stream)
        {
            return new JsonParser(stream)._Parse();
        }

        private readonly JsonTokenizer _tokenizer;
        private JsonToken _currentToken;

        public JsonParser(string s)
        {
            _tokenizer = new JsonTokenizer(s);
        }

        public JsonParser(Stream stream)
        {
            _tokenizer = new JsonTokenizer(stream);
        }

        private JsonObject _Parse()
        {
            NextToken();

            return ParseValue();
        }

        private void NextToken()
        {
            _currentToken = _tokenizer.NextToken();
        }

        private JsonToken NextToken(JsonTokenType tokenType)
        {
            var token = _currentToken;

            if (token.Type != tokenType)
                throw new Exception("Expected " + tokenType);

            _currentToken = _tokenizer.NextToken();

            return token;
        }
        
        private JsonObject ParseObject()
        {
            Dictionary<string, JsonObject> obj =  new Dictionary<string, JsonObject>();

            NextToken(JsonTokenType.ObjectStart);

            for (;;)
            {
                if (_currentToken.Type == JsonTokenType.ObjectEnd)
                    break;

                var propNameToken = NextToken(JsonTokenType.String);

                NextToken(JsonTokenType.Colon);

                obj[propNameToken.Token] = ParseValue();

                if (_currentToken.Type != JsonTokenType.Comma)
                    break;

                NextToken();
            }

            NextToken(JsonTokenType.ObjectEnd);

            var jsonObject = JsonObject.FromDictionary(obj);

            return jsonObject;
        }

        private JsonObject ParseValue()
        {
            switch (_currentToken.Type)
            {
                case JsonTokenType.ObjectStart:
                    return ParseObject();

                case JsonTokenType.ArrayStart:
                    return ParseArray();

                case JsonTokenType.Null:
                    NextToken();
                    return JsonObject.FromValue(null);

                case JsonTokenType.True:
                    NextToken();
                    return JsonObject.FromValue(true);

                case JsonTokenType.False:
                    NextToken();
                    return JsonObject.FromValue(false);

                case JsonTokenType.Integer:
                    return ParseNumber();

                case JsonTokenType.Float:
                    return ParseNumber();

                case JsonTokenType.String:
                    return ParseString();

                default:
                    throw new Exception("Unexpected token " + _currentToken.Type);
            }
        }

        private JsonObject ParseNumber()
        {
            if (_currentToken.Type != JsonTokenType.Float && _currentToken.Type != JsonTokenType.Integer)
                throw new Exception("Number expected");

            try
            {
                if (_currentToken.Type == JsonTokenType.Integer)
                {
                    long longValue = Int64.Parse(_currentToken.Token);

                    if (longValue > Int32.MinValue && longValue < Int32.MaxValue)
                    {
                        return JsonObject.FromValue((int) longValue);
                    }
                    else
                    {
                        return JsonObject.FromValue(longValue);
                    }
                }
                else
                {
                    return JsonObject.FromValue(Double.Parse(_currentToken.Token));
                }
            }
            finally
            {
                NextToken();
            }
        }

        private JsonObject ParseString()
        {
            var stringToken = NextToken(JsonTokenType.String);

            return JsonObject.FromValue(stringToken.Token);
        }

        private JsonObject ParseArray()
        {
            NextToken(JsonTokenType.ArrayStart);
 
            var list = new List<JsonObject>();

            for (;;)
            {
                if (_currentToken.Type == JsonTokenType.ArrayEnd)
                    break;

                list.Add(ParseValue());

                if (_currentToken.Type != JsonTokenType.Comma)
                    break;

                NextToken();
            }

            NextToken(JsonTokenType.ArrayEnd);

            var jsonArray = JsonObject.FromArray(list);

            return jsonArray;
        }
    }
}