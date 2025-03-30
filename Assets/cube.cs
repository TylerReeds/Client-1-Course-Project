using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;


public class cube : MonoBehaviour
{

    private static Socket UDPClient1; // Make sure this is initialized from your other networking scripts
    private static EndPoint serverEndPoint; // Same for this one

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * 2f,
            0, Input.GetAxis("Vertical") * Time.deltaTime * 2f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin")) // Check if the collided object is a coin
        {
            string message = $"coin_collected,{other.transform.position.x},{other.transform.position.y},{other.transform.position.z}";
            byte[] data = Encoding.ASCII.GetBytes(message);

            // Send the collected coin message to the server
            UDPClient1.SendTo(data, serverEndPoint);

            // Destroy the coin locally (you could also remove it via the server, depending on your setup)
            Destroy(other.gameObject);
        }
    }
}
