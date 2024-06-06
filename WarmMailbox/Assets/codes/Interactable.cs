using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour
{
    Camera mainCamera;
    float cameraSpeed = 2.5f;

    private void Start()
    {
        mainCamera = Camera.main;
    }
    public void InterectEnd()
    {
        {
            CameraMove(new Vector3(0f, 1.5f, 0f), new Vector3(10f, 0f, 0f));
        }
    }
    public void Interact()
    {
        BackButton backButton = new BackButton();
        backButton.OnInteractStart();
    }

    public void monitor1()
    {
        CameraMove(new Vector3(0.204f, 1.36f, 1.203f), new Vector3(0f, 25f, 0f));
    }

    public void monitor2()
    {
        CameraMove(new Vector3(-0.269f, 1.36f, 1.219f), (new Vector3(0f, -20.547f, 0f)));
    }

    public void note()
    {
        CameraMove(new Vector3(-0.617f, 1.18f, 1.231f), new Vector3(86.395f, -27.914f, 0f));
    }
    public void photoframe()
    {
        CameraMove(new Vector3(-0.715f, 1.184f, 0.956f), new Vector3(23f, -31.4f, 0f));
    }
    void CameraMove(Vector3 targetPosition, Vector3 targetRotation)
    {
        StartCoroutine(SmoothMove(mainCamera.transform.position, targetPosition, cameraSpeed, newPosition =>
        {
            mainCamera.transform.position = newPosition;
        }));

        StartCoroutine(SmoothRota(mainCamera.transform.rotation, Quaternion.Euler(targetRotation), cameraSpeed, newRotation =>
        {
            mainCamera.transform.rotation = newRotation;
        }));
    }

    IEnumerator SmoothMove(Vector3 start, Vector3 end, float speed, System.Action<Vector3> onUpdate)
    {
        float t = 0f;
        while (t < 3f)
        {
            t += Time.deltaTime * speed;
            onUpdate(Vector3.Lerp(start, end, t));
            yield return null;
        }
    }
    IEnumerator SmoothRota(Quaternion start, Quaternion end, float speed, System.Action<Quaternion> onUpdate)
    {
        float t = 0f;
        while (t < 3f)
        {
            t += Time.deltaTime * speed;
            onUpdate(Quaternion.Slerp(start, end, t));
            yield return null;
        }
    }
}