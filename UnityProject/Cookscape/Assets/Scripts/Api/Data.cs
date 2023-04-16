using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityProject.Cookscape.Api
{
    public class Data : MonoBehaviour
    {
        public static Data instance = null;

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

        public IEnumerator GetData()
        {
            StringBuilder urlBuilder = new StringBuilder(GameConstants.API_URL)
                .Append("/v1/data/user-data");

            using (UnityWebRequest request = UnityWebRequest.Get(urlBuilder.ToString()))
            {
                request.SetRequestHeader("Authorization", GameManager.instance.GetAuthorization());
                yield return request.SendWebRequest();

                if (null != request.error)
                {
                    Debug.Log("USER DATA Request Failed");
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log("USER DATA Request Success");
                    var response = JsonConvert.DeserializeObject<DataForm>(request.downloadHandler.text);

                    if (null == GameManager.instance.userData)
                    {
                        GameManager.instance.userData = response.body;
                    }
                    else
                    {
                        GameManager.instance.userData.SetUserData(response.body);
                    }
                }
            }
        }

        public IEnumerator GetUsageAvatar()
        {
            StringBuilder urlBuilder = new StringBuilder(GameConstants.API_URL)
                .Append("/v1/data/usage-avatar");

            using (UnityWebRequest request = UnityWebRequest.Get(urlBuilder.ToString()))
            {
                request.SetRequestHeader("Authorization", GameManager.instance.GetAuthorization());
                yield return request.SendWebRequest();

                if (null != request.error)
                {
                    Debug.Log("USAGE AVATAR Request Failed");
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log("USAGE AVATAR Request Success");
                    var response = JsonConvert.DeserializeObject<UsageAvatarForm>(request.downloadHandler.text);

                    if (null == GameManager.instance.usageAvatar)
                    {
                        GameManager.instance.usageAvatar = new Dictionary<string, UsageAvatarData>();
                    }

                    foreach(UsageAvatarData usageAvatar in response.body)
                    {
                        if(!GameManager.instance.usageAvatar.TryAdd(usageAvatar.name, usageAvatar))
                        {
                            GameManager.instance.usageAvatar[usageAvatar.name].SetUsageAvatarData(usageAvatar);
                        }
                    }
                }
            }
        }

        public IEnumerator UpdateGameResult(GameResultForm gameResultForm, string avatarName)
        {
            StringBuilder urlBuilder = new StringBuilder(GameConstants.API_URL)
                .Append("/v1/data/result");

            string jsonData = JsonConvert.SerializeObject(gameResultForm);

            using(UnityWebRequest request = UnityWebRequest.Put(urlBuilder.ToString(), jsonData))
            {
                byte[] bytesData = new UTF8Encoding().GetBytes(jsonData);
                request.uploadHandler.Dispose();
                request.uploadHandler = new UploadHandlerRaw(bytesData);
                request.downloadHandler = new DownloadHandlerBuffer();

                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", GameManager.instance.GetAuthorization());

                yield return request.SendWebRequest();

                if(null != request.error)
                {
                    Debug.Log("Game Result Request Failed");
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log("Game Result Request Success");
                    GameManager.instance.userData.AddGameResultData(gameResultForm);
                    GameManager.instance.usageAvatar[avatarName].AddUsage();
                }
            }
        }

        public IEnumerator RegistReward(RewardData reward)
        {
            StringBuilder urlBuilder = new StringBuilder(GameConstants.API_URL)
                .Append("/v1/data/rewards/")
                .Append(reward.rewardId);
            string jsonData = "Add Reward";
            using(UnityWebRequest request = UnityWebRequest.Post(urlBuilder.ToString(), jsonData))
            {
                request.uploadHandler.Dispose();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", GameManager.instance.GetAuthorization());

                yield return request.SendWebRequest();

                if(null != request.error)
                {
                    Debug.Log("Regist Reward Request Failed");
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log("Regist Reward Request Success");
                    GameManager.instance.userHaveReward.TryAdd(reward.keyValue, reward);
                    GameManager.instance.userNotHaveReward.Remove(reward.keyValue);
                }
            }
        }

        public IEnumerator GetReward(bool isPossession)
        {
            StringBuilder urlBuilder = new StringBuilder(GameConstants.API_URL)
                .Append("/v1/data/rewards")
                .Append("?isPossession=")
                .Append(isPossession);

            using (UnityWebRequest request = UnityWebRequest.Get(urlBuilder.ToString()))
            {
                request.SetRequestHeader("Authorization", GameManager.instance.GetAuthorization());
                yield return request.SendWebRequest();

                if (null != request.error)
                {
                    Debug.Log("User Reward Request Failed");
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log("User Reward Request Success");
                    var response = JsonConvert.DeserializeObject<RewardForm>(request.downloadHandler.text);

                    if (isPossession)
                    {
                        if(null == GameManager.instance.userHaveReward)
                        {
                            GameManager.instance.userHaveReward = new Dictionary<string, RewardData>();
                        }
                        else
                        {
                            GameManager.instance.userHaveReward.Clear();
                        }

                        foreach(RewardData reward in response.body)
                        {
                            GameManager.instance.userHaveReward.TryAdd(reward.keyValue, reward);
                        }
                    }
                    else
                    {
                        if (null == GameManager.instance.userNotHaveReward)
                        {
                            GameManager.instance.userNotHaveReward = new Dictionary<string, RewardData>();
                        }
                        else
                        {
                            GameManager.instance.userNotHaveReward.Clear();
                        }

                        foreach (RewardData reward in response.body)
                        {
                            GameManager.instance.userNotHaveReward.TryAdd(reward.keyValue, reward);
                        }
                    }
                }
            }
        }
    }
}
