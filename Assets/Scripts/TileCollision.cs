using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCollision : MonoBehaviour
{
    // V�rification de l'�tat de d�part ou d'arriv�e
    public bool isStart = false;
    public bool isGoal = false;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isGoal)
        {
            //Debug.Log("Nouvelle destination");
            isGoal = true;
        }
        if (collision.CompareTag("Enemy") && !isStart)
        {
            //Debug.Log("Nouveau d�part");
            isStart = true;
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !isStart)
        {
            //Debug.Log("Nouveau d�part");
            isStart = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isGoal)
        {
            isGoal = false;
        }
        if (collision.CompareTag("Enemy") && isStart)
        {
            isStart = false;
        }
    }
}
