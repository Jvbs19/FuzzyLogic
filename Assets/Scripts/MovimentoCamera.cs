using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentoCamera : MonoBehaviour
{
    public Transform Carro;
 
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(Carro.position.x, Carro.position.y + 5f, Carro.position.z - 15f), 1);
    }
}
