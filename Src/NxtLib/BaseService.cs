﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NxtLib.Local;

namespace NxtLib
{
    public abstract class BaseService
    {
        private readonly IDateTimeConverter _dateTimeConverter;
        private readonly string _baseUrl;

        protected BaseService(IDateTimeConverter dateTimeConverter, string baseUrl = Constants.DefaultNxtUrl)
        {
            _dateTimeConverter = dateTimeConverter;
            _baseUrl = baseUrl;
        }

        protected async Task<JObject> Get(string requestType)
        {
            return await Get(requestType, new Dictionary<string, string>());
        }

        protected async Task<JObject> Get(string requestType, Dictionary<string, string> queryParameters)
        {
            var url = BuildUrl(requestType, queryParameters);

            using (var client = new HttpClient())
            using (var response = await client.GetAsync(url))
            using (var content = response.Content)
            {
                var json = await content.ReadAsStringAsync();
                CheckForErrorResponse(json, url);
                return JObject.Parse(json);
            }
        }

        protected async Task<JObject> Post(string requestType, IEnumerable<KeyValuePair<string, string>> queryParameters)
        {
            using (var client = new HttpClient())
            using (var response = await client.PostAsync(_baseUrl, new FormUrlEncodedContent(queryParameters)))
            using (var content = response.Content)
            {
                var json = await content.ReadAsStringAsync();
                CheckForErrorResponse(json, _baseUrl);
                return JObject.Parse(json);
            }
        }

        protected async Task<T> Get<T>(string requestType) where T : IBaseReply
        {
            return await Get<T>(requestType, new Dictionary<string, string>());
        }

        protected async Task<T> Get<T>(string requestType, Dictionary<string, string> queryParamsDictionary) where T : IBaseReply
        {
            var url = BuildUrl(requestType, queryParamsDictionary);
            return await GetUrl<T>(url);
        }

        protected async Task<T> Get<T>(string requestType, Dictionary<string, List<string>> queryParamsDictionary) where T : IBaseReply
        {
            var url = BuildUrl(requestType, queryParamsDictionary);
            return await GetUrl<T>(url);
        }

