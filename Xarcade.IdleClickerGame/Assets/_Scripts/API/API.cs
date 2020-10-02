using System.Collections;
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

        public static IEnumerator RegisterPlayer(string firstName,string lastName,string email,string userName,string password,string confirmPassword, bool acceptTerms)
        {
            string body ="{\"firstName\":\""+ firstName+ 
                            "\",\"lastName\": \""+ lastName +
                            "\",\"email\": \""+ email +
                            "\",\"userName\": \""+ userName +
                            "\",\"password\": \""+ password +
                            "\",\"confirmPassword\": \""+ confirmPassword +
                            "\",\"acceptTerms\": \""+ acceptTerms +
                            "\"}";  

            UnityWebRequest request = GenerateJSONRequest(URL + "xarcadeaccount/register/", body, "POST");
            yield return request.Send();
            if(!String.IsNullOrEmpty(request.downloadHandler.text)) 
            {
                JSONNode jsonResult = JSON.Parse(request.downloadHandler.text);
                Debug.Log(jsonResult[0]); 

            }
            else
            {
                Debug.Log("No result!");
            }
        }

        public static IEnumerator Login(string username, string password, Action<Account> callback)
        {
            string body ="{\"username\":\""+ username+"\",\"password\": \""+ password +"\"}";  

            UnityWebRequest request = GenerateJSONRequest(URL + "xarcadeaccount/login/", body, "POST");
            yield return request.Send();
            if(!String.IsNullOrEmpty(request.downloadHandler.text) || request.isNetworkError || request.isHttpError)  
            {
                JSONNode jsonResult = JSON.Parse(request.downloadHandler.text);
                authorizationToken = jsonResult[0]["jwtToken"]; 
                Account x = new Account();
                Debug.Log(jsonResult[0]["account"]);


                //x = JsonUtility.FromJson<Account>(jsonResult[0]["account"]);  //TODO Dapat ma deserialize ni siya from JSON mahimog Account
                //Debug.Log(x.firstName);  //Then i print ni siya


                //callback(x);

            }
            else
            {
                Debug.Log("No result!");
            }

        }

        public static IEnumerator GetToken(string userId, string tokenId, Action<Account> callback)
        {
            UnityWebRequest request = GenerateGETRequest(URL + "token/token/?userId=" + userId + "&tokenId=" + tokenId);
            if(request != null)
            {
                yield return request.SendWebRequest();

                if(!String.IsNullOrEmpty(request.downloadHandler.text) || request.isNetworkError || request.isHttpError) 
                {
                    JSONNode jsonResult = JSON.Parse(request.downloadHandler.text);
                    Debug.Log(jsonResult);
                    Token x = JsonUtility.FromJson<Token>(jsonResult);
                    
                    //callback(x);

                }
                else
                {
                    Debug.Log("No result!");
                }
            }
            else
            {
                Debug.Log("User is not Authenticated!");
                yield return null;
            }

        }
        
        public static IEnumerator SendToken(string senderId, string receiverId, string tokenId, long amount, string message, Action<Account> callback)
        {
            WWWForm form = new WWWForm();
            form.AddField("senderId", senderId);
            form.AddField("receiverId", receiverId);
            form.AddField("tokenId", tokenId);
            form.AddField("amount", amount+"");
            form.AddField("message", message);
            UnityWebRequest request = GeneratePOSTRequest(URL + "transaction/send/token", form);
            if(request != null)
            {
                yield return request.SendWebRequest();

                if(!String.IsNullOrEmpty(request.downloadHandler.text) || request.isNetworkError || request.isHttpError) 
                {
                    JSONNode jsonResult = JSON.Parse(request.downloadHandler.text);
                    Debug.Log(jsonResult);
                    Token x = JsonUtility.FromJson<Token>(jsonResult);
                    
                    //callback(x);

                }
                else
                {
                    Debug.Log("No result!");
                }
            }
            else
            {
                Debug.Log("User is not Authenticated!");
                yield return null;
            }

        }
















        public static UnityWebRequest GeneratePOSTRequest(string url, WWWForm form)
        {
            if(String.IsNullOrEmpty(authorizationToken)) return null;
            ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
            var request = UnityWebRequest.Post(url, form);
            Debug.Log(request.uploadHandler.data);
            request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + authorizationToken);

            return request;
        }


        
        public static UnityWebRequest GenerateGETRequest(string uri)
        {
            if(String.IsNullOrEmpty(authorizationToken)) return null;
            ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
            var request = UnityWebRequest.Get(uri);
            request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + authorizationToken);

            return request;
        }

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
