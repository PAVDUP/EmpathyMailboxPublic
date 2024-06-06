using UnityEngine;
using System.Collections;
public class RaycastExample : MonoBehaviour
{
    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit object: " + hit.collider.gameObject.name);
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    if(hit.collider.gameObject.name == "monitor1")
                    {
                        interactable.monitor1();
                    }
                    if(hit.collider.gameObject.name == "monitor2")
                    {
                        interactable.monitor2();
                    }
                    if (hit.collider.gameObject.name == "note")
                    {
                        interactable.note();
                    }
                    if (hit.collider.gameObject.name == "photoframe")
                    {
                        interactable.photoframe();
                    }
                    interactable.Interact();
                }
            }
        }
    }
}
