using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor;
using UnityProject.Cookscape;

namespace UnityProject.Cookscape.Api
{
    public class User : MonoBehaviour
    {
        public static User instance = null;
        private static bool m_IsPending = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (instance != this)
                {
                    Destroy(gameObject);
                }
            }
        }

        public IEnumerator SignIn(string loginId, string password)
        {
            if (m_IsPending) {
                yield break;
            }

            StringBuilder urlBuilder = new StringBuilder(GameConstants.API_URL)
                    .Append("/v1/user/signin");
            m_IsPending = true;

            UnityWebRequest request;
            string jsonData = JsonConvert.SerializeObject(new LoginForm(loginId, password));

            using ( request = UnityWebRequest.Post(urlBuilder.ToString(), jsonData))
            {
                byte[] bytesData = new UTF8Encoding().GetBytes(jsonData);
                request.uploadHandler.Dispose();
                request.uploadHandler = new UploadHandlerRaw(bytesData);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();

                if (null != request.error)
                {
                    long responseCode = request.responseCode;
                    Debug.LogError(responseCode);
                    if (responseCode == 404) {
                        GameManager.instance.Alert("Block", "아이디를 확인해주세요.");
                    } else if (responseCode == 400) {
                        GameManager.instance.Alert("Block", "잘못된 비밀번호입니다.");
                    }
                    
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log("SignIn Request Success");
                    //var response = JsonConvert.DeserializeObject<UserIDForm>(request.downloadHandler.text);

                    // SET JWT TOKEN
                    string token = request.GetResponseHeader("Authorization");
                    Debug.Log($"TOKEN : {token}!!!!!!!!!");
                    GameManager.instance.SetAuthorization(token);

                    StartCoroutine(GetUser());
                    StartCoroutine(Data.instance.GetData());
                    StartCoroutine(Data.instance.GetUsageAvatar());
                    StartCoroutine(Data.instance.GetReward(true));
                    StartCoroutine(Data.instance.GetReward(false));
                    StartCoroutine(Information.instance.GetItem());
                    StartCoroutine(Information.instance.GetObject());
                    StartCoroutine(Information.instance.GetAvatar());
                    StartCoroutine(Information.instance.GetChallenge());
                    StartCoroutine(Information.instance.GetReward());
                }
            }
            m_IsPending = false;
        }

        public IEnumerator GetUser()
        {
            StringBuilder urlBuilder = new StringBuilder(GameConstants.API_URL)
                .Append("/v1/user");

            using (UnityWebRequest request = UnityWebRequest.Get(urlBuilder.ToString()))
            {
                request.SetRequestHeader("Authorization", GameManager.instance.GetAuthorization());
                yield return request.SendWebRequest();

                if (null != request.error)
                {
                    Debug.Log("User Request Failed");
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log("User Request Success");
                    var response = JsonConvert.DeserializeObject<UserForm>(request.downloadHandler.text);

                    if(null == GameManager.instance.user)
                    {
                        GameManager.instance.user = response.body;
                    }
                    else
                    {
                        GameManager.instance.user.SetUser(response.body);
                    }
                    GameManager.instance.user.Print();
                }
            }
        }

        public IEnumerator UpdateUser(UserUpdateForm userUpdateForm)
        {

            StringBuilder urlBuilder = new StringBuilder(GameConstants.API_URL)
                .Append("/v1/user");

            string jsonData = JsonConvert.SerializeObject(userUpdateForm);

            using (UnityWebRequest request = UnityWebRequest.Put(urlBuilder.ToString(), jsonData))
            {
                byte[] bytesData = new UTF8Encoding().GetBytes(jsonData);
                request.uploadHandler.Dispose();
                request.uploadHandler = new UploadHandlerRaw(bytesData);
                request.downloadHandler = new DownloadHandlerBuffer();

                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", GameManager.instance.GetAuthorization());

                yield return request.SendWebRequest();

                if (null != request.error)
                {
                    Debug.Log("Update User Request Failed");
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log("Update User Request Success");
                    GameManager.instance.user.UpdateUser(userUpdateForm);
                }
            }
        }
    }
}