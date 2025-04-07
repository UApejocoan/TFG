using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float tileSize = 1f;
    private bool isMoving = false;
    private Vector3 targetPosition;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float minSwipeDistance = 50f; // Ajusta este valor según tu preferencia

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        if (!isMoving)
        {
            DetectSwipe();
        }
    }

    void DetectSwipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    break;

                case TouchPhase.Ended:
                    endTouchPosition = touch.position;
                    Vector2 swipeDelta = endTouchPosition - startTouchPosition;

                    if (swipeDelta.magnitude >= minSwipeDistance)
                    {
                        Vector3 direction = Vector3.zero;

                        // Determinar si fue un swipe más horizontal o vertical
                        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                        {
                            // Swipe horizontal (Z)
                            if (swipeDelta.x > 0)
                                direction = new Vector3(tileSize, 0, 0); // Derecha
                            else
                                direction = new Vector3(-tileSize, 0, 0); // Izquierda
                        }
                        else
                        {
                            // Swipe vertical (Y)
                            if (swipeDelta.y > 0)
                                direction = new Vector3(0, tileSize, 0); // Arriba
                            else
                                direction = new Vector3(0, -tileSize, 0); // Abajo
                        }

                        if (direction != Vector3.zero)
                        {
                            targetPosition = transform.position + direction;
                            StartCoroutine(MoveToTarget());
                        }
                    }
                    break;
            }
        }
    }

    private IEnumerator MoveToTarget()
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }
}
