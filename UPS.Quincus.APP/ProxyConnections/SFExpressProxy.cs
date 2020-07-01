namespace UPS.Quincus.APP.ProxyConnections
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using UPS.Application.CustomLogs;
    using UPS.DataObjects.Common;
    using UPS.Quincus.APP.Common;
    using UPS.Quincus.APP.Configuration;
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



        public async Task<SFTranslationAPIResponse> GetSFTranslatedAddress(SFTranslationParams sfTranslationParams)
        {
            SFTranslationAPIResponse sfTranslationAPIResponse = new SFTranslationAPIResponse();
            string input = string.Empty;

            try
            {

                input = "{\"address_en\":\"" + sfTranslationParams.address_en + "\"," +
                                   "\"appId\":\"" + sfTranslationParams.appId + "\"," +
                                   "\"token\":\"" + sfTranslationParams.token + "\"," +
                                   "\"company\":\"" + sfTranslationParams.company + "\"," +
                                   "\"contacts\":\"" + sfTranslationParams.contacts + "\"," +
                                   "\"mobile\":\"" + sfTranslationParams.mobile + "\"," +
                                   "\"orderid\":\"" + sfTranslationParams.orderid + "\"," +
                                   "\"tel\":\"" + sfTranslationParams.tel + "\"" +
                                   "}";

                HttpClient client = null;
                if (string.Equals(MapProxy.WebProxyEnable, false.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    client = new HttpClient();
                }
                else
                {
                    client = new HttpClient(GetHttpClientHandler());
                }

                client.BaseAddress = new Uri(sfTranslationParams.endpoint);

                //client.DefaultRequestHeaders.Add("Authorization", authtoken);

                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync("", content).Result;

                string resultContent = await response.Content.ReadAsStringAsync();

                sfTranslationAPIResponse = JsonConvert.DeserializeObject<SFTranslationAPIResponse>(resultContent);

                await Task.Run(() => AuditEventEntry.LogEntry(new DataObjects.LogData.LogDataModel()
                {
                    dateTime = DateTime.Now,
                    apiTypes = DataObjects.LogData.APITypes.SFTranslation_API,
                    apiType = "SFTranslation_API",
                    LogInformation = new DataObjects.LogData.LogInformation()
                    {
                        LogResponse = resultContent,
                        LogRequest = input,
                        LogException = null
                    }
                }));
            }
            catch(Exception exception)
            {
                sfTranslationAPIResponse.exception = exception;
               await Task.Run(() => AuditEventEntry.LogEntry(new DataObjects.LogData.LogDataModel()
                {
                    dateTime = DateTime.Now,
                    apiTypes = DataObjects.LogData.APITypes.SFTranslation_API,
                    apiType = "SFTranslation_API",
                    LogInformation = new DataObjects.LogData.LogInformation()
                    {
                        LogResponse = null,
                        LogRequest = input,
                        LogException = exception.InnerException != null ? exception.InnerException.ToString() + exception.StackTrace : exception.Message.ToString() + exception.StackTrace

                    }
                }));
            }

            return sfTranslationAPIResponse;
        }






    }
}
