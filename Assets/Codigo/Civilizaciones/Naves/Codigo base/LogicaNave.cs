using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using NobleConnect.Mirror;

public class LogicaNave : NetworkBehaviour
{
    public InfoDeNave Info;

    #region Logica. "Mostrar nodos", "Interactuar" y "Alterra vida"


    //Muestra visuales al seleccionar la nave.
    public void MostrarNodos() 
    {
        if (singletonKevin.AdminDeTurno.TurnoDe == SmartBehaviour.local.playerturn) //Seleccion en turno.
        {
            if (Info.MovimientoDisponible > 0)
                global::MostrarNodos.MostrarNodosDeMovimiento(gameObject, Info.MovimientoDisponible);
            if (Info.EstadoAtaque == Ataque.Disponible && Info.TipoDeNaveSO.TipoDeNave_ != TipoDeNave.Fundadora)
                global::MostrarNodos.MostrarMapaDeAtaque(gameObject);
        }
    }

    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="NaveInteractuando"></param>
    /// <param name="Cantidad">La cantidad debe ser positiva para curar, negativa para dañar.</param>
    [Command]
    public void CmdAlterarVida(Transform NaveInteractuando, int Cantidad)
    {
        int Esviquive = Random.Range(0, 100);

        //Si esquiva.
        if ((Mathf.Sign(Cantidad) == -1 && Esviquive <= Info.TipoDeNaveSO.Esquive))
        {
            RpcAlterarVida(NaveInteractuando, true, 0);
        }
        else if ((Info.VidaDiponible + Cantidad) <= 0) //Si muere.
        {
            //Anicia animacion de muerte.


            //Al final destruimos la nave.
            NobleServer.Destroy(gameObject);
        }
        //Curar o dañar de manera local.
        else RpcAlterarVida(NaveInteractuando, false, Cantidad);
    }
    
    [ClientRpc] //Cambia localmente la vida de la nave.
    void RpcAlterarVida(Transform NaveInteractuando, bool Esquivado, int cantidad)
    {
        //Esquive.
        if (Esquivado == true)
        {
            print("Esquivado papulinze");
            Vector2 direccion = NaveInteractuando.position - transform.position;
            LeanTween.move(gameObject, ((-direccion.normalized) / 6) + (Vector2)transform.position, .15f).setEase(LeanTweenType.easeShake);
            return;
        }
        //Interaccion. Entrar en este implica que la nave no morirá.
        else if (Esquivado == false)
        {
            Info.VidaDiponible += cantidad;
            ActualizarVida();

            //Inicia animación de curación
            if(Mathf.Sign(cantidad) == 1)
            {
                Info.EstadoAtaque = Ataque.Agotado;
                //Animacion de curación.
            }
            else//Animacion de daño.
            {

            }
        }
    }
    




    
    [Command] //Metodo usado para dañar, curar, contraatacar a otras naves.
    public void CmdInteractuar(GameObject NaveObjetivo, TipoDeInteraccion Accion) => RpcInteractuar(NaveObjetivo, Accion);

    [ClientRpc]
    void RpcInteractuar(GameObject NaveObjetivo, TipoDeInteraccion Accion)
    {
        global::MostrarNodos.LimpiarNodos();
        //Obtener informacion de la nave objetivo.
        InfoDeNave InfoObjetivo = NaveObjetivo.GetComponent<InfoDeNave>();

        //Desde aqui indicamos si vamos a atacar o contraatacar. Quizá en un futuro otras cosas.
        Interacciones.Interactuar(Info, InfoObjetivo, Accion);
        //Si es contraataque, el estado de "ataque" no se reiniciará.
        if (Accion.Equals(TipoDeInteraccion.Atacar)) Info.EstadoAtaque = Ataque.Agotado;
    }





    [Command]
    public void CmdMover(Vector3Int Destino) => RpcMover(Destino);

    [ClientRpc]
    void RpcMover(Vector3Int Destino) => global::MostrarNodos.MoverANodo(Destino, gameObject);
    


    #endregion


    
    #region Actualizar visuales. "Actulizar vida"

