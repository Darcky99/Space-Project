using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathFindingMetodos
{

    public static List<Vector3Int> PathFinder(Vector3Int NavePos, Vector3Int Destino)
    {
        int Ancho = Mapa.Dimensiones.x; int Alto = Mapa.Dimensiones.y;
        int i = 0;
        

        List<Vector3Int> Camino = new List<Vector3Int>();
        Vector4[,] MapaPathFinding = new Vector4[Ancho, Alto];
        List<Vector2Int> ValoresYaRevisados = new List<Vector2Int>();
        //Preparación para entrar al primer while.
        MapaPathFinding[NavePos.x, NavePos.y] = CalcularvaloresDeTile(NavePos, Destino, NavePos, 0, MapaPathFinding); 
        
        Vector4 ZMenor = MapaPathFinding[NavePos.x, NavePos.y];
        Vector2Int PosdeZ = new Vector2Int(NavePos.x, NavePos.y);

        //Buscamos la "Zmenor".
        while (MapaPathFinding[Destino.x, Destino.y].z == 0) //Mientras no haya valor en z para el destino, seguimos.
        {
            //Recorre todo el mapa y busca la z mas pequeña.
            for (int x = 0; x < Ancho; x++)
            {
                for (int y = 0; y < Alto; y++)
                {
                    //Para entrar a este "if" hay que: No valer "0" en Z. No estar en la lista de valores revisados. No ser un obstaculo.
                    if (MapaPathFinding[x, y].z != 0 && (EstaEnLista(new Vector2Int(x, y), ValoresYaRevisados) == false) && MovimientoExtra.NodoCaso(new Vector3Int(x, y, 0)) != 2)
                    {
                        //Tener un valor menor a ZMenor.
                        if (MapaPathFinding[x, y].z < ZMenor.z )
                        {
                            //Guardamos los datos de la tile con la "Z" mas baja.
                            ZMenor = MapaPathFinding[x, y];
                            PosdeZ.x = x;
                            PosdeZ.y = y;
                        }
                        //Si encuentras otra Z con igual valor, mira en el valor "x" para decidir cual usar.
                        else if (MapaPathFinding[x, y].z == ZMenor.z) 
                        {
                            //Debug.Log("Ha entrado al segundo");
                            //Si tiene una y menor a la de "ZMenor" entonces está será Z menor.
                            if (MapaPathFinding[x, y].y < ZMenor.y) { ZMenor = MapaPathFinding[x, y]; PosdeZ.x = x; PosdeZ.y = y; }
                        }
                    }
                }
            }

            //Antes de calcular los valores, lo añadimos a la lista de valores ya revisados.
            ValoresYaRevisados.Add(new Vector2Int(PosdeZ.x, PosdeZ.y));
            //Calculamos sus valores alrededor..
            MapaPathFinding = CalcularValoresAlrededor((Vector2Int)Destino, (Vector2Int)NavePos, PosdeZ, ValoresYaRevisados, MapaPathFinding);

            //Necesitaremos los datos de la "Z" mas baja. Asi que comprobamos si ya hay que salir antes de reiniciarlos.
            if (MapaPathFinding[Destino.x, Destino.y].z != 0) break;

            i++;

            if(i >= 30) break;

            //Reiniciamos estos valores
            ZMenor = new Vector4(0, 0, 100000, 0);
            PosdeZ = new Vector2Int();
        }
        //Terminamos el PathFinding hasta el destino.


        //Ahora regresa una lista de posiciones hasta el origen.
        return ListaDePosicionesHastaElOrigen(MapaPathFinding, NavePos, Destino, 0);
    }


    #region "Esta en lista", "Camino listo" y "Está dentro de los límites".


    public static bool EstaEnLista(Vector2 PosicionDeValores, List<Vector2Int> ListaValores)
    {
        bool EstaEnLista = false;

        foreach (Vector2Int valor in ListaValores) if (PosicionDeValores == valor) EstaEnLista = true;
        
        return EstaEnLista;
    }
    

    public static bool CaminoListo(List<Vector3Int> Camino, Vector3Int Origen)
    {
        bool CaminoListo = false;
        foreach (Vector3 Coordenada in Camino) if (Coordenada == Origen) CaminoListo = true;
        return CaminoListo;
    }


    public static bool EstaDentrodelosLimites(Vector2Int posicion)
    {
        bool estadentro = false;
        if (posicion.x >= 0 && posicion.x < Mapa.Dimensiones.x && posicion.y >= 0 && posicion.y < Mapa.Dimensiones.y) estadentro = true;
        return estadentro;
    }


    #endregion


    #region Calculo de valores. "Calcular valores alrededor" y "Calcular valores de tile".
    


    /// <summary>
    /// Toma un mapa de Vectores4 y lo devuelve con valores calculados para las tiles alrededor de "x" casilla.
    /// </summary>
    /// <param name="mapaDeValores">El mapa en el que anotaremos los valores.</param>
    /// <param name="Destino">La posicion de la casilla destino</param>
    /// <param name="Origen">La posicion de la casilla origen</param>
    /// <param name="PuntoCentral">Posicion de la casilla con la z mas baja.</param>
    /// <returns></returns>
    public static Vector4[,] CalcularValoresAlrededor(Vector2Int Destino, Vector2Int Origen, Vector2Int PuntoCentral, List<Vector2Int> ListaValores, Vector4[,] MapaPathFinding)
    {
        Vector4[,] _Mapa = MapaPathFinding;
        

        //Norte         [Si no sale de los limites]     Hace falta revisar si ya es un valor revisado para no sobreescribir los valores.
        Vector2Int PosNorte = new Vector2Int(PuntoCentral.x, PuntoCentral.y + 1);
        if (EstaDentrodelosLimites(PosNorte) && EstaEnLista(PosNorte, ListaValores) == false) _Mapa[PosNorte.x, PosNorte.y] = CalcularvaloresDeTile((Vector3Int)PosNorte, (Vector3Int)Destino, (Vector3Int)Origen, 1, MapaPathFinding);
        //Noreste
        Vector2Int PosNorteste = new Vector2Int(PuntoCentral.x + 1, PuntoCentral.y + 1);
        if (EstaDentrodelosLimites(PosNorteste) && EstaEnLista(PosNorteste, ListaValores) == false) _Mapa[PosNorteste.x, PosNorteste.y] = CalcularvaloresDeTile((Vector3Int)PosNorteste, (Vector3Int)Destino, (Vector3Int)Origen, 2, MapaPathFinding);
        //Este
        Vector2Int PosEste = new Vector2Int(PuntoCentral.x + 1, PuntoCentral.y);
        if (EstaDentrodelosLimites(PosEste) && EstaEnLista(PosEste, ListaValores) == false) _Mapa[PosEste.x, PosEste.y] = CalcularvaloresDeTile((Vector3Int)PosEste, (Vector3Int)Destino, (Vector3Int)Origen, 3, MapaPathFinding);
        //Sureste
        Vector2Int PosSureste = new Vector2Int(PuntoCentral.x + 1, PuntoCentral.y - 1);
        if (EstaDentrodelosLimites(PosSureste) && EstaEnLista(PosSureste, ListaValores) == false) _Mapa[PosSureste.x, PosSureste.y] = CalcularvaloresDeTile((Vector3Int)PosSureste, (Vector3Int)Destino, (Vector3Int)Origen, 4, MapaPathFinding);
        //Sur
        Vector2Int PosSur = new Vector2Int(PuntoCentral.x, PuntoCentral.y - 1);
        if (EstaDentrodelosLimites(PosSur) && EstaEnLista(PosSur, ListaValores) == false) _Mapa[PosSur.x, PosSur.y] = CalcularvaloresDeTile((Vector3Int)PosSur, (Vector3Int)Destino, (Vector3Int)Origen, 5, MapaPathFinding);
        //Suroeste
        Vector2Int PosSuroeste = new Vector2Int(PuntoCentral.x - 1, PuntoCentral.y - 1);
        if (EstaDentrodelosLimites(PosSuroeste) && EstaEnLista(PosSuroeste, ListaValores) == false) _Mapa[PosSuroeste.x, PosSuroeste.y] = CalcularvaloresDeTile((Vector3Int)PosSuroeste, (Vector3Int)Destino, (Vector3Int)Origen, 6, MapaPathFinding);
        //Oeste
        Vector2Int PosOeste = new Vector2Int(PuntoCentral.x - 1, PuntoCentral.y);
        if (EstaDentrodelosLimites(PosOeste) && EstaEnLista(PosOeste, ListaValores) == false) _Mapa[PosOeste.x, PosOeste.y] = CalcularvaloresDeTile((Vector3Int)PosOeste, (Vector3Int)Destino, (Vector3Int)Origen, 7, MapaPathFinding);
        //Noroeste
        Vector2Int Noroeste = new Vector2Int(PuntoCentral.x - 1, PuntoCentral.y + 1);
        if (EstaDentrodelosLimites(Noroeste) && EstaEnLista(Noroeste, ListaValores) == false) _Mapa[Noroeste.x, Noroeste.y] = CalcularvaloresDeTile((Vector3Int)Noroeste, (Vector3Int)Destino, (Vector3Int)Origen, 8, MapaPathFinding);
        return _Mapa;
    }

    

    public static Vector4 CalcularvaloresDeTile(Vector3Int CurrentTile, Vector3Int Destino, Vector3Int Origen, int valorDirecction, Vector4[,] MapaPathFinding) //1 = Norte, 2=Noreste, 3 = Este, etc...
    {

        List<Vector3Int> ListaAOrigen = new List<Vector3Int>();
        ListaAOrigen = ListaDePosicionesHastaElOrigen(MapaPathFinding, Origen, CurrentTile, valorDirecction);
        
        float nuevaDistancia = CalcularLargoParaListaDeVectores(ListaAOrigen); //Asigna distancia hasta el origen.

        float distanciaAnterior = 100000f;
        int direccionAnterior = 0;
        //Si es una tile con valores calculados, calcula la distancia hasta el origen anterior. 
        if (MapaPathFinding[CurrentTile.x, CurrentTile.y].z > 0)
        {
            direccionAnterior = (int)MapaPathFinding[CurrentTile.x, CurrentTile.y].w;
            List<Vector3Int> ListaAOrigenAnterior = ListaDePosicionesHastaElOrigen(MapaPathFinding, Origen, CurrentTile, direccionAnterior);
            distanciaAnterior = CalcularLargoParaListaDeVectores(ListaAOrigenAnterior);
        }

        //Se empiezan a asignar valores finales.
        Vector4 valores = new Vector4();
        if (nuevaDistancia < distanciaAnterior) valores.x = nuevaDistancia;
        else valores.x = distanciaAnterior;
        valores.y = Vector3.Distance(CurrentTile, Destino);
        valores.z = valores.x + valores.y;

        if (nuevaDistancia < distanciaAnterior) valores.w = valorDirecction;
        else valores.w = direccionAnterior;


        
        return valores;
    }
    


    #endregion


    #region "Posiciones hasta el origen" y  "Calcular largo para lista de vectores".



    /// <summary>
    /// Devuelve una lista de posiciones hasta el origen.
    /// </summary>
    /// <param name="MapaPathFinding">Mapa de valores importantes para los metodos de pathfinding.</param>
    /// <param name="Origen">Coordenadas del origen.</param>
    /// <param name="CurrentTile">Coordenadas desde las que queremos calcular el origen</param>
    /// <param name="direccionInicial">Esta direccion se sobreescribirá a "CurrentTile", a no ser que este valor sea igual a cero.</param>
    /// <returns></returns>
    public static List<Vector3Int> ListaDePosicionesHastaElOrigen(Vector4[,] MapaPathFinding, Vector3Int Origen, Vector3Int CurrentTile, int direccionInicial)
    {
        List<Vector3Int> Lista = new List<Vector3Int>();
        Lista.Add(CurrentTile);

        if (CurrentTile == Origen) return Lista;
        MapaPathFinding[Origen.x, Origen.y].w = 0;

        int i = 0;
        while (CaminoListo(Lista, Origen) == false)
        {
            int cuenta = Lista.Count - 1; //Sirve para acceder al dato de "Camino" mas alto; es decir, el ultimo añadido a la lista.
            int Direccion = Mathf.FloorToInt(MapaPathFinding[Lista[cuenta].x, Lista[cuenta].y].w); //Saca la direccion del ultimo valor añadido a la lista.

            //Si vamos comenzando, reescribe la direccion.
            if (direccionInicial != 0 && cuenta == 0) Direccion = direccionInicial;


            if (Direccion == 1) Lista.Add(new Vector3Int(Lista[cuenta].x, Lista[cuenta].y - 1, 0));
            else if (Direccion == 2) Lista.Add(new Vector3Int(Lista[cuenta].x - 1, Lista[cuenta].y - 1, 0));
            else if (Direccion == 3) Lista.Add(new Vector3Int(Lista[cuenta].x - 1, Lista[cuenta].y, 0));
            else if (Direccion == 4) Lista.Add(new Vector3Int(Lista[cuenta].x - 1, Lista[cuenta].y + 1, 0));
            else if (Direccion == 5) Lista.Add(new Vector3Int(Lista[cuenta].x, Lista[cuenta].y + 1, 0));
            else if (Direccion == 6) Lista.Add(new Vector3Int(Lista[cuenta].x + 1, Lista[cuenta].y + 1, 0));
            else if (Direccion == 7) Lista.Add(new Vector3Int(Lista[cuenta].x + 1, Lista[cuenta].y, 0));
            else if (Direccion == 8) Lista.Add(new Vector3Int(Lista[cuenta].x + 1, Lista[cuenta].y - 1, 0));

            i++;
            if (i >= 30) { Debug.Log("La lista hasta el origen no fue completada."); break; }
        }

        return Lista;
    }
    


    public static float CalcularLargoParaListaDeVectores(List<Vector3Int> Lista)
    {
        float Largo = 0;
        Vector3Int[] Array = Lista.ToArray();

        if (Array.Length == 1) return Largo = 0;

        for (int i = Array.Length - 1; i > 0; i--)
            Largo += Vector3.Distance(Array[i], Array[i - 1]);


        return Largo;
    }
    


    #endregion
}
