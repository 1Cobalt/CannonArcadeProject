using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    public float speed = 0.2f;

    void Update()
    {
        // Просто ползем вниз
        transform.Translate(Vector2.down * speed * Time.deltaTime);

        // Если улетели слишком низко — уничтожаем (чтобы не засорять память)
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
}
