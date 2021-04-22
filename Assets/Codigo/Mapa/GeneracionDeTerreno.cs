using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public enum TipoDeMapa { SistemasSolares }

public static class GeneracionDeTerreno
{
    [Server]
    public static void GenerarMapa(TipoDeMapa TipodeMapa, Vector2Int DimensionesDeMapa)
    {
        singletonKevin.mapa.RpcDefinirMapa(DimensionesDeMapa);

        int[,] MapaGenerado = new int[DimensionesDeMapa.x, DimensionesDeMapa.y];
        Vector2Int PerlinOffSet = new Vector2Int(UnityEngine.Random.Range(0, 500000), UnityEngine.Random.Range(0, 500000)); //La semilla.

        switch (TipodeMapa)
        {
            case TipoDeMapa.SistemasSolares:
                MapaGenerado = GeneracionSistemasSolares(MapaGenerado, PerlinOffSet);
                break;
        }
        byte[] mapabytes = ObjectAndByte.ObjectABytes(MapaGenerado);


        singletonKevin.mapa.RpcDibujarMapa(mapabytes);
        //singletonKevin.mapa.DrawPerlin(new Vector2Int(DimensionesDeMapa.x, DimensionesDeMapa.y), PerlinOffSet);
    }


    [Server]
    public static int[,] GeneracionSistemasSolares(int[,] Mapa, Vector2Int PerlinOffSet)
    {
        //Estrellas.
        GeneracionBase.Estrellas(Mapa, 15, PerlinOffSet);
        //Planetas.
        List<Vector2Int> Planetas = GeneracionBase.AñadirPlanetas_AEstrellas(Mapa, new Vector2Int(2, 6));
        //Lunas.
        GeneracionBase.AñadirLunas(Mapa, 65, Planetas);
        //Generación de asteroides.
        GeneracionBase.AsteroidesSistemasSolares(Mapa, PerlinOffSet);
        //Generación de nebulosas.

        //Limpiesa extra.
        GeneracionBase.LimpiarEstrellas(Mapa);

        return Mapa;
    }


    public static void LimpiarMapa()
    {
        Tilemap Tilemap_ = Mapa.tileMap;
        int Ancho = Mapa.Dimensiones.x;
        int Alto = Mapa.Dimensiones.y;
        Vector3Int Pos = new Vector3Int();
        for (int x = 0; x <= Ancho; x++)
        {
            for (int y = 0; y <= Alto; y++)
            {
                Pos = new Vector3Int(x, y, 0);
                Tilemap_.SetTile(Pos, null);
            }
        }
    }

}



/// <summary>
/// Metodos elementales para añadir elementos al mapa.
/// </summary>
public static class GeneracionBase
{
    public static void Estrellas(int[,] mapa, int DistanciaMinima, Vector2Int PerlinOffSet)
    {
        float Ancho = NetworkManagerTest.Dimensiones.x;
        float Alto = NetworkManagerTest.Dimensiones.y;
        float Escala = NetworkManagerTest.Escala;

        for (int x = 1; x < Ancho; x++) {
            for (int y = 1; y < Alto; y++) {
                float valor = Mathf.PerlinNoise((x / Ancho) * Escala + PerlinOffSet.x, (y / Alto) * Escala + PerlinOffSet.y);

                if(valor > 0.55f) mapa[x, y] = 1;
            }   
        }
        mapa = DistanciasrEstrellas(mapa, DistanciaMinima);
    }




    static int[,] DistanciasrEstrellas(int[,] Mapa_, int DistanciaMinima)
    {
        int[,] MapaConEstrellas = Mapa_;
        Vector2Int Dimensiones = new Vector2Int(Mapa_.GetUpperBound(0) + 1, Mapa_.GetUpperBound(1) + 1);
        List<Vector2Int> PosEstrellas = TilesExtra.BuscarNumerosEnMapaInt(MapaConEstrellas, 1, new List<Vector2Int>());
        List<Vector2Int> EstrellasYaRevisadas = new List<Vector2Int>();

        
        do {
            int indice = UnityEngine.Random.Range(0, PosEstrellas.Count - 1);
            Vector2Int EstrellaActual = PosEstrellas[indice];
            EstrellasYaRevisadas.Add(EstrellaActual);
            //Actualiza la posición de las estrellas para poder seleccionar una aleatoreamente y llevar cuenta de cuantas estrellas sin revisar hay.
            PosEstrellas = TilesExtra.BuscarNumerosEnMapaInt(MapaConEstrellas, 1, EstrellasYaRevisadas);
            //Luego el mapa se actualiza borrando los "unos" alrededor.
            TilesExtra.NumeroEnRangoACero(Mapa_, EstrellaActual, DistanciaMinima, 1);

            
        } while (PosEstrellas.Count > 0);

        return MapaConEstrellas;
    }



