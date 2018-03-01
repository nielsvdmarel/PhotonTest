using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonNetworkManager : Photon.MonoBehaviour
{
    [SerializeField]
    private Text _connectText;
    [SerializeField]
    private GameObject _player;
    [SerializeField]
    private GameObject _lobbyCamera;
    [SerializeField]
    private Transform _spawnPoints;
	void Start ()
    {
        PhotonNetwork.ConnectUsingSettings("Version 0.1");
	}

    public virtual void OnJoinedLobby()
    {
        Debug.Log(" We have now Joined the lobby!!");
        // join room if it exists or create one if it doesn't exists
        PhotonNetwork.JoinOrCreateRoom("Niels Lobby", null, null);
    }

    public virtual void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate(_player.name, _spawnPoints.position, _spawnPoints.rotation, 0);
        _lobbyCamera.SetActive(false);
    }
    
    void Update ()
    {
        _connectText.text = PhotonNetwork.connectionStateDetailed.ToString(); 
	}
}
