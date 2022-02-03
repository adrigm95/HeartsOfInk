using Assets.Scripts.Data.Constants;
using NETCoreServer.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DataAccess
{
    public enum Method { GET, PUT, POST, DELETE }

    public class WebServiceCaller<S>
    {
        public async Task<HOIResponseModel<S>> GenericWebServiceCaller(Method method, string targetRequest)
        {
            WebServiceCaller<object, S> webServiceCaller = new WebServiceCaller<object, S>();

            return await webServiceCaller.GenericWebServiceCaller(method, targetRequest, null);
        }
    }

    public class WebServiceCaller<T, S>
    {
        DataContractJsonSerializer sendSerializer = new DataContractJsonSerializer(typeof(T));
        DataContractJsonSerializer receiveSerializer = new DataContractJsonSerializer(typeof(HOIResponseModel<S>));

        /// <summary>
        /// Método genérico para llamadas a API.
        /// </summary>
        /// <param name="requestBody"> Request body to serialize as json. </param>
        /// <param name="targetRequest"> Value like "api/Home/GetData". </param>
        public async Task<HOIResponseModel<S>> GenericWebServiceCaller(Method method, string targetRequest, object requestBody)
        {
            HttpClient client = new HttpClient();
            MemoryStream memoryStream;
            StreamReader strReader;
            HOIResponseModel<S> serverResponse;
            HttpResponseMessage response = null;
            HttpContent content;
            string json = string.Empty;
            long start;
            long end;
            TimeSpan difference;

            try
            {
                start = DateTime.Now.Ticks;
                client.BaseAddress = new Uri(ApiConfig.NETCoreServerUrl);

                if (requestBody != null)
                {
                    memoryStream = new MemoryStream();
                    sendSerializer.WriteObject(memoryStream, requestBody);
                    memoryStream.Position = 0;
                    strReader = new StreamReader(memoryStream);
                    json = strReader.ReadToEnd();
                    memoryStream.Close();
                    strReader.Close();

                    Debug.Log("Sending json: " + json);
                }

                content = new StringContent(json, Encoding.UTF8, "application/json");

                switch (method)
                {
                    case Method.POST:
                        response = await client.PostAsync(targetRequest, content);
                        break;
                    case Method.PUT:
                        response = await client.PutAsync(targetRequest, content);
                        break;
                    case Method.DELETE:
                        throw new NotImplementedException("DELETE not implemented on WebServiceCaller");
                        break;
                    case Method.GET:
                        response = await client.GetAsync(targetRequest);
                        break;
                }

                memoryStream = new MemoryStream();
                await response.Content.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                end = DateTime.Now.Ticks;
                difference = TimeSpan.FromTicks(end - start);
                Debug.Log("Start: " + start + " End: " + end + " Difference: " + difference);
                serverResponse = (HOIResponseModel<S>)receiveSerializer.ReadObject(memoryStream);

                LogServerResponse(serverResponse);
                LogConnectionResponse(response.StatusCode);
            }
            catch (NullReferenceException ex)
            {
                serverResponse = new HOIResponseModel<S>();

                if (response == null)
                {
                    serverResponse.internalResultCode = InternalStatusCodes.KOBadResponse;
                    Debug.LogError("Error on client WebServiceCaller: server response cannot be empty.");
                }
                else
                {
                    serverResponse.internalResultCode = InternalStatusCodes.KOConnectionCode;
                    Debug.LogError("Error on client WebServiceCaller: unexpected null reference " + ex);
                }
            }
            catch (Exception ex)
            {
                serverResponse = new HOIResponseModel<S>();
                serverResponse.internalResultCode = InternalStatusCodes.KOConnectionCode;
                Debug.LogError("Error on connection: " + ex);
            }

            return serverResponse;
        }

        private void LogConnectionResponse(System.Net.HttpStatusCode httpStatusCode)
        {
            switch (httpStatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    break;
                default:
                    Debug.LogWarning("Non 200 http response: " + httpStatusCode.ToString());
                    break;
            }
        }

        private void LogServerResponse(HOIResponseModel<S> responseModel)
        {
            switch (responseModel.internalResultCode)
            {
                case InternalStatusCodes.KOConnectionCode:
                    Debug.LogError("Unexpected error on connection, exception maybe logged in origin method.");
                    break;
                case InternalStatusCodes.KOCode:
                    Debug.LogError("Server response with unexpected error: ObjectResponse: " + responseModel.serviceResponse);
                    break;
                default:
                    Debug.Log("Server response - InternalResultCode: " + responseModel.internalResultCode);
                    break;
            }
        }
    }
}
