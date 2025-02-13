using System.Collections;
using UnityEngine;

/*
 * This script will allow any game object to be moved smoothly
 * from its current position to a new target position at a speed
 * set in the inspector using a coroutine.
 * 
 * The speed must be a positive number.
 * 
 * You can see if the object is currently moving using Idle
 * 
 * There is an Easing function to alter the speed of the animation over time
 */
public class Movable : MonoBehaviour
{
    private Vector3 from,
                    to;

    private float howfar;

    protected bool idle = true;

    public bool Idle
    {
        get
        {
            return idle;
        }
    }


    private float speed = 3;

    public float Speed
    {
        get
        {
            return speed;
        }
    }


    public IEnumerator MoveToPosition(Vector3 targetPosition, bool bounce = true)
    {

        if (speed <= 0)
            Debug.LogWarning("Speed must be a positive number.");
        if (!bounce)
        {
            speed = 1;
        }
        from = transform.position;
        to = targetPosition;
        howfar = 0;
        idle = false;

        do
        {
            howfar += speed * Time.deltaTime;

            if (howfar > 1)
                howfar = 1;
            if (bounce)
            {
                transform.position = Vector3.LerpUnclamped(from, to, EasingBounce(howfar));
            }
            else
            {
                transform.position = Vector3.LerpUnclamped(from, to, Easing(howfar));
            }

            yield return null;
        }
        while (howfar != 1);

        idle = true;
        speed = 3;
    }

    public IEnumerator MoveToTransform(Transform target)
    {
        if (speed <= 0)
            Debug.LogWarning("Speed must be a positive number.");

        speed = 2.5f;

        from = transform.position;
        to = target.position;
        howfar = 0;
        idle = false;

        do
        {
            howfar += speed * Time.deltaTime;

            if (howfar > 1)
                howfar = 1;

            to = target.position;
            transform.position = Vector3.LerpUnclamped(from, to, EaseInOutBack(howfar));

            yield return null;
        }
        while (howfar != 1);

        idle = true;
        speed = 3;
    }
    private float Easing(float t)
    {
        float c1 = 1.70158f,
                c2 = c1 * 1.525f;

        return t < 0.5f
            ? (Mathf.Pow(t * 2, 2) * ((c2 + 1) * 2 * t - c2)) / 2
            : (Mathf.Pow(t * 2 - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2
            ;
    }

    private float EasingBounce(float t)
    {
        float n1 = 4f;

        if (t < 0.5f)
        {
            // First half of the easing
            return n1 * t * t;
        }
        else
        {
            // Second half with a bounce effect

            float bounceT = t - 0.5f; // Shift the range to [0, 0.5] for the bounce
            return 1 - (1.1f * bounceT * (0.5f - bounceT)); // A simple parabola for the bounce
        }
    }

    private float EaseInOutBack(float x)
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;

        return x < 0.5
          ? (Mathf.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
          : (Mathf.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
    }





}