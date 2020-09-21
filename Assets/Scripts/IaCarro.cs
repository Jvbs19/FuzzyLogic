using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IaCarro : MonoBehaviour
{
    public float DistanciaSensores = 3f;
    public Vector3 SensorFrentePos = new Vector3(0, 0, 0);
    public float SensorLadoPos = 0.3f;
    public float SensorAngulo = 30f;
    public float curva = 5f;

    public float Vel;
    public float Rot;
    float DisFinalEsq;
    float DisFinalDir;
    float DisBombaFrente;

    Rigidbody CarroRb;

    void Awake()
    {
        CarroRb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        MovimentaFuzzy();
    }
    private void Update()
    {
        SensoresFuzzy();
    }
    void MovimentaFuzzy()
    {
        CarroRb.AddForce(transform.forward * Vel);
        CarroRb.AddTorque(transform.up * Rot);
    }
   
    #region Raycast
    void SensoresFuzzy() // Gera sensores para observar o carro
    {
        RaycastHit hit;
        Vector3 PosInicialSensor = transform.position;
        PosInicialSensor += transform.position * SensorFrentePos.z;
        PosInicialSensor += transform.up * SensorFrentePos.y;


        //Sensor Central Frente
        if (Physics.Raycast(PosInicialSensor, transform.forward, out hit, DistanciaSensores))
        {
            if (hit.collider)
            {
                DisBombaFrente = hit.distance;
            }

            Debug.DrawLine(PosInicialSensor, hit.point);
        }

        //Sensor Lateral Frente dir Angular
        else if (Physics.Raycast(PosInicialSensor, Quaternion.AngleAxis(SensorAngulo, transform.up) * transform.forward, out hit, DistanciaSensores))
        {
            if (hit.collider)
            {
                DisFinalDir = hit.distance;
            }
            Debug.DrawLine(PosInicialSensor, hit.point);
        }
 
        //Sensor Lateral Frente esq Angular
        if (Physics.Raycast(PosInicialSensor, Quaternion.AngleAxis(-SensorAngulo, transform.up) * transform.forward, out hit, DistanciaSensores))
        {
            if (hit.collider)
            {
                DisFinalEsq = hit.distance;
            }
            Debug.DrawLine(PosInicialSensor, hit.point);
        }


    }
    #endregion
   
    #region Getters/Setters
    public float GetDistanciaDirCol()
    {
        return DisFinalDir;
    }
    public void SetDistanciaDirCol(float dis)
    {
        DisFinalDir = dis;
    }
    public float GetDistanciaEsqCol()
    {
        return DisFinalEsq;
    }
    public void SetDistanciaEsqCol(float dis)
    {
        DisFinalEsq = dis;
    }
    public float GetDistanciaFrenteCol()
    {
        return DisBombaFrente;
    }
    public void SetDistanciaFrenteCol(float dis)
    {
        DisBombaFrente = dis;
    }
    public void SetAceleracao(float acc)
    {
        Vel = acc;
    }

    public float GetVelocidade()
    {
        return CarroRb.velocity.magnitude;
    }
    public float GetRotacao()
    {
        return CarroRb.angularVelocity.y;
    }
    public void SetRotacao(float rot)
    {
        Rot = rot;
    }
    #endregion
}
