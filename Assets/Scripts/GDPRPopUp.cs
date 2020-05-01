using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GDPRPopUp : MonoBehaviour
{
    public void GrantConsent()
    {
        VoodooSauce.GrantConsent();
        InitSceneController.instance.GoToMainScene();
    }

    public void RevokeConsent()
    {
        VoodooSauce.RevokeConsent();
        InitSceneController.instance.GoToMainScene();
    }
    
}
