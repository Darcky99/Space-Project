using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public static class MostrarNodos
{
    public static bool MoviendoNave = false;
    public static bool MostrandoNodos = false;
    

    #region Nodos de movimiento (Crear mapa y mostrar)

    public static int[,] CrearMapaDeNodos(GameObject Nave, int MaxMov)
    {
        if (MoviendoNave) return new int[0, 0];
        LimpiarNodos();

        //Posicion de la nave en el grid.
        Grid grid = singletonKevin.mapa.grid_;
        Vector2 NavePos = new Vector2(Nave.transform.position.x, Nave.transform.position.y);
        Vector3Int PosGrid = grid.WorldToCell(NavePos);
        
        //En este mapa, solo se representan los nodos de movimiento.
        int[,] mapadeNodos = new int[Mapa.Dimensiones.x, Mapa.Dimensiones.x];
        //Define la posicion de la nave.
        mapadeNodos[PosGrid.x, PosGrid.y] = 1;
        int ValorDeMovimientoDeNodosActual = 1;

        while (ValorDeMovimientoDeNodosActual <= MaxMov)
        {
            //Lista de numeros X.
            List<Vector3Int> ListaDeNumerosEnElMapa = BuscarNumeroEnMapa(ValorDeMovimientoDeNodosActual, mapadeNodos);

            //Para cada uno en la lista, tomamos las coordenadas de alrededor y ponemos "X+1"
            foreach (Vector3Int CoordenadaDeNumero in ListaDeNumerosEnElMapa) 
            {
                //Lista de coordenadas alrededor.
                List<Vector3Int> TilesAlRededor = CoordenadasDeAlrededor(CoordenadaDeNumero);
                for (int i = 0; i < TilesAlRededor.Count; i++) //Vemos cada coordenada.
                {
                    Vector3Int CurrentTile = TilesAlRededor[i];

                    //Solo se "salta" los lugares donde ya hay un nodo. Por ello los obstaculos se "analizan" varias veces.
                    if (mapadeNodos[CurrentTile.x, CurrentTile.y] > 0) continue; 

                    switch (MovimientoExtra.NodoCaso(CurrentTile))
                    {
                        case 0:
                            mapadeNodos[CurrentTile.x, CurrentTile.y] = ValorDeMovimientoDeNodosActual + 1;
                            break;
                        case 1:
                            mapadeNodos[CurrentTile.x, CurrentTile.y] = ValorDeMovimientoDeNodosActual + 2;
                            break;
                        case 2:
                            mapadeNodos[CurrentTile.x, CurrentTile.y] = 0;
                            break;
                    }
                }
            }
            ValorDeMovimientoDeNodosActual++;
        }
        return mapadeNodos;
    }
    

    public static void MostrarNodosDeMovimiento(GameObject Nave, int MaxMov)
    {
        if (MoviendoNave) return;
        InfoDeNave CN = Nave.GetComponent<InfoDeNave>();
        //[Cada IA u otro jugador verificará que sea su turno antes de poder hacer cualquier accion]
       
        //En este mapa, solo se representan los nodos de movimiento.
        int[,] mapadeNodos = CrearMapaDeNodos(Nave, CN.MovimientoDisponible);
        //Dibujas mapas.
        DibujarNodosDeMovimiento(mapadeNodos);
    }
    #endregion



    #region Mover a nodo metodo y corrutina.

    
    public static void MoverANodo(Vector3Int NodoPos, GameObject NaveAMover)
    {
        InfoDeNave CN = NaveAMover.GetComponent<InfoDeNave>(); //Controlador de nave.
        Vector3Int NavePos = singletonKevin.mapa.grid_.WorldToCell(NaveAMover.transform.position);//Coordenadas de nave a mapa.
        
        List<Vector3Int> Camino = PathFindingMetodos.PathFinder(NavePos, NodoPos); //Tomamos el camino.
        if (Camino == null) return; 

        CN.MovimientoDisponible -= Camino.Count - 1; //Resta el movimiento.
        
        singletonKevin.mapa.StartCoroutine(Mover(Camino, NaveAMover, CN.MovimientoDisponible));
    }


    static IEnumerator Mover(List<Vector3Int> Camino, GameObject NaveAMover, int MovimientoRestante)
    {
        MoviendoNave = true;
        InfoDeNave CN = NaveAMover.GetComponent<InfoDeNave>();
        singletonKevin.Acciones.NoInteracuable(); //Esto se "manda a recalcular" al final de este mismo metodo.

        LimpiarNodos();
        for(int i = Camino.Count - 1; i >= 0; i--)
        {
            Vector3 Destino = singletonKevin.mapa.grid_.CellToWorld(Camino[i]);
            Destino.y += 0.25f;
            
            Vector3 direccion = Destino - NaveAMover.transform.position; 
            if (direccion != Vector3.zero) CN.Logica.CambiarDireccion(direccion); //Cambia sprite.
            //                         [Quizá dibujar una "estela" sea buena idea]
            while (Vector3.Distance(NaveAMover.transform.position, Destino) > .01f) //Mientras aun no estes ahí, acercate. owo
            {
                //NaveAMover.transform.position = Destino;
                NaveAMover.transform.position += (direccion * 2.5f * Time.fixedDeltaTime);
                yield return new WaitForSeconds(0.01f);
            }
            NaveAMover.transform.position = Destino;
        }
        MoviendoNave = false;


        //Si no tiene "Hostilidad", se agota su ataque al moverse.
        if (!CN.TipoDeNaveSO.Caracteristicas_.Contains(Caracteristicas.Hostilidad)) CN.EstadoAtaque = Ataque.Agotado;
        //Si hay movimiento disponible o Naves enemigas en rango, selecciona de nuevo.
        int NavesEnRango = MostrarNodos.NavesEnRango(CN, CN.TipoDeNaveSO.Alcanze).Count;
        if (MovimientoRestante > 0 || NavesEnRango > 0)
            Seleccionar.SeleccionarNave(CN.gameObject); //Si aun tiene movimiento o hay naves enemigas en rango, selecciona de nuevo. 
        else MarcarSeleccion.ReiniciarSeleccion(); //Si no hay más, reinicia la seleccion.
    }


    #endregion



    #region Nodos de ataque.


    public static List<GameObject> NavesEnRango(InfoDeNave NaveCentral, int Rango)
    {
        if (NaveCentral == null) return null;

        Grid grid_ = singletonKevin.mapa.grid_;
        //Saca la posición.
        Vector2 navecentral = NaveCentral.transform.position;
        Vector3Int NaveCentral_PosicionEnGrid = grid_.WorldToCell(navecentral);
        //Crear lista con los objetos alrededor.
        List<GameObject> NavesEnRango = new List<GameObject>();
        Collider2D[] Objetos = Physics2D.OverlapCircleAll(navecentral, Rango);



        foreach (Collider2D objeto in Objetos)
        {
            Vector3Int Nave_PosicionEnGrid = grid_.WorldToCell(objeto.transform.position);

            if (objeto.tag != "Nave") continue; // [[No sé si esto "rompa" todo el foreach o solo pase al siguiente]]

            InfoDeNave CN_DeNave = objeto.GetComponent<InfoDeNave>();
            //Solo se agrerga si está en rango y no pertenece a jugador.
            if (Vector3.Distance(Nave_PosicionEnGrid, NaveCentral_PosicionEnGrid) < Rango + 1 &&
                CN_DeNave.hasAuthority == false)

                NavesEnRango.Add(objeto.gameObject);
        }
        return NavesEnRango;
    }



    public static void MostrarMapaDeAtaque(GameObject NaveSeleccionada)
    {
        InfoDeNave CN = NaveSeleccionada.GetComponent<InfoDeNave>();
        List<GameObject> Mapa_de_naves = NavesEnRango(CN, CN.TipoDeNaveSO.Alcanze); //Mapa de ataque.
        

        foreach (GameObject Nave in Mapa_de_naves) {
            Debug.Log("Nave encontrada.");
            Vector3Int GridPos = singletonKevin.mapa.grid_.WorldToCell(Nave.transform.position);
            Mapa.tileMapUI.SetTile(new Vector3Int(GridPos.x, GridPos.y, 0), Mapa.Tile__NodoAtaque); } }


    #endregion



    #region "Dibujar nodos de movimiento" y "Reiniciar Nodos".


    static void DibujarNodosDeMovimiento(int[,] mapa)
    {
        int Ancho = Mapa.Dimensiones.x;
        int Alto = Mapa.Dimensiones.y;
        //Dibujamos nodos donde haya un numero igual o mas alto que 2.
        for (int x = 1; x < Ancho; x++)
        {
            for (int y = 1; y < Alto; y++)
            {
                if (mapa[x, y] >= 2) Mapa.tileMapUI.SetTile(new Vector3Int(x, y, 0), Mapa.Tile_NodoMovimiento);
            }
        }
        MostrandoNodos = true;
    }


    public static void LimpiarNodos()
    {
        Mapa.tileMapUI.ClearAllTiles();
        MostrandoNodos = false;
    }


    #endregion



    #region "Coordenadas al rededor" y "Buscar numero en mapa".

    //Toma una cordenada y devuelve una lista de 8 coordenadas alrededor de esa.
    static List<Vector3Int> CoordenadasDeAlrededor(Vector3Int Centro) 
    {
        int Ancho = Mapa.Dimensiones.x;
        int Alto = Mapa.Dimensiones.y;
        List<Vector3Int> TilesAlRededor = new List<Vector3Int>();

        if (Centro.y + 1 < Alto)
            TilesAlRededor.Add(new Vector3Int(Centro.x, Centro.y + 1, 0)); //Norte
        if (Centro.y + 1 < Alto && Centro.x + 1 < Ancho)
            TilesAlRededor.Add(new Vector3Int(Centro.x + 1, Centro.y + 1, 0)); //NorEste
        if (Centro.x + 1 < Ancho)
            TilesAlRededor.Add(new Vector3Int(Centro.x + 1, Centro.y, 0)); //Este
        if (Centro.y - 1 >= 0 && Centro.x + 1 < Ancho)
            TilesAlRededor.Add(new Vector3Int(Centro.x + 1, Centro.y - 1, 0)); //SurEste
        if (Centro.y - 1 >= 0)
            TilesAlRededor.Add(new Vector3Int(Centro.x, Centro.y - 1, 0)); //Sur
        if (Centro.y - 1 >= 0 && Centro.x - 1 >= 0)
            TilesAlRededor.Add(new Vector3Int(Centro.x - 1, Centro.y - 1, 0)); //SurOeste
        if (Centro.x - 1 >= 0)
            TilesAlRededor.Add(new Vector3Int(Centro.x - 1, Centro.y, 0)); //Oeste
        if (Centro.y + 1 < Alto && Centro.x - 1 >= 0)
            TilesAlRededor.Add(new Vector3Int(Centro.x - 1, Centro.y + 1, 0)); //NorOeste

        return TilesAlRededor;
    }

    //Busca en todo el mapa un numero y devielve todos esos en una lista
    static List<Vector3Int> BuscarNumeroEnMapa(int numeroBuscado, int[,] mapadeNodos) 
    {
        int Ancho = Mapa.Dimensiones.x;
        int Alto = Mapa.Dimensiones.y;
        List<Vector3Int> ListaDeCoordenadas = new List<Vector3Int>();
        for (int x = 0; x < Ancho; x++) {
            for (int y = 0; y < Alto; y++) {
                if (mapadeNodos[x, y] == numeroBuscado) ListaDeCoordenadas.Add(new Vector3Int(x, y, 0)); } }

        return ListaDeCoordenadas;
    }
    

    #endregion


}