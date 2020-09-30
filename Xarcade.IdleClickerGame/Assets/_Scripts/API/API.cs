﻿using System.Collections;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using SimpleJSON;
using Xarcade.Models;

namespace Xarcade
{
    public static class API
    {
        public static string URL = "http://localhost:5000/";
        public static string authorizationToken = "";    


        public static IEnumerator Login(string username, string password, Action<Account> callback)
        {
            string body ="{\"username\":\""+ username+"\",\"password\": \""+ password +"\"}";  

            UnityWebRequest request = GenerateJSONRequest(URL + "xarcadeaccount/login/", body, "POST");
            yield return request.Send();
            JSONNode jsonResult = JSON.Parse(request.downloadHandler.text.Trim());
            authorizationToken = jsonResult[0]["jwtToken"]; 
            var x = JsonUtility.FromJson<Account>(jsonResult[0]["account"]);
            if(x!=null)
                callback(x);
            else
                Debug.Log("Request returned Null!");
        }

        /*
        public static UnityWebRequest GeneratePARAMRequest(string url, WWWForm param, string callType)
        {
            ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
            var request = new UnityWebRequest.Post(url, callType);
            request.uploadHandler = (UploadHandler) new UploadHandlerRaw(param);
            request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            return request;
        }*/
        public static UnityWebRequest GenerateJSONRequest(string url, string bodyJsonString, string callType)
        {
            ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
            var request = new UnityWebRequest(url, callType);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            return request;
        }
        

    //http://stackoverflow.com/questions/3674692/mono-webclient-invalid-ssl-certificates
        private static bool TrustCertificate(object sender, X509Certificate x509Certificate, X509Chain x509Chain, SslPolicyErrors sslPolicyErrors)
        {
            // all Certificates are accepted
            return true;
        }
    }

}
