using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

// TCP service that computes mean or average of an array

public class TCPService : MonoBehaviour
{
    public SequenceDiagram sequenceDiagram;

    //TODO this will need refactor anyways
    private Stack<string> classStack;
    private JObject fileMap;

    private void Awake()
    {
        classStack = new Stack<string>();
        classStack.Push("Entry");
    }

    private void Start()
    {
        try
        {
            int port = 5000;
            AsyncService service = new AsyncService(port, this);
            service.Run(); // very specific service
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private string MapCall(string file, int lineNo)
    {
        Debug.Log("Mapping: " + file + ":" + lineNo);
        var classes = fileMap.Value<JArray>(file.ToLower());
        foreach (var obj in classes)
        {
            var from = obj.Value<int>("fromLineNo");
            var to = obj.Value<int>("toLineNo");
            if (lineNo >= from && lineNo <= to)
            {
                var qname = obj.Value<string>("qname");
                Debug.Log("Mapped to: " + qname);
                return qname;
            }
        }
        return null;
    }

    public void HandleInitDiagram(JObject data)
    {
        //parse and store mapping
        fileMap = data.Value<JObject>("fileMap");
    }

    public void HandleDrawTrace(JObject data)
    {
        foreach (var item in data.Value<JArray>("calls").Children())
        {
            var name = item.Value<string>("name");
            var file = item.Value<string>("file");
            var lineNo = item.Value<int>("line_no");
            var tevent = item.Value<string>("type");

            var next = MapCall(file, lineNo);

            if (next == null || name == "<module>")
            {
                continue;
            }
            
            if (tevent == "call")
            {
                var from = classStack.Peek();
                var to = next;
                classStack.Push(next);
                sequenceDiagram.AddMessage(from, to, name);
            }
            else if (tevent == "return")
            {
                classStack.Pop();
                var to = classStack.Peek();
                var from = next;
                sequenceDiagram.AddMessage(from, to, tevent, true);
            }
        }
    }

    public async void HandleReceive(string request)
    {
        JObject data = await Task.Run(() =>
        {
            return ParseJson(request);
        });
        Debug.Log("Request: " + request);
        string action = data.Value<string>("action");
        JObject payload = data.Value<JObject>("payload");
        Debug.Log("Action: " + action);
        if (action == "G_CLS_DIA")
        {
            HandleInitDiagram(payload);
        }
        else if (action == "S_DRW_TRC")
        {
            Debug.Log(payload);
            HandleDrawTrace(payload);
        }
    }

    private JObject ParseJson(string json)
    {
        JsonTextReader reader = new JsonTextReader(new StringReader(json));
        return JObject.Load(reader);
    }

}

public class AsyncService
{
    private IPAddress ipAddress;
    private int port;
    private TCPService service;

    public AsyncService(int port, TCPService service)
    {
        // set up port and determine IP Address
        this.port = port;
        this.ipAddress = IPAddress.Parse("127.0.0.1");
        this.service = service;
    } // AsyncService ctor

    // normally the return would be Task but if so, we'd get a compiler warning in Main
    // for not using await, but if we prepend await that's an error because Main returns void.
    // A work-around attempt of making Main return a Task is not allowed. In short, void makes
    // sense in this situation.

    //public async Task Run()
    public async void Run()
    {
        TcpListener listener = new TcpListener(this.ipAddress, this.port);
        listener.Start();

        while (true)
        {
            try
            {
                TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                Task t = Process(tcpClient);
                await t; // could combine with above
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
    } // Start

    private async Task Process(TcpClient tcpClient)
    {
        string clientEndPoint = tcpClient.Client.RemoteEndPoint.ToString();
        Debug.Log("Received connection request from " + clientEndPoint);
        try
        {
            NetworkStream networkStream = tcpClient.GetStream();
            StreamReader reader = new StreamReader(networkStream);
            StreamWriter writer = new StreamWriter(networkStream);
            writer.AutoFlush = true;
            while (true)
            {
                string request = await reader.ReadLineAsync();
                if (request != null)
                {
                    Debug.Log("Received service request: " + request);
                    service.HandleReceive(request);
                    string response = Response(request);
                    await writer.WriteLineAsync(response);
                }
                else
                    break; // client closede connection
            }
            tcpClient.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            if (tcpClient.Connected)
                tcpClient.Close();
        }
    }

    private static string Response(string request)
    {
        //not needed
        string response = "OK\n";
        return response;
    }
}