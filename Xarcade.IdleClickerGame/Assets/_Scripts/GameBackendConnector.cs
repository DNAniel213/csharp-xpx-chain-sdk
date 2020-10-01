using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameBackendConnector : MonoBehaviour
{
    public void LogIn()
    {
        Action<Xarcade.Models.Account> callback = LoginCallback;
        StartCoroutine(Xarcade.API.Login("dnaniel213", "dandandandan", callback));
    }

    public void LoginCallback(Xarcade.Models.Account acc)
    {
        Debug.Log("PITI");
        Debug.Log(acc.email);
    }

}
