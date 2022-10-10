using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Paramétrages
    private Vector3 offset = new Vector3(0f, 0f, -10f);

    // Cible de la caméra
    [SerializeField]
    private Transform targetTransform;

    // Update is called once per frame
    void Update()
    {
        transform.position = targetTransform.position + offset;
    }
}
