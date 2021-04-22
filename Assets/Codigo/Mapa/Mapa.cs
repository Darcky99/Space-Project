using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;
using NobleConnect.Mirror;

public class Mapa : NetworkBehaviour
{
    #region Referencias a "Dimensiones" y a Grid/Tilemaps
    public static Vector2Int Dimensiones;
    public Grid grid_;
    public Tilemap PerlinTilemap;
    public static Tilemap tileMap;
    public static Tilemap tileMapUI;
    #endregion

    #region Tiles de elementos del mapa
    [Header("Elementos del mapa")]
    public List<Tile> Estrellas = new List<Tile>();
    public List<Tile> Planetas = new List<Tile>();
    public List<Tile> Lunas = new List<Tile>();
    public List<Tile> GigantesGaseosos = new List<Tile>();
    public List<Tile> CumuloDeAsteroides = new List<Tile>();
    public List<Tile> Asteroide = new List<Tile>();
    public List<Tile> AsteroidesRaros = new List<Tile>();
    public List<Tile> Perlin = new List<Tile>();
    #endregion

    #region Tiles de nodos
    private void Awake()
    {
        grid_ = GridYTileMapGameObject.GetComponent<Grid>();
        tileMap = GridYTileMapGameObject.transform.GetChild(0).GetComponent<Tilemap>();
        tileMapUI = GridYTileMapGameObject.transform.GetChild(1).GetComponent<Tilemap>();
        Tile_NodoMovimiento = Tile_NodoMovimientoPublico;
        Tile__NodoAtaque = Tile_NodoAtacquePublico;
    }
    
    private void OnEnable() => singletonKevin.mapa = this;
    public GameObject GridYTileMapGameObject;

    [Header("Tiles de nodos")]
    public Tile Tile_NodoMovimientoPublico;
    public static Tile Tile_NodoMovimiento;
    public Tile Tile_NodoAtacquePublico;
    public static Tile Tile__NodoAtaque;
    #endregion


    [ClientRpc]
    public void RpcDibujarMapa(byte[] bytesmapa)
    {
        System.Random random = new System.Random(656565);
        
        
        //Convierte a int[,] el array de bytes.
        int[,] mapa = (int[,])ObjectAndByte.BytesAObject(bytesmapa);
        int Ancho = mapa.GetUpperBound(0);
        int Alto = mapa.GetUpperBound(1);
        

        for (int x = 0; x <= Ancho; x++)
        {
            for (int y = 0; y <= Alto; y++)
            {
                Vector3Int Pos = new Vector3Int(x, y, 0);
                switch (mapa[x, y])
                {
                    case 1: //Una estrella
                        tileMap.SetTile(Pos, Estrellas[random.Next(0, Estrellas.Count)]);
                        break;
                    case 2: //Planeta rocoso
                        tileMap.SetTile(Pos, Planetas[random.Next(0, Planetas.Count)]);
                        break;
                    case 3: //Planeta gaseoso
                        tileMap.SetTile(Pos, GigantesGaseosos[random.Next(0, GigantesGaseosos.Count)]);
                        break;
                    case 4: //Lunas
                        tileMap.SetTile(Pos, Lunas[random.Next(0, Lunas.Count)]);
                        break;
                    case 5: //Cumulo de asteroides
                        tileMap.SetTile(Pos, CumuloDeAsteroides[random.Next(0, CumuloDeAsteroides.Count)]);
                        break;
                    case 6: //Asteroide
                        print("XD");
                        break;
                    case 7: //Asteroides raros
                        tileMap.SetTile(Pos, AsteroidesRaros[random.Next(0, AsteroidesRaros.Count)]);
                    break;
                } } } }
    
    [ClientRpc]
    public void RpcDefinirMapa(Vector2Int _Dimensiones) => Dimensiones = _Dimensiones;


    public void DrawPerlin(Vector2Int AnchoYAlto, Vector2Int PerlinOffSet)
    {
        float Escala = 50f;

        print(AnchoYAlto);
        print(PerlinOffSet);

        for (int x = 1; x <= AnchoYAlto.x; x++)
        {
            for (int y = 1; y <= AnchoYAlto.y; y++)
            {
                float valor = Mathf.PerlinNoise((x / (float)AnchoYAlto.x) * Escala + PerlinOffSet.x, (y / (float)AnchoYAlto.y) * Escala + PerlinOffSet.y);
                
                string valorS = valor.ToString("F1");

                switch (valorS)
                {
                    case "0":
                        PerlinTilemap.SetTile(new Vector3Int(x, y, 0), Perlin[0]);
                        break;
                    case "0.1":
                        PerlinTilemap.SetTile(new Vector3Int(x, y, 0), Perlin[1]);
                        break;
                    case "0.2":
                        PerlinTilemap.SetTile(new Vector3Int(x, y, 0), Perlin[2]);
                        break;
                    case "0.3":
                        PerlinTilemap.SetTile(new Vector3Int(x, y, 0), Perlin[3]);
                        break;
                    case "0.4":
                        PerlinTilemap.SetTile(new Vector3Int(x, y, 0), Perlin[4]);
                        break;
                    case "0.5":
                        PerlinTilemap.SetTile(new Vector3Int(x, y, 0), Perlin[5]);
                        break;
                    case "0.6":
                        PerlinTilemap.SetTile(new Vector3Int(x, y, 0), Perlin[6]);
                        break;
                    case "0.7":
                        PerlinTilemap.SetTile(new Vector3Int(x, y, 0), Perlin[7]);
                        break;
                    case "0.8":
                        PerlinTilemap.SetTile(new Vector3Int(x, y, 0), Perlin[8]);
                        break;
                    case "0.9":
                        PerlinTilemap.SetTile(new Vector3Int(x, y, 0), Perlin[9]);
                        break;
                    case "1":
                        PerlinTilemap.SetTile(new Vector3Int(x, y, 0), Perlin[11]);
                        break;
                }
            }
        }
    }
}