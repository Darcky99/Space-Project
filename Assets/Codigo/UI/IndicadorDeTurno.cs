using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NobleConnect.Mirror;

public class IndicadorDeTurno : MonoBehaviour
{
    [Header("Actualizar turnos")]
    public TextMeshProUGUI TurnoActual;
    AdministradorDeTurnos Admin;

    

    public void ActualizarTurnos()
    {
        int TurnoDe = singletonKevin.AdminDeTurno.TurnoDe;


        if (TurnoDe == SmartBehaviour.local.playerturn) TurnoActual.text = "Es tu turno";
        else TurnoActual.text = "Turno de jugador " + TurnoDe.ToString();
    }
}
