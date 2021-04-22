using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Interceptor", menuName = "Civilizaciones/Naves/Interceptor")]
public class InterceptorSO : NavesSO
{
    private void Awake()
    {
        if (Caracteristicas_.Count <= 0)
        {
            TipoDeNave_ = TipoDeNave.Interceptor;
            Caracteristicas_.Add(Caracteristicas.Hostilidad);
            Caracteristicas_.Add(Caracteristicas.Estrategia);
            Caracteristicas_.Add(Caracteristicas.Reparar);

            Vida = 25; Ataque = 4; Defenza = 2; Movilidad = 6; Alcanze = 2; Costo = 15;
            Debug.Log("Se a llamado al Awake de InterceptorSO");
        }
    }
}
