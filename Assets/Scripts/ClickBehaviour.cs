using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ClickBehaviour : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private GameObject gameManager;
    private GameManager gameManagerInstance;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerInstance = gameManager.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!gameManagerInstance.isPlacingObject)
        {
            gameManagerInstance.isPlacingObject = true;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 300f, LayerMask.NameToLayer("UI")))
            {
                if (hit.collider.transform.CompareTag("Ground"))
                {
                    Vector3 spawnPosition = hit.point;
                    gameManagerInstance.heldObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity).GetComponent<DragObject>();
                }
            }
        }
    }
}
