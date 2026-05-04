using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class AutoClosePanel : MonoBehaviour
{
    public TMP_Text errorTxtBody;
    private void OnEnable()
    {
        //StartCoroutine(AutoOffPanel());
    }

    // private IEnumerator AutoOffPanel()
    // {
    //     yield return new WaitForSeconds(3);
    //     gameObject.SetActive(false);
    //     errorTxtBody.text = "Disconnected from room!";
    // }

    public void OffErrorPanelBtn()
    {
        gameObject.SetActive(false);
        errorTxtBody.text = "Disconnected from room!";
    }
}
