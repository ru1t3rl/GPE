using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using TMPro;
using System.Drawing;
using DG.Tweening;

//https://forum.unity.com/threads/simple-udp-implementation-send-read-via-mono-c.15900/

public class NetworkScript : MonoBehaviour
{
    private UdpConnection connection;
    public bool isServer = true;
    int myID;

    bool upKey, downKey, leftKey, rightKey;

    public GameObject playerPrefab;

    public PlayerScript[] players = new PlayerScript[2];

    long delay, currentTimeStamp;
    bool receivedInitial = false;

    [Header("Ping")]
    Coroutine updatePing;
    private Ping ping;
    private bool threadRunning = false;
    public int pingUpdateInterval = 100;
    public int maxPing;
    [SerializeField] TextMeshProUGUI text;
    bool shouldUpdatePing = true;

    [SerializeField, Range(0, 100)]
    [Tooltip("Percentage from which to start with extrapolation")]
    int enableExtrpolatePercentage;
    Tweener tweenerX, tweenerY;

    [Header("Testing")]
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
            players[myID].id = myID;
        }
        else
        {
            sendPort = 11000;
            receivePort = 8881;
            myID = 1;
        }

        pingUpdateInterval = maxPing > pingUpdateInterval ? maxPing : pingUpdateInterval;

        ping = new Ping(sendIp);

        connection = new UdpConnection();
        connection.StartConnection(sendIp, sendPort, receivePort);

        StartPingUpdate();
        StartCoroutine(HandShake());
    }

    IEnumerator HandShake()
    {
        text.text = "Connecting...";
        players.ToList().ForEach(p =>
        {
            UpdatePositions(p.id);

            do
            {
                CheckIncomingMessages();
            } while (!receivedInitial);

            receivedInitial = false;
        });

        yield return null;
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

    private void StartPingUpdate()
    {
        updatePing = StartCoroutine(UpdatePing());
    }

    IEnumerator UpdatePing()
    {
        while (true)
        {
            ping = new Ping(sendIp);

            yield return new WaitForSeconds(pingUpdateInterval / 1000f);

            UpdateGUI();
        }
    }

    void UpdateGUI()
    {
        text.text = $"{ping.time}ms";
        if (ping.time <= maxPing * .25)
            text.color = new Color(0, 200, 0);
        else if (ping.time <= maxPing * .5)
            text.color = new Color(243, 247, 0);
        else if (ping.time <= maxPing * .75)
            text.color = new Color(235, 149, 52);
        else if (ping.time <= maxPing)
            text.color = new Color(255, 0, 0);
        else
        {
            text.color = new Color(255, 0, 0);
            text.text = "disconnected";
            OnDestroy();
            Debug.Log("<b>[Newtwork Manager]</b> Connnection stopped: ping was to high");
        }
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

                //..and update the right player_entity:
                InterpolatePosition(sendData);
                delay = (System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond) - sendData.timeStamp;

                if (!receivedInitial)
                    receivedInitial = true;
            }
        }

    }

    public void InterpolatePosition(Sendable sendData)
    {
        if (tweenerX != null && tweenerX.IsActive())
            tweenerX.Kill();
        if (tweenerY != null && tweenerY.IsActive())
            tweenerY.Kill();

        if (ping.time * 1f / maxPing > enableExtrpolatePercentage)
        {
            tweenerX = players[sendData.id].transform.DOMoveX((players[sendData.id].transform.position.x + sendData.previousX + sendData.x) / 3f, ping.time / 1000f / 8f);
            tweenerY = players[sendData.id].transform.DOMoveY((players[sendData.id].transform.position.y + sendData.previousY + sendData.y) / 3f, ping.time / 1000f / 8f);
        }
        else
        {
            players[sendData.id].transform.position = new Vector3(
                (players[sendData.id].transform.position.x + sendData.previousX + sendData.x) / 3f,
                (players[sendData.id].transform.position.y + sendData.previousY + sendData.y) / 3f,
                0
            );
        }
    }

    public void ExtrapolatePosition(ref Sendable sendData)
    {
        Vector2 movement = sendData.moveDirection * players[sendData.id].Speed;
        currentTimeStamp = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
        sendData.x += movement.x + movement.x * ((currentTimeStamp - sendData.timeStamp) / 1000f);
        sendData.x += movement.y + movement.y * ((currentTimeStamp - sendData.timeStamp) / 1000f);
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
        sendData.timeStamp = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;

        if (ping.time * 1f / maxPing >= enableExtrpolatePercentage / 100f)
            ExtrapolatePosition(ref sendData);

        string json = JsonUtility.ToJson(sendData); //Convert to String

        connection.Send(json); //send the string
    }

    void OnDestroy()
    {
        if (updatePing != null)
            StopCoroutine(updatePing);
        connection.Stop();
    }
}

