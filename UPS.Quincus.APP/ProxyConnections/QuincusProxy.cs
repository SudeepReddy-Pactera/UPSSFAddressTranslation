﻿namespace UPS.Quincus.APP.ProxyConnections
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Cache;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using UPS.Application.CustomLogs;
    using UPS.DataObjects.Common;
    using UPS.Quincus.APP.Common;
    using UPS.Quincus.APP.Configuration;
    using UPS.Quincus.APP.Request;
    using UPS.Quincus.APP.Response;
    using UPS.Quincus.APP.Utilities;

    public static class QuincusProxy
    {
        public static QuincusTokenDataResponse GetToken(QuincusParams quincusParams)
        {
            QuincusTokenDataResponse quincusTokenDataResponse = new QuincusTokenDataResponse();
            var input = string.Empty;
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(quincusParams.endpoint);
                if (string.Equals(MapProxy.WebProxyEnable, true.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    WebProxy myProxy = new WebProxy(MapProxy.webProxyURI, false, null, new NetworkCredential(MapProxy.webProxyUsername, MapProxy.webProxyPassword));
                    httpWebRequest.Proxy = myProxy;
                }
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    input = "{\"username\":\"" + quincusParams.username + "\"," +
                                "\"password\":\"" + quincusParams.password + "\"}";

                    streamWriter.Write(input);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                httpWebRequest.KeepAlive = false;
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                    streamReader.Close();
                }

                if (!string.IsNullOrWhiteSpace(response))
                {
                    quincusTokenDataResponse.quincusTokenData = JsonConvert.DeserializeObject<QuincusTokenData>(response);
                    quincusTokenDataResponse.ResponseStatus = true;
                }

                httpResponse.Close();

                Task.Run(()=>AuditEventEntry.LogEntry(new DataObjects.LogData.LogDataModel()
                {
                    dateTime = DateTime.Now,
                    apiTypes = DataObjects.LogData.APITypes.QuincusAPI_Token,
                    apiType = "QuincusAPI_Token",
                    LogInformation = new DataObjects.LogData.LogInformation()
                    {
                        LogResponse = response,
                        LogRequest = string.Format("Senstive Information Identified {0}", System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(input))),
                        LogException = null
                    }
                }));

            }
            catch (Exception exception)
            {
                quincusTokenDataResponse.exception = exception;
                Task.Run(()=>AuditEventEntry.LogEntry(new DataObjects.LogData.LogDataModel()
                {
                    dateTime = DateTime.Now,
                    apiTypes = DataObjects.LogData.APITypes.QuincusAPI_Token,
                    apiType = "QuincusAPI_Token",
                    LogInformation = new DataObjects.LogData.LogInformation()
                    {
                        LogResponse = null,
                        LogRequest = string.Format("Senstive Information Identified {0}", System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(input))),
                        LogException = exception.InnerException.ToString()

                    }
                }));
            }

            return quincusTokenDataResponse;
        }

        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        public static QuincusTranslatedAddressResponse GetTranslatedAddressResponse(IQuincusAddressTranslationRequest quincusAddressTranslationRequest, QuincusParams quincusParams)
        {
            string response = string.Empty;
            var input = string.Empty;
            QuincusTranslatedAddressResponse quincusTranslatedAddressResponse = new QuincusTranslatedAddressResponse();
            quincusTranslatedAddressResponse.ResponseData = new List<GetBatchResponseForAddressTranslation>();

            try
            {
                quincusTranslatedAddressResponse.RequestDataCount = quincusAddressTranslationRequest.shipmentWorkFlowRequests.Count;

                List<string> content = GetRequestContextForAddress.GetAddressStringFromRequest(quincusAddressTranslationRequest.shipmentWorkFlowRequests, quincusParams);
                quincusTranslatedAddressResponse.QuincusContentRequest = JsonConvert.SerializeObject(content);

                content.ForEach(requestdata =>
                {

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(
                        quincusAddressTranslationRequest.endpoint);
                    if (string.Equals(MapProxy.WebProxyEnable, true.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        WebProxy myProxy = new WebProxy(MapProxy.webProxyURI, false, null, new NetworkCredential(MapProxy.webProxyUsername, MapProxy.webProxyPassword));

                        httpWebRequest.Proxy = myProxy;
                    }

                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Headers.Add("AUTHORIZATION", "JWT " + quincusAddressTranslationRequest.token);
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        input = requestdata;

                        streamWriter.Write(input);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        response = streamReader.ReadToEnd();

                        streamReader.Close();
                    }

                    httpResponse.Close();

                    quincusTranslatedAddressResponse.ResponseData.Add(JsonConvert.DeserializeObject<GetBatchResponseForAddressTranslation>(response));
                    quincusTranslatedAddressResponse.Response = true;
                    quincusTranslatedAddressResponse.ResponseData.ForEach(record =>
                    {
                        record.addresses.ForEach(address =>
                        {
                            address.rcV_CPY_TE =
                              Convert.ToString(
                                  quincusAddressTranslationRequest.shipmentWorkFlowRequests
                                  .FirstOrDefault(_ =>
                                    Convert.ToString(_.pkG_NR_TE) == address.id)
                                  .rcV_CPY_TE);
                        });

                    });


                    Task.Run(()=>AuditEventEntry.LogEntry(new DataObjects.LogData.LogDataModel()
                    {
                        dateTime = DateTime.Now,
                        apiTypes = DataObjects.LogData.APITypes.QuincusAPI_Translation,
                        apiType = Enum.GetName(typeof(UPS.DataObjects.LogData.APITypes), 4),
                        LogInformation = new DataObjects.LogData.LogInformation()
                        {
                            LogResponse = response,
                            LogRequest = JsonConvert.SerializeObject(quincusAddressTranslationRequest.shipmentWorkFlowRequests), 
                            LogException = null

                        }
                    }));

                });
            }
            catch (Exception exception)
            {
                quincusTranslatedAddressResponse.exception = exception;
                Task.Run(()=>AuditEventEntry.LogEntry(new DataObjects.LogData.LogDataModel()
                {
                    dateTime = DateTime.Now,
                    apiTypes = DataObjects.LogData.APITypes.QuincusAPI_Translation,
                    apiType = Enum.GetName(typeof(UPS.DataObjects.LogData.APITypes), 4),
                    LogInformation = new DataObjects.LogData.LogInformation()
                    {
                        LogResponse = null,
                        LogRequest = JsonConvert.SerializeObject(quincusAddressTranslationRequest.shipmentWorkFlowRequests),
                        LogException = exception.InnerException.ToString()

                    }
                }));
            }

            return quincusTranslatedAddressResponse;
        }

        public static QuincusResponse GetQuincusResponse(QuincusGeoCodeDataRequest quincusGeoCodeDataRequest)
        {

            QuincusResponse quincusResponse = new QuincusResponse();
            HttpWebResponse httpResponse = null;
            quincusResponse.QuincusReponseDataList = new List<QuincusReponseData>();
            try
            {

                List<string> webRequestURLS = new List<string>();

                quincusGeoCodeDataRequest.batchIDList.ForEach(requestID =>
                {
                    webRequestURLS.Add(quincusGeoCodeDataRequest.endpoint + requestID);
                });

                //Parallel.ForEach(webRequestURLS, urls =>
                // {
                //     var httpWebRequest = (HttpWebRequest)WebRequest.Create(urls);

                //     HttpRequestCachePolicy noCachePolicy =
                //         new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);

                //     httpWebRequest.CachePolicy = noCachePolicy;

                //     if (string.Equals(MapProxy.WebProxyEnable, true.ToString(), StringComparison.OrdinalIgnoreCase))
                //     {
                //         WebProxy myProxy = new WebProxy(MapProxy.webProxyURI, false, null, new NetworkCredential(MapProxy.webProxyUsername, MapProxy.webProxyPassword));
                //         httpWebRequest.Proxy = myProxy;
                //     }

                //     httpWebRequest.ContentType = "application/json";
                //     httpWebRequest.Headers.Add("AUTHORIZATION", "JWT " + quincusGeoCodeDataRequest.quincusTokenData.token);
                //     httpWebRequest.Method = "GET";
                //     httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                //     string response;

                //     using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                //     {
                //         response = streamReader.ReadToEnd();
                //         streamReader.Close();
                //     }

                //     if (!string.IsNullOrEmpty(response))
                //     {
                //         quincusResponse.QuincusReponseDataList.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<QuincusReponseData>(response));
                //         quincusResponse.ResponseStatus = true;
                //     }

                //     httpResponse.Close();
                // });

                //var intermediateResponse = quincusResponse.QuincusReponseDataList;

                quincusGeoCodeDataRequest.batchIDList.ForEach(requestData =>
                {
                    System.Threading.Thread.Sleep(5000);
                    HttpRequestCachePolicy requestCachePolicy =
                            new HttpRequestCachePolicy(HttpRequestCacheLevel.Default);

                    HttpWebRequest.DefaultCachePolicy = requestCachePolicy;

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(
                        quincusGeoCodeDataRequest.endpoint + requestData + "/");

                    HttpRequestCachePolicy noCachePolicy =
                        new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);

                    httpWebRequest.CachePolicy = noCachePolicy;

                    if (string.Equals(MapProxy.WebProxyEnable, true.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        WebProxy myProxy = new WebProxy(MapProxy.webProxyURI, false, null, new NetworkCredential(MapProxy.webProxyUsername, MapProxy.webProxyPassword));
                        httpWebRequest.Proxy = myProxy;
                    }

                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Headers.Add("AUTHORIZATION", "JWT " + quincusGeoCodeDataRequest.quincusTokenData.token);
                    httpWebRequest.Method = "GET";
                    httpWebRequest.Timeout = 60000;
                    httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    string response;

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        response = streamReader.ReadToEnd();
                        streamReader.Close();
                    }

                    if (!string.IsNullOrEmpty(response))
                    {
                        quincusResponse.QuincusReponseDataList.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<QuincusReponseData>(response));
                        quincusResponse.ResponseStatus = true;
                    }

                    httpResponse.Close();
                    httpResponse.Dispose();
                    httpWebRequest.Abort();

                    Task.Run(()=>AuditEventEntry.LogEntry(new DataObjects.LogData.LogDataModel()
                    {
                        dateTime = DateTime.Now,
                        apiTypes = DataObjects.LogData.APITypes.QuincusAPI_Batch,
                        apiType = "QuincusAPI_Batch",

                        LogInformation = new DataObjects.LogData.LogInformation()
                        {
                            LogResponse = response,
                            LogRequest = JsonConvert.SerializeObject(quincusGeoCodeDataRequest.batchIDList),
                            LogException = null

                        }
                    }));
                });
            }
            catch (Exception exception)
            {
                quincusResponse.Exception = exception;
                Task.Run(()=>AuditEventEntry.LogEntry(new DataObjects.LogData.LogDataModel()
                {
                    dateTime = DateTime.Now,
                    apiTypes = DataObjects.LogData.APITypes.QuincusAPI_Batch,
                    apiType = "QuincusAPI_Batch",
                    LogInformation = new DataObjects.LogData.LogInformation()
                    {
                        LogResponse = null,
                        LogRequest = JsonConvert.SerializeObject(quincusGeoCodeDataRequest.batchIDList),
                        LogException = exception.InnerException.ToString()

                    }
                }));
            }

            return quincusResponse;
        }
    }
}
