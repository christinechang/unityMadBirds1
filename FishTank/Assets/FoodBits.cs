using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBits : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Fish fishColl = collision.collider.GetComponent<Fish>();
        if (fishColl != null) // if  fish collision
        {
            Debug.Log("particle collision" + fishColl);
            gameObject.SetActive(false);
        }
        return;
    }
}
