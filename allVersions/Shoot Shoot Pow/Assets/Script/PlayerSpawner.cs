using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;
    [SerializeField] GameObject playerPrefab;
    GameObject player;

    private void Awake()
    {
        instance = this;
    }

    

    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
        
    }

    public void SpawnPlayer()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            player = PhotonNetwork.Instantiate(playerPrefab.name,new Vector3(0f,0f,0f),Quaternion.identity);
        }
        else
        {
            player = PhotonNetwork.Instantiate(playerPrefab.name,new Vector3(4f,0f,4f),Quaternion.identity);
        }

    }
}
