using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField, Range(0.25f, 100f)] private float camMulti = 1f;
    private Vector3 cameraBoom;
    // Start is called before the first frame update
    void Start()
    {
        cameraBoom = new Vector3(15, 20, 15);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + (cameraBoom * camMulti);
    }
}
