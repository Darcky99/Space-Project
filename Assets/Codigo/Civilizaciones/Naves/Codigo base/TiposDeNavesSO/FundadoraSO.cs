
using UnityEngine;

[CreateAssetMenu(fileName = "Fundadora", menuName = "Civilizaciones/Naves/Fundadora")]
public class FundadoraSO : NavesSO
{
    private void Awake()
    {
        if (Vida <= 0)
        {
            TipoDeNave_ = TipoDeNave.Fundadora;
            Caracteristicas_.Add(Caracteristicas.Construccion);
            Debilidades_.Add(Debilidades.Capturable);

            Vida = 10; Ataque = 0; Defenza = 1; Movilidad = 4; Alcanze = 0; Costo = 10; Esquive = 3;
            Debug.Log("Mexico se inicializó cabrones");
        }
    }
}
