using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BackButton : MonoBehaviour
{
    public Button backButton;
    void Start()
    {
            backButton.gameObject.SetActive(false);
    }

    public void OnInteractStart()
    {
            backButton.gameObject.SetActive(true);
    }

    public void OnclickBackButton()
    {
        Interactable interactable = new Interactable();
        interactable.InterectEnd();
    }
}