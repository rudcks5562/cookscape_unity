using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

//public class Information
//{
//    public string timeStamp;
//    public string status;
//    public string msg;
//    public Dictionary<string, List<object>> data;
//}

//public class Item
//{
//    public long itemId { get; set; }
//    public string name { get; set; }
//    public string desc { get; set; }
//    public int weight { get; set; }
//}

//public class Avatar
//{
//    public long avatarId { get; set; }
//    public string name { get; set; }
//    public string desc { get; set; }
//    public float speed { get; set; }
//    public float jump { get; set; }
//}

public class HttpNetworkTest : MonoBehaviour
{
    public string url = "localhost:9999/api";

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetRequest());
    }

    private IEnumerator GetRequest()
    {
        Debug.Log("============ GET Request start! ============");
        url += "/v1/information/all";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if(request.error != null)
            {
                Debug.Log("GET Request Failure");
                Debug.Log(request.error);
            }
            else
            {
                //Debug.Log("GET Request Success");
                //string jsonString = request.downloadHandler.text;
                //Debug.Log(jsonString);

                //var test = JsonToObject<Information>(jsonString);

                //foreach(var list in test.data)
                //{
                //    Debug.Log(list.Value[0]
                //        );
                //}
                //Information information = JsonConvert.DeserializeObject<Information>(jsonString);

                //Information information = JsonSerializer.Deserialize<Information>(jsonString);

                //Debug.Log($"information{information.itemInfos}");
                //foreach(Item i in information.itemInfos)
                //{
                //    Debug.Log(i.name);
                //}


                //Debug.Log("CONVERT LIST");
                //Items itemList = JsonUtility.FromJson<Items>(request.downloadHandler.text);

                //Debug.Log($"{itemList.itemInfos[0]}");
                //foreach(Item i in itemList.itemInfos)
                //{
                //    Debug.Log(i);
                //    Debug.Log("================================");

                //}
            }
        }
    }

    string ObjectToJson(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    T JsonToObject<T>(string jsonData)
    {
        return JsonConvert.DeserializeObject<T>(jsonData);
    }
}
