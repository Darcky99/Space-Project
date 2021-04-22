using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;
using TMPro;

public class AdministradorDeTurnos : NetworkBehaviour
{
    [SyncVar]
    public int TurnoDe = 1;
    public static int Jugadores;
    public static event Action<int> NuevoTurno;

    public IndicadorDeTurno IndicadorTurno;

    private void Start() => singletonKevin.AdminDeTurno = this; 
    
    


    [ClientRpc]
    public void RpcEstablecerJugadores()
    {
        Jugadores = SmartBehaviour.smartbehaviours.Count;
        
    }


    //Metodo que se llama desde un botón.
    public void SIGUIENTETURNO() => CmdSiguienteTurno(SmartBehaviour.local.playerturn);

    [Command(ignoreAuthority = true)]
    private void CmdSiguienteTurno(int JugadorSolicitante) //Aumenta el numero de turno.
    {
        //Primero comprobamos que el jugador que llamó el metodo, es el que tiene el turno actual.
        if (JugadorSolicitante != TurnoDe) return;
        
        //Aumentamos el turno.
        if (TurnoDe == Jugadores) TurnoDe = 1;
        else TurnoDe++;
        RpcSiguienteTurno(TurnoDe);
    }
    [ClientRpc]
    void RpcSiguienteTurno(int nuevoTurnoDe_) //En cada cliente, se llama al evento que actualiza el int "TurnoDe".
    {
        TurnoDe = nuevoTurnoDe_;

        NuevoTurno?.Invoke(TurnoDe); //subxsub

        MarcarSeleccion.ReiniciarSeleccion(); //Esconde la seleccion cada que hay un nuevo turno (Si tu eres el que pasa el turno).
        IndicadorTurno.ActualizarTurnos(); //Actualiza indicador de turno.
    }
}
