// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.Atlas.CommandLine.OAuth2;

namespace Microsoft.Atlas.CommandLine.Blueprints.Providers
{
    internal class HttpsFilesBlueprintPackage : IBlueprintPackage
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private UriParts _blueprintUri;
        private HttpClient _httpClient;

        public HttpsFilesBlueprintPackage(IHttpClientFactory httpClientFactory, UriParts blueprintUri)
        {
            _httpClientFactory = httpClientFactory;
            _blueprintUri = blueprintUri;
            _httpClient = _httpClientFactory.Create(auth: null);
        }

        public string Location => _blueprintUri.ToString();

        public bool Exists(string path)
        {
            var request = new HttpRequestMessage(HttpMethod.Head, _blueprintUri.Append(path).ToString());
            var response = _httpClient.SendAsync(request).GetAwaiter().GetResult();

            var statusCode = (int)response.StatusCode;
            if (statusCode == 404)
            {
                return false;
            }

            response.EnsureSuccessStatusCode();
            return true;
        }

        public IEnumerable<string> GetGeneratedPaths() => Enumerable.Empty<string>();

        public TextReader OpenText(string path)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _blueprintUri.Append(path).ToString());
            var response = _httpClient.SendAsync(request).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var text = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return new StringReader(text);
        }
    }
}
