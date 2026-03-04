using UnityEngine;
using System.Collections.Generic;

public class SnakeMovement : MonoBehaviour
{
    public Transform[] path;
    public int currentTargetIndex = 1;
    public bool isBroken = false;

    public void Initialize(Transform[] waypoints)
    {
        path = waypoints;
    }

    void Update()
    {
        if (SnakeSpawner.Instance == null) return;
        List<SnakeMovement> chain = SnakeSpawner.Instance.snakeChain;

        int myIndex = chain.IndexOf(this);
        if (myIndex == -1) return;

        // Если за нами кто-то есть...
        if (myIndex < chain.Count - 1)
        {
            SnakeMovement neighborBehind = chain[myIndex + 1];

            if (neighborBehind != null)
            {
                // ЦЕПНАЯ РЕАКЦИЯ: Если задний откатывается, я тоже должен
                if (neighborBehind.isBroken) isBroken = true;

                // ЛОГИКА СЦЕПКИ
                if (isBroken)
                {
                    float dist = Vector3.Distance(transform.position, neighborBehind.transform.position);

                    // БЕРЕМ ИДЕАЛЬНУЮ ДИСТАНЦИЮ ИЗ СПАВНЕРА
                    float targetGap = SnakeSpawner.Instance.maintainedGap;

                    // Если мы приблизились к заднему на нужное расстояние (или ближе)
                    if (dist <= targetGap)
                    {
                        isBroken = false; // Восстановили строй!
                    }
                }
            }
        }
        else
        {
            // Хвост всегда толкает вперед
            isBroken = false;
        }

        // ДВИЖЕНИЕ
        if (isBroken)
        {
            MoveBackwards();
        }
        else
        {
            MoveForward();
        }
    }

    void MoveForward()
    {
        Transform targetPoint = path[currentTargetIndex];
        float speed = SnakeSpawner.Instance.snakeSpeed;

        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.05f)
        {
            currentTargetIndex++;
            if (currentTargetIndex >= path.Length) OnReachEnd();
        }
    }

    void MoveBackwards()
    {
        // Не уходим за старт (Точка 0)
        // Если мы рядом со стартом - прекращаем пятиться, иначе застрянем
        if (currentTargetIndex <= 1 && Vector3.Distance(transform.position, path[0].position) < 0.1f)
        {
            isBroken = false;
            return;
        }

        int backIndex = Mathf.Max(0, currentTargetIndex - 1);
        Transform targetPoint = path[backIndex];
        float speed = SnakeSpawner.Instance.retreatSpeed;

        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        // Переключение вейпоинта назад
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.05f)
        {
            currentTargetIndex--;
            if (currentTargetIndex < 1) currentTargetIndex = 1;
        }
    }

    public void OnDestroyByPlayer()
    {
        if (SnakeSpawner.Instance != null) SnakeSpawner.Instance.OnRockDied(this);
    }

    void OnReachEnd()
    {
        if (SnakeSpawner.Instance != null) SnakeSpawner.Instance.RemoveFromChain(this);
        Destroy(gameObject);
    }
}