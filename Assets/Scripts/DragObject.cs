using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class DragObject : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    public bool isPlaced = false;
    LayerMask dragObjectMask;
    bool okayToPlace = false;
    Vector3 halfExtends;

    void Start()
    {
        halfExtends = new Vector3(transform.localScale.x, transform.localScale.y * (6 / transform.localScale.y), transform.localScale.x);
        dragObjectMask = LayerMask.NameToLayer("DragObject");
        transform.GetComponent<Collider>().enabled = false;
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        // Store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
    }
    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    public void Update()
    {
        if (!isPlaced)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 300f, dragObjectMask))
            {
                if (hit.collider.transform.CompareTag("Ground"))
                {
                    transform.position = hit.point + mOffset + new Vector3(0, 2.5f, 0);
                }
            }
        }
        if(Input.GetMouseButton(0) && okayToPlace)
        {
            isPlaced = true;
            transform.GetComponent<Collider>().enabled = true;
            GetComponent<Renderer>().material.color = Color.gray;
        }
    }

    void FixedUpdate()
    {
        if (!isPlaced)
        {
            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, halfExtends, Quaternion.identity);
            if (hitColliders.Length < 2)
            {
                GetComponent<Renderer>().material.color = Color.green;
                okayToPlace = true;
            }
            else
            {
                GetComponent<Renderer>().material.color = Color.red;
                okayToPlace = false;
            }
            if (Input.GetMouseButton(1))
            {
                isPlaced = true;
                Destroy(gameObject);
            }
        }
    }
}