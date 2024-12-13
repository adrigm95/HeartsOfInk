using NETCoreServer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DataAccess
{
    /// <summary>
    /// Clase para realizar multiples llamadas secuenciales a servicios web del mismo servidor.
    /// </summary>
    /// <typeparam name="S"></typeparam>
    public class WebServiceCallerReusable<S>
    {
        private WebServiceCallerReusable<object, S> webServiceCaller;
        public bool MakingCall { get { return webServiceCaller.MakingCall; } }

        public WebServiceCallerReusable(string baseAdress)
        {
            webServiceCaller = new WebServiceCallerReusable<object, S>(baseAdress);
        }

        public async Task<HOIResponseModel<S>> GenericWebServiceCaller(Method method, string targetRequest)
        {
            return await webServiceCaller.GenericWebServiceCaller(method, targetRequest, null);
        }
    }

    public class WebServiceCallerReusable<T, S>
    {
        private readonly HttpClient client;
        private bool makingCall;
        private float totalExecutionTime;
        private int callsMaked;
        public float AverageTimeForCalls { get { return totalExecutionTime / callsMaked; } }
        public bool MakingCall { get { return makingCall; } }

        public WebServiceCallerReusable(string baseAddress)
        {
            makingCall = false;
            client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
        }

        public void AddAuthorizationToken(string bearerToken)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        /// <summary>
        /// Método genérico para llamadas a API.
        /// </summary>
        /// <param name="requestBody"> Request body to serialize as json. </param>
        /// <param name="targetRequest"> Value like "api/Home/GetData". </param>
        public async Task<HOIResponseModel<S>> GenericWebServiceCaller(Method method, string targetRequest, T requestBody)
        {
            HOIResponseModel<S> serverResponse;
            HttpResponseMessage response = null;
            HttpContent content;
            string json = string.Empty;
            string responseContent = string.Empty;
            long start;
            long end;
            TimeSpan difference;

            makingCall = true;
            try
            {
                if (requestBody != null)
                {
                    json = JsonConvert.SerializeObject(requestBody);

                    Debug.Log($"Sending to: {client.BaseAddress + targetRequest} json: {json}");
                }
                content = new StringContent(json, Encoding.UTF8, "application/json");

                start = DateTime.Now.Ticks;
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

                totalExecutionTime += difference.Milliseconds + (difference.Seconds * 1000);
                callsMaked += 1;
                Debug.Log("Avg: " + AverageTimeForCalls + " (ms); Start: " + start + " End: " + end + " Difference: " + difference);
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
            finally
            {
                makingCall = false;
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
                case System.Net.HttpStatusCode.Unauthorized:
                    Debug.LogError("Unauthorized: 401");
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
