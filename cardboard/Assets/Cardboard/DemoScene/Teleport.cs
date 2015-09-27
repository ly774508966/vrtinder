// Copyright 2014 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Net.Sockets;
using System.Net;
using System.Text;


[RequireComponent(typeof(Collider))]
public class Teleport : MonoBehaviour
{
    private Vector3 startingPosition;
    private Socket socket;
    private Socket handler;

    private string user_id;

    private string x_auth_token;

    void Start()
    {
        startingPosition = transform.localPosition;
        x_auth_token = "{ ENTER YOUR TINDER AUTH TOKEN HERE }";

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //socket.Blocking = false;
        IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
        Debug.Log(ipHostInfo);
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 6612);
        socket.Bind(localEndPoint);
        socket.Listen(1);

        handler = socket.Accept();
        handler.Blocking = false;
        set_new_picture();
    }

    void OnApplicationQuit()
    {
        handler.Close();
    }

    void LateUpdate()
    {
        byte[] bytes;
        string data = null;
        bytes = new byte[1];

        if (handler.Available > 0)
        {
            int bytesRec = handler.Receive(bytes);
            data = Encoding.ASCII.GetString(bytes, 0, bytesRec);

            string url;
            if (data == "L")
            {
                url = "https://api.gotinder.com/pass/" + user_id;
            }
            else
            {
                url = "https://api.gotinder.com/like/" + user_id;
            }
            WWWForm form = new WWWForm();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-Auth-Token", x_auth_token);

            WWW www = new WWW(url, null, headers);
            StartCoroutine(wait_dont_care(www));
            set_new_picture();
            Debug.Log(data);
        }

    }

    IEnumerator wait_dont_care(WWW www)
    {
        yield return www;
    }

    public void Reset()
    {
        transform.localPosition = startingPosition;
    }

    public void ToggleVRMode()
    {
        Cardboard.SDK.VRModeEnabled = !Cardboard.SDK.VRModeEnabled;
    }

    public void TeleportRandomly()
    {
        Vector3 direction = Random.onUnitSphere;
        direction.y = Mathf.Clamp(direction.y, 1f, 1.5f);
        float distance = 2 * Random.value + 1.5f;
        transform.localPosition = direction * distance;
    }

    private void set_new_picture()
    {
        string url = "https://api.gotinder.com/user/recs";
        WWWForm form = new WWWForm();

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("X-Auth-Token", "51c06b2e-20c8-447b-9cc1-04ace019faeb");

        WWW www = new WWW(url, null, headers);
        StartCoroutine(WaitForRequest(www));
    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;
        var N = JSON.Parse(www.text);
        var url = N["results"][0]["photos"][0]["url"];
        user_id = N["results"][0]["_id"];

        WWW www2 = new WWW(url);

        StartCoroutine(wait(www2));
    }

    IEnumerator wait(WWW www)
    {
        yield return www;
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = www.texture;
    }

}