    /// <summary>
    /// Añade planetas de todo tipo. Podrías hacer variaciones muy facilmente copiando el metodo y haciendo que coloque otros numeros en funcion de x reglas.
    /// </summary>
    /// <param name="mapa">Mapa que se modificará</param>
    /// <param name="MinimoYMaximo">Cantidad mínima y máxima de planetas por estrella</param>
    /// <returns></returns>
    public static List<Vector2Int> AñadirPlanetas_AEstrellas(int[,] mapa, Vector2Int MinimoYMaximo)
    {
        List<Vector2Int> Planetas = new List<Vector2Int>();

        Vector2Int Dimensiones = new Vector2Int(mapa.GetUpperBound(0) + 1, mapa.GetUpperBound(1) + 1);
        List<Vector2Int> CoordenadasDeEstrellas = TilesExtra.BuscarNumerosEnMapaInt(mapa, 1, new List<Vector2Int>());

        while (CoordenadasDeEstrellas.Count != 0)
        {
            int indice = UnityEngine.Random.Range(0, CoordenadasDeEstrellas.Count - 1);
            Vector2Int Seleccion_Estrella = CoordenadasDeEstrellas[indice];

            int CantidadDePlanetas = UnityEngine.Random.Range(MinimoYMaximo.x, MinimoYMaximo.y); int Cuenta = 0;
            int Anillo = Random.Range(2, 4);

            //Rodearemos de planetas la estrella mientras no supere el límite "CantidadDePlanetas".
            while (Cuenta < CantidadDePlanetas)
            {
                //La coordenada aleatoria alrededor de la estrella.
                Vector2Int NuevoPlanetaCoord = TilesExtra.CoordenadaRandomEnAnillo(Seleccion_Estrella, Anillo, Dimensiones);
               
                //Si la coordenada elegida está dentro del mapa...
                if(TilesExtra.DentroLimites(NuevoPlanetaCoord, Dimensiones))
                {
                    //Si está a 8 tiles o menos de la estrella será rocoso.
                    if(Anillo <= 8)
                    {
                        mapa[NuevoPlanetaCoord.x, NuevoPlanetaCoord.y] = 2;
                        //Borramos otros planetas alrededor.
                        TilesExtra.NumeroEnRangoACero(mapa, NuevoPlanetaCoord, 1, 2); 
                        Planetas.Add(NuevoPlanetaCoord);
                    }
                    //Si está más lejos, se generará un planeta gaseoso.
                    else
                    {
                        // [[Retirado de momento a falta de sprite para planeta gigante]]  

                        // mapa[NuevoPlanetaCoord.x, NuevoPlanetaCoord.y] = 3;
                        // //Borramos otros planetas alrededor.
                        // TilesExtra.NumeroEnRangoACero(mapa, NuevoPlanetaCoord, 2, 2); 
                        // TilesExtra.NumeroEnRangoACero(mapa, NuevoPlanetaCoord, 2, 3); 
                        // Planetas.Add(NuevoPlanetaCoord);
                    }
                    //Se hace una "separación" entre este y el próximo planeta.
                    Anillo += Random.Range(2, 4);
                    //"Cuenta" es el numero de planetas colocados.
                    Cuenta++;
                }
            }
            CoordenadasDeEstrellas.RemoveAt(indice);
        }
        return Planetas; //[Regresa coordenadas de planetas que fueron borrados]
    }




