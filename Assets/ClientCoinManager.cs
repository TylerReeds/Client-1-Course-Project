using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;

public class ClientCoinManager : MonoBehaviour
{
    public GameObject coinPrefab; 

    private static Socket UDPClient1;
    private static EndPoint serverEndPoint;
    private static byte[] UDPBuffer = new byte[1024];

    void Start()
    {
        UDPClient1 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        UDPClient1.Bind(new IPEndPoint(IPAddress.Any, 0)); 

        serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8889);

        // Start receiving coin data from the server
        UDPClient1.BeginReceiveFrom(UDPBuffer, 0, UDPBuffer.Length, 0, ref serverEndPoint, new AsyncCallback(ReceiveUDPCallback), UDPClient1);
    }

    private void ReceiveUDPCallback(IAsyncResult result)
    {
        int receivedDataLength = UDPClient1.EndReceiveFrom(result, ref serverEndPoint);

        if (receivedDataLength > 0)
        {
            string message = Encoding.ASCII.GetString(UDPBuffer, 0, receivedDataLength);
            string[] messageParts = message.Split(',');

            if (messageParts[0] == "coin_spawn")
            {
                float x = float.Parse(messageParts[1]);
                float y = float.Parse(messageParts[2]);
                float z = float.Parse(messageParts[3]);

                Vector3 spawnPosition = new Vector3(x, y, z);

                // Instantiate the coin prefab at the received position
                InstantiateCoin(spawnPosition);
            }
        }

        // Continue receiving data
        UDPClient1.BeginReceiveFrom(UDPBuffer, 0, UDPBuffer.Length, 0, ref serverEndPoint, new AsyncCallback(ReceiveUDPCallback), UDPClient1);
    }

    public void InstantiateCoin(Vector3 position)
    {
        // Create a new coin at the specified position
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, position, Quaternion.identity);
            Debug.Log($"Coin spawned at: {position}");
        }
        else
        {
            Debug.LogError("Coin Prefab is not assigned!");
        }
    }
}
