using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{

        public static List<Bomb> ActiveBombs = new List<Bomb>();

        private void OnEnable()
        {
            ActiveBombs.Add(this);
        }

        private void OnDisable()
        {
            ActiveBombs.Remove(this);
        }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
            GameManager.Instance.Explode();
        }
    }

}