    public static void AñadirLunas(int[,] mapa, int probabilidad, List<Vector2Int> Planetas)
    {
        Vector2Int Dimensiones = new Vector2Int(mapa.GetLength(0), mapa.GetLength(1));
        

        foreach (Vector2Int Planeta in Planetas)
        {
            //Cantidad a colocar de lunas.
            int CantidadLunas = 0;
            //Tipo de planeta determina cuantas lunas puede tener.
            int planetaTipo = mapa[Planeta.x, Planeta.y];
            
            
            //Determinamos la cantidad de lunas que tendrá. [Si añades más tipos de planetas hace falta actualizar esto]
            switch (planetaTipo)
            {
                case 2: //Planeta rocoso
                    if (Random.Range(0, 101) <= probabilidad) CantidadLunas++;
                    break;
                case 3: //Planeta gaseoso
                    if (Random.Range(0, 101) <= probabilidad) CantidadLunas++;
                    if (Random.Range(0, 101) <= probabilidad) CantidadLunas++;
                    break;
                default: //Puede entrar aquí si cae sobre otra luna. [creo q no puede entrar en otras lunas. revisar-.]
                    Debug.Log("Numero de planeta no registrado");
                    Debug.Log(planetaTipo); //Será un 4.
                    break;
            }

            //Seleccionamos posición alatoria alrededor y colocamos tantas lunas como se indique (1 o 2).
            for(int i = 0; i < CantidadLunas; i++)
            {
                //Posición de la luna.
                Vector2Int LunaPos = TilesExtra.CoordenadaRandomEnAnillo(Planeta, i + 1, Dimensiones);

                mapa[LunaPos.x, LunaPos.y] = 4;
            }
        }
    }



    public static void AsteroidesSistemasSolares(int[,] mapa, Vector2Int PerlinOffSet)
    {
        float Ancho = NetworkManagerTest.Dimensiones.x;
        float Alto = NetworkManagerTest.Dimensiones.y;
        float Escala = NetworkManagerTest.Escala;

        //Añade tanto cumulos de asteroides como asteroides.
        for (int x = 1; x < Ancho; x++) {
            for (int y = 1; y < Alto; y++) {
                float valor = Mathf.PerlinNoise((x / Ancho) * Escala + PerlinOffSet.x, (y / Alto) * Escala + PerlinOffSet.y);

                //Si está vacio y tiene un valor menor a "x" entonces pon un cumulo de asteroides raros. [20%]
                if(valor <= 0.25f && valor > 0.2f && mapa[x, y] == 0 && Random.Range(0,100) < 20) mapa[x, y] = 7;

                //En este caso es un asteroide. [40%]
                //else if(mapa[x, y] == 0 && valor > 0.25f && valor <= .4f && Random.Range(0,100) < 40) mapa[x,y] = 6;

                //Y en este otro cumulo de asteroides. [30%]
                else if(mapa[x, y] == 0 && valor > 0.4f && valor <= .5f && Random.Range(0,100) < 30) mapa[x,y] = 5;
            }   
        }
    }
    
    
    
    
    public static void Nebulosas(int[,] mapa)
    {
        
    }




    public static void LimpiarEstrellas(int[,] mapa){
        Vector2Int Dimensiones = new Vector2Int(mapa.GetUpperBound(0) + 1, mapa.GetUpperBound(1) + 1);
        Vector2Int PosEstrella = new Vector2Int();
        for(int x = 0; x < Dimensiones.x; x++){
            for(int y = 0; y < Dimensiones.y; y++){
                if(mapa[x,y] == 1){
                    PosEstrella.x = x;
                    PosEstrella.y = y;
                    TilesExtra.NumeroEnRangoACero(mapa, PosEstrella, 8, 5);
                    TilesExtra.NumeroEnRangoACero(mapa, PosEstrella, 8, 7);
                }
            }
        }
    }
}


public static class TilesExtra
{
    //Esta versión excluye coorndenadas que entregas en una lista.
    public static List<Vector2Int> BuscarNumerosEnMapaInt(int[,] mapa, int NumeroABuscar, List<Vector2Int> CoordenadasAExcluir)
    {
        Vector2Int Dimensiones = new Vector2Int(mapa.GetLength(0), mapa.GetLength(1));
        List<Vector2Int> Posiciones = new List<Vector2Int>();
        Vector2Int CurrentPos = new Vector2Int(); //Si contiene el numero, añade a la lista.
        
        for (int x = 0; x < Dimensiones.x; x++) {
            for (int y = 0; y < Dimensiones.y; y++) {
                CurrentPos = new Vector2Int(x, y); 
                if (mapa[x, y] == NumeroABuscar )
                    if(CoordenadasAExcluir.Count < 1)
                        Posiciones.Add(CurrentPos);
                else if (Comparaciones.EstaEnLista(CurrentPos, CoordenadasAExcluir) == false)
                    Posiciones.Add(CurrentPos);
                
        }   }
        return Posiciones;
    }



