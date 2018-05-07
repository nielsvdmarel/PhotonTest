using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {
    [SerializeField]
    private bool MenuIsActivated = false;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject MainCameraRotation;
    [SerializeField]
    private GameObject UiCameraFollow;
    [SerializeField]
    private GameObject MainCameraFollow;
    [SerializeField]
    private GameObject Player;
    [SerializeField]
    private GameObject HealthUI;
	void Start ()
    {
        pauseMenu.active = false;
        MenuIsActivated = false;
	}
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!MenuIsActivated)
            {
                StartCoroutine(OpenUITimer());
            }

            if (MenuIsActivated)
            {
                StartCoroutine(CloseUITimer());
            }
        }
	}


   public IEnumerator OpenUITimer()
    {
        HealthUI.SetActive(false);
        SetPlayerMovementStop();
        MainCameraRotation.GetComponent<CameraFollow>().CameraMoveSpeed = 10;
        MainCameraRotation.GetComponent<CameraFollow>().CameraFollowObject = UiCameraFollow;
        yield return new WaitForSeconds(.2f);
        MainCameraRotation.GetComponent<CameraFollow>().enabled = false;
        pauseMenu.active = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        MenuIsActivated = true;
    }

   public IEnumerator CloseUITimer()
    {
        pauseMenu.active = false;
        MainCameraRotation.GetComponent<CameraFollow>().CameraMoveSpeed = 10;
        MainCameraRotation.GetComponent<CameraFollow>().CameraFollowObject = MainCameraFollow;
        MainCameraRotation.GetComponent<CameraFollow>().enabled = true;
        yield return new WaitForSeconds(.2f);
        HealthUI.SetActive(true);
        MainCameraRotation.GetComponent<CameraFollow>().CameraMoveSpeed = 120;
        MainCameraRotation.GetComponent<CameraFollow>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        MenuIsActivated = false;
        Player.GetComponent<MovementInput>().enabled = true;
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void Settings()
    {
        Debug.Log("Settings");
    }

    public void CloseUIFunc()
    {
        Debug.Log("wtf");
        StartCoroutine(CloseUITimer());

    }

    public void SetPlayerMovementStop()
    {
        Player.GetComponent<MovementInput>().InputX = 0;
        Player.GetComponent<MovementInput>().InputZ = 0;
        Player.GetComponent<MovementInput>().enabled = false;
    }
}
