using System.Collections;
using UnityEngine;
using Cinemachine;

public class OrthoPerspectiveTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    public float orthoCollapseTime = 2f;   // tiempo para colapsar ortho
    public float fovExpandTime = 0.6f;       // tiempo para abrir FOV
    public float targetFOV = 60f;            // FOV final en perspectiva

    private Camera cam;
    private CinemachineBrain brain;
    private bool isTransitioning = false;

    void Awake()
    {
        cam = GetComponent<Camera>();
        brain = GetComponent<CinemachineBrain>();
    }

    void Update()
    {
        // Detecta si el blend ha empezado
        if (brain.IsBlending && !isTransitioning)
        {
            // Si la cámara actual es ortográfica, iniciamos transición
            if (cam.orthographic)
            {
                StartCoroutine(BlendOrthoToPersp());
            }
        }
    }

    IEnumerator BlendOrthoToPersp()
    {
        Debug.Log("Transicionando");
        isTransitioning = true;

        float startSize = cam.orthographicSize;
        float t = 0;

        // 1. Colapsar ortho size suavemente
        while (t < orthoCollapseTime)
        {
            t += Time.deltaTime;
            float k = t / orthoCollapseTime;

            cam.orthographicSize = Mathf.Lerp(startSize, 0.01f, k);
            yield return null;
        }

        // 2. Cambiar a perspectiva (ya no se nota)
        cam.orthographic = false;

        // 3. Abrir FOV suavemente
        t = 0;
        float startFOV = 1f;
        cam.fieldOfView = startFOV;

        while (t < fovExpandTime)
        {
            t += Time.deltaTime;
            float k = t / fovExpandTime;

            cam.fieldOfView = Mathf.Lerp(startFOV, targetFOV, k);
            yield return null;
        }

        isTransitioning = false;
    }
}