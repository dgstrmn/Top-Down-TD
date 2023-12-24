using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isPlacingObject = false;
    public DragObject heldObject;

    void Update()
    {
        if(isPlacingObject && heldObject.isPlaced)
        {
            isPlacingObject = false;
        }
    }
}
