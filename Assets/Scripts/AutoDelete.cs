using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDelete : MonoBehaviour
{
    public int lifeTime;
    private float time;

    private void Update()
    {
        time += 1 * Time.deltaTime;

        if(time > lifeTime)
            Destroy(gameObject);
    }
}
