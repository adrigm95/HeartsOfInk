using Assets.Scripts.Data.Constants;
using NETCoreServer.Models;
using Newtonsoft.Json;
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
        public async Task<HOIResponseModel<S>> GenericWebServiceCaller(string baseAdress, Method method, string targetRequest)
        {
            WebServiceCaller<object, S> webServiceCaller = new WebServiceCaller<object, S>();

            return await webServiceCaller.GenericWebServiceCaller(baseAdress, method, targetRequest, null);
        }
    }

    public class WebServiceCaller<T, S>
    {
        /// <summary>
        /// Método genérico para llamadas a API.
        /// </summary>
        /// <param name="requestBody"> Request body to serialize as json. </param>
        /// <param name="targetRequest"> Value like "api/Home/GetData". </param>
        public async Task<HOIResponseModel<S>> GenericWebServiceCaller(string baseAdress, Method method, string targetRequest, object requestBody)
        {
            HttpClient client = new HttpClient();
            HOIResponseModel<S> serverResponse;
            HttpResponseMessage response = null;
            HttpContent content;
            string json = string.Empty;
            string responseContent = string.Empty;
            long start;
            long end;
            TimeSpan difference;

            try
            {
                start = DateTime.Now.Ticks;
                client.BaseAddress = new Uri(baseAdress);

                if (requestBody != null)
                {
                    json = JsonConvert.SerializeObject(requestBody);

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

                responseContent = await response.Content.ReadAsStringAsync();
                serverResponse = JsonConvert.DeserializeObject<HOIResponseModel<S>>(responseContent);
                end = DateTime.Now.Ticks;
                difference = TimeSpan.FromTicks(end - start);
                Debug.Log("Start: " + start + " End: " + end + " Difference: " + difference);

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
                Debug.LogError($"Error on connection: {ex} for response {responseContent}");
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
