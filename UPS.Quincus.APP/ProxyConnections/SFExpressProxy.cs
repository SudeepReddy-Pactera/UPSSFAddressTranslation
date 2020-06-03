﻿namespace UPS.Quincus.APP.ProxyConnections
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using UPS.Application.CustomLogs;
    using UPS.Quincus.APP.Common;
    using UPS.Quincus.APP.Request;
    using UPS.Quincus.APP.Response;

    public class SFExpressProxy
    {
        public ICustomLog iCustomLog { get; set; }
        private HttpClientHandler GetHttpClientHandler()
        {
            WebProxy myProxy = new WebProxy(MapProxy.webProxyURI, false, null, new NetworkCredential(MapProxy.webProxyUsername, MapProxy.webProxyPassword));
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.Proxy = myProxy;

            return httpClientHandler;
        }

        public async Task<GetSFCreateOrderServiceResponse> getSFCreateOrderServiceResponse(SFCreateOrderServiceRequest sFCreateOrderServiceRequest)
        {
            GetSFCreateOrderServiceResponse getSFCreateOrderServiceResponse = new GetSFCreateOrderServiceResponse();
            //string verifyText = sFCreateOrderServiceRequest.Checkword;
            try
            {
                string toVerifyText = sFCreateOrderServiceRequest.RequestOrderXMLMessage + sFCreateOrderServiceRequest.Checkword;

                System.Security.Cryptography.MD5 hs = System.Security.Cryptography.MD5.Create();

                byte[] base64Encryption = hs.ComputeHash(System.Text.Encoding.UTF8.GetBytes(toVerifyText));

                string base64VeirificatioCode = Convert.ToBase64String(base64Encryption);

                Dictionary<String, String> map = new Dictionary<string, string>();
                map.Add("xml", sFCreateOrderServiceRequest.RequestOrderXMLMessage);
                map.Add("verifyCode", base64VeirificatioCode);
                map.Add("checkCode", sFCreateOrderServiceRequest.Checkcode);

                IEnumerable<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();
                keyValuePairs = map;

                string resultContent = string.Empty;
                HttpClient httpClient = null;

                if (string.Equals(MapProxy.WebProxyEnable, false.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    httpClient = new HttpClient();
                }
                else
                {
                    httpClient = new HttpClient(GetHttpClientHandler());
                }

                using (var client = httpClient)
                {
                    client.BaseAddress = new Uri(sFCreateOrderServiceRequest.BaseURI);
                    var content = new FormUrlEncodedContent(keyValuePairs);


                    var result = await client.PostAsync(sFCreateOrderServiceRequest.RequestURI, content);

                    resultContent = await result.Content.ReadAsStringAsync();

                    getSFCreateOrderServiceResponse.OrderResponse = resultContent;
                    getSFCreateOrderServiceResponse.Response = true;

                }
            }
            catch(Exception exception)
            {
                getSFCreateOrderServiceResponse.exception = exception;

                await iCustomLog.AddLogEntry(new UPS.DataObjects.LogData.LogDataModel()
                {
                    apiTypes = UPS.DataObjects.LogData.APITypes.SFExpress,
                    apiType = Enum.GetName(typeof(UPS.DataObjects.LogData.APITypes), 1),
                    dateTime = System.DateTime.Now,
                    LogInformation = new UPS.DataObjects.LogData.LogInformation()
                    {
                        LogException = exception.InnerException.ToString(),
                        LogRequest = JsonConvert.SerializeObject(sFCreateOrderServiceRequest),
                        LogResponse = null
                    }
                });
            }

            return getSFCreateOrderServiceResponse;
        }

        public async Task<GetSFCancelOrderServiceResponse> getSFCancelOrderServiceResponse(SFCancelOrderServiceRequest sFCancelOrderServiceRequest)
        {
            GetSFCancelOrderServiceResponse getSFCancelOrderServiceResponse = new GetSFCancelOrderServiceResponse();

            try
            {
                string toVerifyText = sFCancelOrderServiceRequest.RequestOrderXMLMessage + sFCancelOrderServiceRequest.Checkword;

                System.Security.Cryptography.MD5 hs = System.Security.Cryptography.MD5.Create();

                byte[] base64Encryption = hs.ComputeHash(System.Text.Encoding.UTF8.GetBytes(toVerifyText));

                string base64VeirificatioCode = Convert.ToBase64String(base64Encryption);

                Dictionary<String, String> map = new Dictionary<string, string>();
                map.Add("xml", sFCancelOrderServiceRequest.RequestOrderXMLMessage);
                map.Add("verifyCode", base64VeirificatioCode);
                map.Add("checkCode", sFCancelOrderServiceRequest.Checkword);

                IEnumerable<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();
                keyValuePairs = map;

                string resultContent = string.Empty;

                HttpClient httpClient = null;

                if (string.Equals(MapProxy.WebProxyEnable, false.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    httpClient = new HttpClient();
                }
                else
                {
                    httpClient = new HttpClient(GetHttpClientHandler());
                }

                using (var client = httpClient) 
                {
                    client.BaseAddress = new Uri(sFCancelOrderServiceRequest.BaseURI);
                    var content = new FormUrlEncodedContent(keyValuePairs);


                    var result = await client.PostAsync(sFCancelOrderServiceRequest.RequestURI, content);

                    resultContent = await result.Content.ReadAsStringAsync();

                    getSFCancelOrderServiceResponse.OrderResponse = resultContent;
                    getSFCancelOrderServiceResponse.Response = true;

                }
            }
            catch (Exception exception)
            {
                getSFCancelOrderServiceResponse.exception = exception;
                await iCustomLog.AddLogEntry(new UPS.DataObjects.LogData.LogDataModel()
                {
                    apiTypes = UPS.DataObjects.LogData.APITypes.SFExpress,
                    apiType = Enum.GetName(typeof(UPS.DataObjects.LogData.APITypes), 1),
                    dateTime = System.DateTime.Now,
                    LogInformation = new UPS.DataObjects.LogData.LogInformation()
                    {
                        LogException = exception.InnerException.ToString(),
                        LogRequest = JsonConvert.SerializeObject(sFCancelOrderServiceRequest),
                        LogResponse = null
                    }
                });

            }

            return getSFCancelOrderServiceResponse;
        }




        //public static SFTranslationAPIResponse GetSFTranslatedAddress()
        //{
        //    SFTranslationAPIResponse sfTranslationAPIResponse = new SFTranslationAPIResponse();
        //    var input = string.Empty;
        //    try
        //    {
        //        var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://outint.sf-express.com/V1/translation/single");
        //        if (string.Equals(MapProxy.WebProxyEnable, true.ToString(), StringComparison.OrdinalIgnoreCase))
        //        {
        //            WebProxy myProxy = new WebProxy(MapProxy.webProxyURI, false, null, new NetworkCredential(MapProxy.webProxyUsername, MapProxy.webProxyPassword));
        //            httpWebRequest.Proxy = myProxy;
        //        }
        //        httpWebRequest.ContentType = "application/json";
        //        httpWebRequest.Method = "POST";
        //        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        //        {
        //            input = "{\"address_en\":\"" + "Baidu International Building, Guangdong Province, Shenzhen City, Nanshan District Room 111, 5th Floor, Block C, 1st Floor AAA AAA" + "\"," +
        //                        "\"appId\":\"" + "ups"+ "\"" +
        //                        "\"token\":\"" + "83f6016d-d206-4619-89e8-666313286d13" + "\"" +
        //                        "\"company\":\"" + "Company" + "\"" +
        //                        "\"contacts\":\"" + "Contacts" + "\"" +
        //                        "\"mobile\":\"" + "18936127776" + "\"" +
        //                        "\"orderid\":\"" + "123456" + "\"" +
        //                        "\"tel\":\"" + "18936127776" + "\"" +
        //                        "}";

        //            streamWriter.Write(input);
        //            streamWriter.Flush();
        //            streamWriter.Close();
        //        }
        //        httpWebRequest.KeepAlive = false;
        //        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //        string response;

        //        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //        {
        //            response = streamReader.ReadToEnd();
        //            streamReader.Close();
        //        }

        //        if (!string.IsNullOrWhiteSpace(response))
        //        {
        //            sfTranslationAPIResponse.data = JsonConvert.DeserializeObject<SFTranslationDataResponse>(response);
        //            sfTranslationAPIResponse.responseStatus = true;
        //        }

        //        httpResponse.Close();

        //        Task.Run(() => AuditEventEntry.LogEntry(new DataObjects.LogData.LogDataModel()
        //        {
        //            dateTime = DateTime.Now,
        //            apiTypes = DataObjects.LogData.APITypes.SFTranslation_API,
        //            apiType = "SFTranslation_API",
        //            LogInformation = new DataObjects.LogData.LogInformation()
        //            {
        //                LogResponse = response,
        //                LogRequest = string.Format("Senstive Information Identified {0}", System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(input))),
        //                LogException = null
        //            }
        //        }));

        //    }
        //    catch (Exception exception)
        //    {
        //        sfTranslationAPIResponse.exception = exception;
        //        Task.Run(() => AuditEventEntry.LogEntry(new DataObjects.LogData.LogDataModel()
        //        {
        //            dateTime = DateTime.Now,
        //            apiTypes = DataObjects.LogData.APITypes.SFTranslation_API,
        //            apiType = "SFTranslation_API",
        //            LogInformation = new DataObjects.LogData.LogInformation()
        //            {
        //                LogResponse = null,
        //                LogRequest = string.Format("Senstive Information Identified {0}", System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(input))),
        //                LogException = exception.InnerException.ToString()

        //            }
        //        }));
        //    }

        //    return sfTranslationAPIResponse;
        //}



    }
}