    /// <summary>
    /// Toma una coordenada y borra un numero "x" a su alrededor dentro de un rango.
    /// </summary>
    /// <param name="mapa">Mapa en el que se hace la modificación</param>
    /// <param name="Rango">Rango en el que se borra el "NumeroABorrar"</param>
    /// <param name="Dimensiones">Dimensiones del mapa</param>
    /// <param name="Centro">Posicion central desde la que borrar "NumeroABorrar"</param>
    /// <param name="NumeroABorrar"></param>
    /// <returns></returns>
    public static void NumeroEnRangoACero(int[,] mapa, Vector2Int Centro, int Rango, int NumeroABorrar)
    {
        Vector2Int Dimensiones = new Vector2Int(mapa.GetLength(0), mapa.GetLength(1));

        int i = 1;
        
        Vector2Int EsquinaNorOeste = new Vector2Int();
        Vector2Int EsquinaNorEste = new Vector2Int();
        Vector2Int EsquinaSurEste = new Vector2Int();
        Vector2Int EsquinaSurOeste = new Vector2Int();

        Vector2Int TileActual = new Vector2Int();

        while (i <= Rango)
        {
            EsquinaNorOeste = new Vector2Int(Centro.x - i, Centro.y + i);
            EsquinaNorEste = new Vector2Int(Centro.x + i, Centro.y + i);
            EsquinaSurEste = new Vector2Int(Centro.x + i, Centro.y - i);
            EsquinaSurOeste = new Vector2Int(Centro.x - i, Centro.y - i);

            TileActual = EsquinaNorOeste;
            
            while (TileActual.x < EsquinaNorEste.x)
            {
                if (DentroLimites(TileActual, Dimensiones) && mapa[TileActual.x, TileActual.y] == NumeroABorrar)
                    mapa[TileActual.x, TileActual.y] = 0; TileActual += Vector2Int.right;
            }
            while (TileActual.y > EsquinaSurEste.y)
            {
                if (DentroLimites(TileActual, Dimensiones) && mapa[TileActual.x, TileActual.y] == NumeroABorrar)
                    mapa[TileActual.x, TileActual.y] = 0; TileActual += Vector2Int.down;
            }
            while (TileActual.x > EsquinaSurOeste.x)
            {
                if (DentroLimites(TileActual, Dimensiones) && mapa[TileActual.x, TileActual.y] == NumeroABorrar)
                    mapa[TileActual.x, TileActual.y] = 0; TileActual += Vector2Int.left;
            }
            while (TileActual.y < EsquinaNorOeste.y)
            {
                if (DentroLimites(TileActual, Dimensiones) && mapa[TileActual.x, TileActual.y] == NumeroABorrar)
                    mapa[TileActual.x, TileActual.y] = 0; TileActual += Vector2Int.up;
            }
            i++;
        }
    }



    public static bool DentroLimites(Vector2Int posicion, Vector2Int Dimensiones)
    {
        bool estadentro = false;
        if (posicion.x >= 1 && posicion.x < Dimensiones.x && posicion.y >= 1 && posicion.y < Dimensiones.y) estadentro = true;
        return estadentro;
    }


    //Coordenada aleatoria alrededor de un Vector2Int
    internal static Vector2Int CoordenadaRandomEnAnillo(Vector2Int Centro, int Anillo, Vector2Int Dimensiones)
    {
        List<Vector2Int> CoordenadasAElegir = new List<Vector2Int>();
        Vector2Int CurrentCoord = new Vector2Int(Centro.x - Anillo, Centro.y + Anillo);

        //[Necesitas comprobar si está dentro de los límites antes de añadirlo a la lista de coordenadas.]
        while (CurrentCoord.x < Centro.x + Anillo) { if (DentroLimites(CurrentCoord += Vector2Int.right, Dimensiones)) CoordenadasAElegir.Add(CurrentCoord); CurrentCoord += Vector2Int.right; }
        while (CurrentCoord.y > Centro.y - Anillo) { if (DentroLimites(CurrentCoord += Vector2Int.down, Dimensiones)) CoordenadasAElegir.Add(CurrentCoord); CurrentCoord += Vector2Int.down; }
        while (CurrentCoord.x > Centro.x - Anillo) { if (DentroLimites(CurrentCoord += Vector2Int.left, Dimensiones)) CoordenadasAElegir.Add(CurrentCoord); CurrentCoord += Vector2Int.left; }
        while (CurrentCoord.y < Centro.y + Anillo) { if (DentroLimites(CurrentCoord += Vector2Int.up, Dimensiones)) CoordenadasAElegir.Add(CurrentCoord); CurrentCoord += Vector2Int.up; }

        return CoordenadasAElegir[UnityEngine.Random.Range(0, CoordenadasAElegir.Count - 1)];
}
    }
    