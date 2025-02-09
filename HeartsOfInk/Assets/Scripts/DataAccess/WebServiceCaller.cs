using NETCoreServer.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
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
        public async Task<HOIResponseModel<S>> GenericWebServiceCaller(string baseAdress, Method method, string targetRequest, T requestBody)
        {
            HOIResponseModel<S> serverResponse;
            HttpResponseMessage response = null;
            HttpContent content = null;
            string json = string.Empty;
            string responseContent = string.Empty;
            long start;
            long end;
            TimeSpan difference;

            try
            {
                if (requestBody == null)
                {
                    Debug.Log($"Calling to: {baseAdress + targetRequest}");
                }
                else if (method == Method.GET)
                {
                    targetRequest = targetRequest + "/" + requestBody;
                    Debug.Log($"Calling to: {baseAdress + targetRequest}");
                }
                else
                {
                    json = JsonConvert.SerializeObject(requestBody);
                    content = new StringContent(json, Encoding.UTF8, "application/json");

                    Debug.Log($"Sending to: {baseAdress + targetRequest} json: {json}");
                }

                start = DateTime.Now.Ticks;
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseAdress);

                    switch (method)
                    {
                        case Method.POST:
                            response = await client.PostAsync(targetRequest, content);
                            break;
                        case Method.PUT:
                            response = await client.PutAsync(targetRequest, content);
                            break;
                        case Method.DELETE:
                            response = await client.DeleteAsync(targetRequest);
                            break;
                        case Method.GET:
                            response = await client.GetAsync(targetRequest);
                            break;
                    }

                    responseContent = await response.Content.ReadAsStringAsync();
                    serverResponse = JsonConvert.DeserializeObject<HOIResponseModel<S>>(responseContent);
                    LogConnectionResponse(response.StatusCode);

                    end = DateTime.Now.Ticks;
                    difference = TimeSpan.FromTicks(end - start);
                }

                Debug.Log("Start: " + start + " End: " + end + " Difference: " + difference);
                LogServerResponse(serverResponse);
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
                case System.Net.HttpStatusCode.BadRequest:
                    Debug.LogError("Bad request: 400");
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
