using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopUpGenerator : MonoBehaviour
{
    public static DamagePopUpGenerator instance;
    public GameObject prefab;
    public int poolSize = 10; // Number of pop-ups to pre-instantiate
    private Queue<GameObject> popUpPool;

    void Awake()
    {
        instance = this;
        InitializePool();
    }

    void InitializePool()
    {
        popUpPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject popUp = Instantiate(prefab);
            popUp.SetActive(false); 
            popUpPool.Enqueue(popUp);
        }
    }

    public void CreatePopUp(Vector3 position, string text, Color color, int fontSize = 10)
    {
        if (popUpPool.Count > 0)
        {
            GameObject popUp = popUpPool.Dequeue();
            popUp.SetActive(true);
            popUp.transform.position = position;

            var animation = popUp.GetComponent<DamagePiopUpAnimation>();
            if (animation != null)
            {
                animation.ResetAnimation(position); 
            }

            var tmp = popUp.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.faceColor = color;

            StartCoroutine(ReturnToPoolAfterDelay(popUp, 1f));
        }
        else
        {
            Debug.LogWarning("Pop-up pool is empty.");
        }
    }

    private IEnumerator ReturnToPoolAfterDelay(GameObject popUp, float delay)
    {
        yield return new WaitForSeconds(delay);
        popUp.SetActive(false); 
        popUpPool.Enqueue(popUp);
    }
}
