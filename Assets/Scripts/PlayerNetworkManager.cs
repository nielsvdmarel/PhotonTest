using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkManager : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private MonoBehaviour[] playerControlScripts;

    private PhotonView photonview;

    private void Start()
    {
        photonview = GetComponent<PhotonView>();

        Initialize();
    }

    private void Initialize()
    {
        if (photonview.isMine)
        {
            //mine
        }
        // handle shit for disab other people yo
        else
        {
            //disab 
            playerCamera.gameObject.SetActive(false);
            foreach (var m in playerControlScripts)
            {
                m.enabled = false;
            }
        }
    }
}

	
