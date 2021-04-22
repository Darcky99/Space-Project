
using UnityEngine;

[CreateAssetMenu(fileName = "Caza estandar", menuName = "Civilizaciones/Naves/Caza estandar")]
public class CazaEstandarSO : NavesSO
{
    private void Awake()
    {
        if (Caracteristicas_.Count <= 0)
        {
            TipoDeNave_ = TipoDeNave.CazaEstandar;
            Caracteristicas_.Add(Caracteristicas.Hostilidad);
            Caracteristicas_.Add(Caracteristicas.Reparar);

            Vida = 20; Ataque = 5; Defenza = 3; Movilidad = 3; Alcanze = 2; Costo = 10;
            Debug.Log("Mexico se inicializó cabrones");
        }
    }
}
