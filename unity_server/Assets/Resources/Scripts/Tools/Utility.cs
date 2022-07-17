using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace eggcation
{
    public static class Utility
    {
        private const string url = "";

        private static string dataResponse;

        public static JObject requestData(JObject jsonRequest, string api)
        {
            StartCoroutine(requestPostApi(jsonRequest.ToString(), api));
            return JObject.parse(dataResponse);
        }

        private static IEnumerator
        requestPostApi(string stringJsonRequest, string api)
        {
            using (
                UnityWebRequest webRequest =
                    UnityWebRequest.Post(url + api, stringJsonRequest)
            )
            {
                byte[] byteJsonRequest =
                    new UTF8Encoding().GetBytes(stringJsonRequest);
                webRequest.uploadHandler =
                    new UploadHandlerRaw(byteJsonRequest);

                webRequest.SetRequestHeader("Content-Type", "application/json");

                yield return webRequest.SendWebRequest();

                if (webRequest == UnityWebRequest.Result.Success)
                {
                    dataResponse = webRequest.downloadHandler.text;
                }
                else
                {
                    dataResponse = webRequest.error;
                }
            }
        }
    }
}
