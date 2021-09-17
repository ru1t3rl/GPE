using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

//https://forum.unity.com/threads/simple-udp-implementation-send-read-via-mono-c.15900/

public class NetworkScript : MonoBehaviour
{
    private UdpConnection connection;
    public bool isServer = true;
    int myID;

    bool upKey, downKey, leftKey, rightKey;

    public GameObject playerPrefab;

    public PlayerScript[] players = new PlayerScript[2];


    Thread updatePing;
    private Ping ping;
    private bool threadRunning = false;
    public int pingUpdateInterval = 100;

    string sendIp = "127.0.0.1";
    Sendable sendData = new Sendable();

    void Start()
    {

        int sendPort, receivePort;
        if (isServer)
        {
            sendPort = 8881;
            receivePort = 11000;
            myID = 0;
        }
        else
        {
            sendPort = 11000;
            receivePort = 8881;
            myID = 1;
        }

        ping = new Ping(sendIp);

        connection = new UdpConnection();
        connection.StartConnection(sendIp, sendPort, receivePort);
    }

    void FixedUpdate()
    {
        //Check input...
        if (upKey)
        {
            players[myID].TranslatePlayer(0, players[myID].Speed, 0);
            UpdatePositions(myID);
        }
        if (downKey)
        {
            players[myID].TranslatePlayer(0, -players[myID].Speed, 0);
            UpdatePositions(myID);
        }
        if (leftKey)
        {
            players[myID].TranslatePlayer(-players[myID].Speed, 0, 0);
            UpdatePositions(myID);
        }
        if (rightKey)
        {
            players[myID].TranslatePlayer(players[myID].Speed, 0, 0);
            UpdatePositions(myID);
        }

        //network stuff:
        CheckIncomingMessages();
    }

    private void startPingUpdate()
    {
        updatePing = new Thread(() => { 
            ping = new Ping(sendIp);
            Thread.Sleep(pingUpdateInterval); 
        });
        updatePing.IsBackground = true;
        threadRunning = true;
        updatePing.Start();
    }

    public void Update()
    {
        //handling keyboard (in Update, because FixedUpdate isnt meant for that(!))
        if (Input.GetKeyDown("w")) upKey = true;
        if (Input.GetKeyUp("w")) upKey = false;
        if (Input.GetKeyDown("s")) downKey = true;
        if (Input.GetKeyUp("s")) downKey = false;
        if (Input.GetKeyDown("a")) leftKey = true;
        if (Input.GetKeyUp("a")) leftKey = false;
        if (Input.GetKeyDown("d")) rightKey = true;
        if (Input.GetKeyUp("d")) rightKey = false;
    }

    void CheckIncomingMessages()
    {
        //Do the networkStuff:
        string[] o = connection.getMessages();
        if (o.Length > 0)
        {
            foreach (var json in o)
            {
                JsonUtility.FromJsonOverwrite(json, sendData);
                //now, check its id..
                int i = sendData.id;

                FixIncommingData(ref sendData);

                //..and update the right player_entity:
                players[i].transform.position = new Vector3(sendData.x, sendData.y, 0);
            }
        }

    }

    void FixIncommingData(ref Sendable sendData)
    {
        Vector2 movement = sendData.moveDirection * players[sendData.id].Speed;
        players[sendData.id].TranslatePlayer(movement + movement * sendData.ping / 1000f);
        // 1s = 1000ms
        // 715ms = .715
        // 50ms =  .05
        // Speed = speed + speed * ping/1000
    }

    public void UpdatePositions(int id)
    {
        //update sendData-object
        sendData.id = id;
        sendData.ping = ping.time;
        sendData.x = players[id].transform.position.x;
        sendData.y = players[id].transform.position.y;
        sendData.previousX = players[id].PreviousPosition.x;
        sendData.previousY = players[id].PreviousPosition.y;
        sendData.frame = Time.frameCount;

        string json = JsonUtility.ToJson(sendData); //Convert to String
        Debug.Log(json);

        connection.Send(json); //send the string
    }

    void OnDestroy()
    {
        threadRunning = false;
        updatePing.Abort();
        connection.Stop();
    }
}