    public void ActualizarVida() => Info.Vida_diplay.text = Info.VidaDiponible.ToString();

    //Este es para el movimiento.
    public void CambiarDireccion(Vector3 direccion) => CNMetodos.CambiarSprite(Info.Spriterenderer, direccion, Info.TipoDeNaveSO);

    //Este es para el ataque.
    public void CambiarDireccion(Vector3Int NaveAtacante, Vector3Int NaveAtacada)
    {
        Vector3 Direccion = CNMetodos.Calculardireccion(NaveAtacante, NaveAtacada);
        CambiarDireccion(Direccion);
    }

    #endregion

    


    //Metodo llamado cada que se pasa de turno.
    public void NuevoTurno(int TurnoDe)
    {
        //Si no es el turno de este jugador o no tiene autoridad no regeneres la nave.
        if (TurnoDe != SmartBehaviour.local.playerturn || !hasAuthority) return;


        Info.EstadoAtaque = Ataque.Disponible;
        
        Info.MovimientoDisponible = Info.TipoDeNaveSO.Movilidad;
        //Si tiene X mejora, entonces al movimiento sumamos aún más.
        /*Animación de turno nuevo!*/
    }

}






public static class CNMetodos
{
    //Cambia el sprite en función de la direccion.
    public static void CambiarSprite(SpriteRenderer SR, Vector3 direccion, NavesSO TipodeNave)
    {
        if (direccion == new Vector3(-.5f, .25f)) SR.sprite = TipodeNave.Sprites.Norte_Idle;
        else if (direccion == new Vector3(0f, .5f)) SR.sprite = TipodeNave.Sprites.Noreste_Idle;
        else if (direccion == new Vector3(.5f, .25f)) SR.sprite = TipodeNave.Sprites.Este_Idle;
        else if (direccion == new Vector3(1f, 0f)) SR.sprite = TipodeNave.Sprites.Sureste_Idle;
        else if (direccion == new Vector3(.5f, -.25f)) SR.sprite = TipodeNave.Sprites.Sur_Idle;
        else if (direccion == new Vector3(0f, -.5f)) SR.sprite = TipodeNave.Sprites.Suroeste_Idle;
        else if (direccion == new Vector3(-.5f, -.25f)) SR.sprite = TipodeNave.Sprites.Oeste_Idle;
        else if (direccion == new Vector3(-1f, 0f)) SR.sprite = TipodeNave.Sprites.Noroeste_Idle;
    }

    //Calcula una direccion fija que se usa para el metodo de arriba.
    public static Vector3 Calculardireccion(Vector3Int PosNaveDeAtaque, Vector3Int NaveAtacada)
    {
        Vector3Int PuntoMedio = NaveAtacada;
        while (Vector3Int.Distance(PuntoMedio, PosNaveDeAtaque) > 1.5f)
        {
            PuntoMedio = PuntoMedioMetodo(PuntoMedio, PosNaveDeAtaque);
        }
        //Cambiar coordenadas a "World" y de ahí regresarlas
        Vector3 PosNaveDeAtaqueWorld = singletonKevin.mapa.grid_.CellToWorld(PosNaveDeAtaque);
        PosNaveDeAtaqueWorld = new Vector3(PosNaveDeAtaqueWorld.x, PosNaveDeAtaqueWorld.y + .25f);
        Vector3 PuntoMedioWorld = singletonKevin.mapa.grid_.CellToWorld(PuntoMedio);
        PuntoMedioWorld = new Vector3(PuntoMedioWorld.x, PuntoMedioWorld.y + .25f);

        return PuntoMedioWorld - PosNaveDeAtaqueWorld;
    }

    //Punto medio entre 2 puntos.
    static Vector3Int PuntoMedioMetodo(Vector3Int PuntoDeDestino, Vector3Int PuntoNaveDeAtaque)
    {
        return new Vector3Int((PuntoDeDestino.x + PuntoNaveDeAtaque.x) / 2, (PuntoDeDestino.y + PuntoNaveDeAtaque.y) / 2, 0);
    }

}