        private static async Task<T> GetUrl<T>(string url) where T : IBaseReply
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(url))
            using (var content = response.Content)
            {
                return await ReadAndDeserializeResponse<T>(content, url);
            }
        }

        protected async Task<T> Post<T>(string requestType) where T : IBaseReply
        {
            return await Post<T>(requestType, new Dictionary<string, string>());
        }

        protected async Task<T> Post<T>(string requestType, Dictionary<string, string> queryParamsDictionary) where T : IBaseReply
        {
            queryParamsDictionary.Add("requestType", requestType);
            return await PostUrl<T>(_baseUrl, queryParamsDictionary);
        }

        protected async Task<T> Post<T>(string requestType, Dictionary<string, List<string>> queryParamsDictionary) where T : IBaseReply
        {
            var postData = (from queryParameterList in queryParamsDictionary
                            from queryParameter in queryParameterList.Value
                            select new KeyValuePair<string, string>(queryParameterList.Key, queryParameter))
                            .ToList();
            postData.Add(new KeyValuePair<string, string>("requestType", requestType));
            return await PostUrl<T>(_baseUrl, postData);
        }

        protected async Task<T> PostAsContent<T>(string requestType, Dictionary<string, string> queryParamsDictionary) where T : IBaseReply
        {
            queryParamsDictionary.Add("requestType", requestType);
            return await PostAsMultipartFormData<T>(_baseUrl, queryParamsDictionary);
        }

        private static async Task<T> PostAsMultipartFormData<T>(string url, Dictionary<string, string> queryParamsDictionary) where T : IBaseReply
        {
            using (var client = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent("---------------------------7da24f2e50046"))
            {
                foreach (var queryParameter in queryParamsDictionary)
                {
                    formDataContent.Add(new StringContent(queryParameter.Value), queryParameter.Key);
                }

                using (var response = await client.PostAsync(url, formDataContent))
                using (var content = response.Content)
                {
                    return await ReadAndDeserializeResponse<T>(content, url);
                }
            }
        }

        private static async Task<T> PostUrl<T>(string url, IEnumerable<KeyValuePair<string, string>> queryParameters) where T : IBaseReply
        {
            using (var client = new HttpClient())
            using (var response = await client.PostAsync(url, new FormUrlEncodedContent(queryParameters)))
            using (var content = response.Content)
            {
                return await ReadAndDeserializeResponse<T>(content, url);
            }
        }

        private string BuildUrl(string requestType, Dictionary<string, List<string>> queryParamsDictionary)
        {
            var url = _baseUrl + "?requestType=" + requestType;

            return queryParamsDictionary.Aggregate(url, (current1, keyValuePair) =>
                keyValuePair.Value.Aggregate(current1, (current, value) => current + ("&" + keyValuePair.Key + "=" + value)));
        }

        protected string BuildUrl(string requestType, Dictionary<string, string> queryParamsDictionary)
        {
            var url = _baseUrl + "?requestType=" + requestType;
            url = queryParamsDictionary.Aggregate(url, (current, queryParam) => current + ("&" + queryParam.Key + "=" + queryParam.Value));
            return url;
        }

        private static async Task<T> ReadAndDeserializeResponse<T>(HttpContent content, string url) where T : IBaseReply
        {
            var json = await content.ReadAsStringAsync();
            CheckForErrorResponse(json, url);
            var response = JsonConvert.DeserializeObject<T>(json);
            response.RawJsonReply = json;
            response.RequestUri = url;
            return response;
        }

        private static void CheckForErrorResponse(string json, string url)
        {
            var jObject = JObject.Parse(json);
            var errorCode = jObject.SelectToken("errorCode");
            var error = jObject.SelectToken("error");
            var errorDescription = jObject.SelectToken("errorDescription");

            if (errorCode != null)
            {
                throw new NxtException((int)errorCode, json, url, errorDescription.ToString());
            }
            if (error != null)
            {
                throw new NxtException(-1, json, url, error.ToString());
            }
        }

        protected static void AddToParametersIfHasValue(string keyName, bool? value, Dictionary<string, string> queryParameters)
        {
            if (value.HasValue)
            {
                queryParameters.Add(keyName, value.Value.ToString());
            }
        }

        protected static void AddToParametersIfHasValue(string keyName, byte? value, Dictionary<string, string> queryParameters)
        {
            if (value.HasValue)
            {
                queryParameters.Add(keyName, value.Value.ToString());
            }
        }

        protected static void AddToParametersIfHasValue(string keyName, long? value, Dictionary<string, string> queryParameters)
        {
            if (value.HasValue)
            {
                queryParameters.Add(keyName, value.Value.ToString());
            }
        }

        protected static void AddToParametersIfHasValue(string keyName, ulong? value, Dictionary<string, string> queryParameters)
        {
            if (value.HasValue)
            {
                queryParameters.Add(keyName, value.Value.ToString());
            }
        }

        protected static void AddToParametersIfHasValue(string keyName, string value, Dictionary<string, string> queryParameters)
        {
            if (!string.IsNullOrEmpty(value))
            {
                queryParameters.Add(keyName, value);
            }
        }

        protected void AddToParametersIfHasValue(DateTime? timeStamp, Dictionary<string, string> queryParameters)
        {
            AddToParametersIfHasValue("timestamp", timeStamp, queryParameters);
        }

        protected void AddToParametersIfHasValue(string keyName, DateTime? timeStamp, Dictionary<string, string> queryParameters)
        {
            if (timeStamp.HasValue)
            {
                var convertedTimeStamp = _dateTimeConverter.GetEpochTime(timeStamp.Value.ToUniversalTime());
                queryParameters.Add(keyName, convertedTimeStamp.ToString());
            }
        }

        protected void AddToParametersIfHasValue(string keyName, int? value,
            Dictionary<string, List<string>> queryParameters)
        {
            if (value.HasValue)
            {
                queryParameters.Add(keyName, new List<string> { value.Value.ToString() });
            }
        }

        protected void AddToParametersIfHasValue(string keyName, ulong? value,
            Dictionary<string, List<string>> queryParameters)
        {
            if (value.HasValue)
            {
                queryParameters.Add(keyName, new List<string> {value.Value.ToString()});
            }
        }

        protected void AddToParametersIfHasValue(string keyName, bool? value,
            Dictionary<string, List<string>> queryParameters)
        {
            if (value.HasValue)
            {
                queryParameters.Add(keyName, new List<string> { value.Value.ToString() });
            }
        }
    }
}