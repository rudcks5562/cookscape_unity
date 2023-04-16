using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor;
using static UnityProject.Cookscape.AvatarData;

namespace UnityProject.Cookscape.Api
{
    public class Information : MonoBehaviour
    {
        public static Information instance = null;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if(instance != this)
                {
                    Destroy(gameObject);
                }
            }
        }

        private void Start()
        {
            StartCoroutine(GetVersion());
        }

        public IEnumerator GetVersion()
        {
            StringBuilder urlBuilder = new StringBuilder(GameConstants.API_URL)
                .Append("/v1/version/latest");
                
            using (UnityWebRequest request = UnityWebRequest.Get(urlBuilder.ToString()))
            {
                yield return request.SendWebRequest();

                if (null != request.error)
                {
                    Debug.Log("Version Request Failed");
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log("Version Request Success");
                    var response = JsonConvert.DeserializeObject<VersionForm>(request.downloadHandler.text);

                    if(null == GameManager.instance.version)
                    {
                        GameManager.instance.version = new VersionData();
                    }
                    GameManager.instance.version.version = response.body;
                }
            }
        }

        public IEnumerator GetItem()
        {
            StringBuilder urlBuilder = new StringBuilder(GameConstants.API_URL)
                .Append("/v1/information/item");

            using (UnityWebRequest request = UnityWebRequest.Get(urlBuilder.ToString()))
            {
                request.SetRequestHeader("Authorization", GameManager.instance.GetAuthorization());
                yield return request.SendWebRequest();

                if (null != request.error)
                {
                    Debug.Log("Item Request Failed");
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log("Item Request Success");
                    var response = JsonConvert.DeserializeObject<ItemForm>(request.downloadHandler.text);

                    if(null == GameManager.instance.item)
                    {
                        GameManager.instance.item = new Dictionary<string, ItemData>();
                    }

                    foreach(ItemData item in response.body)
                    {
                        GameManager.instance.item.TryAdd(item.name, item);
                    }
                }
            }
        }

        public IEnumerator GetObject()
        {
            StringBuilder urlBuilder = new StringBuilder(GameConstants.API_URL)
                .Append("/v1/information/object");

            using (UnityWebRequest request = UnityWebRequest.Get(urlBuilder.ToString()))
            {
                request.SetRequestHeader("Authorization", GameManager.instance.GetAuthorization());
                yield return request.SendWebRequest();

                if (null != request.error)
                {
                    Debug.Log("Object Request Failed");
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log("Object Request Success");
                    var response = JsonConvert.DeserializeObject<ObjectForm>(request.downloadHandler.text);

                    if(null == GameManager.instance.mapObject)
                    {
                        GameManager.instance.mapObject = new Dictionary<string, ObjectData>();
                    }

                    foreach(ObjectData mapObject in response.body)
                    {
                        GameManager.instance.mapObject.TryAdd(mapObject.name, mapObject);
                    }
                }
            }
        }

        public IEnumerator GetAvatar()
        {
            StringBuilder urlBuilder = new StringBuilder(GameConstants.API_URL)
                .Append("/v1/information/avatar");

            using (UnityWebRequest request = UnityWebRequest.Get(urlBuilder.ToString()))
            {
                request.SetRequestHeader("Authorization", GameManager.instance.GetAuthorization());
                yield return request.SendWebRequest();

                if (null != request.error)
                {
                    Debug.Log("Avatar Request Failed");
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log("Avatar Request Success");
                    var response = JsonConvert.DeserializeObject<AvatarForm>(request.downloadHandler.text);

                    if(null == GameManager.instance.avatar)
                    {
                        GameManager.instance.avatar = new Dictionary<string, AvatarData>();
                    }

                    foreach (AvatarData avatar in response.body)
                    {
                        GameManager.instance.avatar.TryAdd(avatar.name, avatar);
                    }
                }
            }
        }

        public IEnumerator GetReward()
        {
            StringBuilder urlBuilder = new StringBuilder(GameConstants.API_URL)
                .Append("/v1/information/reward");

            using (UnityWebRequest request = UnityWebRequest.Get(urlBuilder.ToString()))
            {
                request.SetRequestHeader("Authorization", GameManager.instance.GetAuthorization());
                yield return request.SendWebRequest();

                if (null != request.error)
                {
                    Debug.Log("Reward Request Failed");
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log("Reward Request Success");
                    var response = JsonConvert.DeserializeObject<RewardForm>(request.downloadHandler.text);

                    if (null == GameManager.instance.reward)
                    {
                        GameManager.instance.reward = new Dictionary<string, RewardData>();
                    }

                    foreach (RewardData reward in response.body)
                    {
                        GameManager.instance.reward.TryAdd(reward.keyValue, reward);
                    }
                }
            }
        }

        public IEnumerator GetChallenge()
        {
            StringBuilder urlBuilder = new StringBuilder(GameConstants.API_URL)
                .Append("/v1/information/challenge");

            using (UnityWebRequest request = UnityWebRequest.Get(urlBuilder.ToString()))
            {
                request.SetRequestHeader("Authorization", GameManager.instance.GetAuthorization());
                yield return request.SendWebRequest();

                if (null != request.error)
                {
                    Debug.Log("challenge Request Failed");
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log("challenge Request Success");
                    var response = JsonConvert.DeserializeObject<ChallengeForm>(request.downloadHandler.text);

                    if (null == GameManager.instance.challenge)
                    {
                        GameManager.instance.challenge = new Dictionary<string, ChallengeData>();
                    }

                    foreach (ChallengeData challenge in response.body)
                    {
                        GameManager.instance.challenge.TryAdd(challenge.keyValue, challenge);
                    }

                }
            }
        }
    }
}