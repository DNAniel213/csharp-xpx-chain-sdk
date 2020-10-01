using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameBackendConnector : MonoBehaviour
{

    public void RegisterPlayer()
    {
        StartCoroutine(Xarcade.API.RegisterPlayer ("John", "Doe", "JohnDoe@gmail.com", "johndoe", "johnjohndoedoe" , "johnjohndoedoe", true));
    }
    public void LogIn()
    {
        Action<Xarcade.Models.Account> callback = LoginCallback;
        StartCoroutine(Xarcade.API.Login("dnaniel213", "dandandandan", callback));
    }

    public void LoginCallback(Xarcade.Models.Account acc)
    {
        Debug.Log(acc.email);
    }


    public void GetToken()
    {
        Action<Xarcade.Models.Account> callback = LoginCallback;

        StartCoroutine(Xarcade.API.GetToken("cdd96ea0-2df3-43e6-88dd-f16a6800ad2f", "75e7dedb-457b-494f-81cf-53e230f2f0c9", callback));
    }

    public void SendToken()
    {
        
    }

}
