using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using NobleConnect.Mirror;

public class SmartBehaviourMethods : NetworkBehaviour
{
    public SmartBehaviour smartbehaviour;

    [Command(ignoreAuthority = true)]
    public void CmdPrimerSpawn(Vector2Int Posicion)
    {
        Colonizar(Posicion);
        CrearNave(smartbehaviour.Civilizacion.Fundadora, Posicion); //La primer nave spawneada será un constructor.

        TargetLocalGameStart(connectionToClient);
    }

    [TargetRpc]
    void TargetLocalGameStart(NetworkConnection Conn)
    {
        singletonKevin.AdminDeTurno.IndicadorTurno.ActualizarTurnos();
    }
    



    public void Colonizar(Vector2Int Posicion)
    {
        Vector3 WorldPos = singletonKevin.mapa.grid_.CellToWorld((Vector3Int)Posicion);
        //Corrige la coordenada
        WorldPos.y += 0.25f; 
        GameObject Planeta = Instantiate(smartbehaviour.Civilizacion.Planeta.Prefab, WorldPos, Quaternion.identity);
        NobleServer.Spawn(Planeta, base.connectionToClient);
        RpcFinishPlanet(Planeta, Posicion);
    }

    [ClientRpc]
    void RpcFinishPlanet(GameObject Planeta, Vector2Int TilePosicion)
    {
        //Limpiar tile y tomar sprite.
        Sprite _sprite = Mapa.tileMap.GetSprite((Vector3Int)TilePosicion);
        Mapa.tileMap.SetTile((Vector3Int)TilePosicion, null);
        //Visual
        Planeta.GetComponent<SpriteRenderer>().sprite = _sprite;
        //  [Añadir efecto visual y de sonido]

        //Logica
        Planetas ContrPlant = Planeta.GetComponent<Planetas>();
        ContrPlant.Planeta_Tipo = smartbehaviour.Civilizacion.Planeta;
        //Produccion de cristales.
        ContrPlant.AsignarNombre();


        
    }
    




    public void CrearNave(NavesSO NaveTipo, Vector2Int Posicion)
    {
        Vector3 WorldPos = singletonKevin.mapa.grid_.CellToWorld((Vector3Int)Posicion);
        //Corrige la coordenada
        WorldPos.y += 0.25f; 
        //Instanciamos y colocamos en su lugar.
        GameObject Nave = Instantiate(smartbehaviour.Civilizacion.NavePrefab, WorldPos, Quaternion.identity);
        Nave.transform.position = WorldPos;
        NobleServer.Spawn(Nave, base.connectionToClient);
        //Asigna el tipo de nave localmente.
        AsignarTipoDeNave(Nave, NaveTipo.TipoDeNave_);
    }
    public void CrearNave(NavesSO NaveTipo, Vector3 WorldPos)
    {
        //Corrige la coordenada
        //WorldPos.y += 0.25f;  En este no hace falta porque se instancia desde la posición de un planeta..(?)
        //Instanciamos y colocamos en su lugar.
        GameObject Nave = Instantiate(smartbehaviour.Civilizacion.NavePrefab, WorldPos, Quaternion.identity);
        Nave.transform.position = WorldPos;
        NobleServer.Spawn(Nave, base.connectionToClient);
        //Asigna el tipo de nave localmente.
        AsignarTipoDeNave(Nave, NaveTipo.TipoDeNave_);
    }

    [ClientRpc]
    void AsignarTipoDeNave(GameObject Nave, TipoDeNave Tipo)
    {
        InfoDeNave ConNav = Nave.GetComponent<InfoDeNave>();

        switch (Tipo)
        {
            case TipoDeNave.CazaEstandar:
                ConNav.TipoDeNaveSO = smartbehaviour.Civilizacion.CazaEstandar;
                break;
            case TipoDeNave.Interceptor:
                ConNav.TipoDeNaveSO = smartbehaviour.Civilizacion.Interceptor;
                break;
            case TipoDeNave.Fundadora:
                ConNav.TipoDeNaveSO = smartbehaviour.Civilizacion.Fundadora;
                break;
        }
        ConNav.Inicializacion();
    }





    public void AumentarGemas(int Aumento){
        //Se llama desde construcciónes con autoridad (metodo llamado con autoridad y se ejecuta en todos los clientes)
        smartbehaviour.cristales += Aumento;
        singletonKevin.GemasDisplay.ActualizarValor();
    }
}