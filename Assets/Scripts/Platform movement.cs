
using UnityEngine;

public class Platformmovement : MonoBehaviour
{
    [SerializeField] private GameObject platformToMove;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float speed;
    private Vector3 moveTo;

    private void Start()
    {
        moveTo = endPoint.position;
    }

    private void Update()
    {
        platformToMove.transform.position = Vector3.MoveTowards(platformToMove.transform.position, moveTo, speed*Time.deltaTime);

        if (platformToMove.transform.position == endPoint.position)
        {
            moveTo = startPoint.position;
        }
         if (platformToMove.transform.position == startPoint.position)
        {
            moveTo = endPoint.position;
        }
    }

  
}
