using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using Mirror;
using NobleConnect.Mirror;

public class AdministradorDeUI : MonoBehaviour
{
    [Header("Panel inferior")]
    public TextMeshProUGUI NombreDeLaSeleccion;
    public TextMeshProUGUI DescripcionEnPanel;

    [Header("Tiles de seleccion")]
    public Tile SeleccionDefault;

    [Header("Codigo")]
    public RectTransform PanelInferior;
    public RectTransform PanelDeSeleccion;
    public LeanTweenType Animacion_PanelInferior;
    public EventSystem EventSystemCanvas;

    [Header("Materiales")]
    public Material SpriteDefault;
    public Material SpriteOutline;
    
    [Header("Historial de selección")]
    public static GameObject CurrentSelect = null;
    public static Vector3Int CurrentTile = Vector3Int.zero;

    Vector2 MouseClicDownPos = Vector2.zero;
    
    
    private void OnEnable() => singletonKevin.AdminUI = this;


    //Se determina cuando se debe llamar al metodo "Seleccionar()".
    private void Update()
    {
        if (SmartBehaviour.local == null) { return; }

        
        //Al hacer clic se guarda la posición del mouse.
        if(Input.GetMouseButtonDown(0)) MouseClicDownPos = Input.mousePosition;

        //Al levantar el mouse, si no está sobre un elemento de la UI, la direrencia de posiciones entre MouseDown y MouseUp no es mucha y además no se está moviendo ninguna nave.
        else if (Input.GetMouseButtonUp(0) && EventSystemCanvas.IsPointerOverGameObject().Equals(false) &&
                 Vector2.Distance(MouseClicDownPos, Input.mousePosition) < 5f && !MostrarNodos.MoviendoNave)
        ElegirCollider();
        
    }
    
    //Se toca un punto y dependiendo de los colliders que se toquen se seleccionará una u otra.
    void ElegirCollider()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), .01f);
        
        switch (colliders.Length)
        {
            case 0:
                SeleccionarTile();
                break;

            case 1:
                Vector3Int PosColiderGrid = singletonKevin.mapa.grid_.WorldToCell(colliders[0].transform.position);
                //Si hay nodo
                if (Mapa.tileMapUI.HasTile(PosColiderGrid) && Mapa.tileMapUI.GetTile(PosColiderGrid).name.Contains("Nodo")) 
                SeleccionarNodo(PosColiderGrid);
                //Si no hay nodo selecciona el GameObject.
                else
                {
                    if (CurrentSelect != colliders[0].gameObject) //Es uno nuevo
                    SeleccionarGO(colliders[0].gameObject);
                    //Es el mismo
                    else{
                        SeleccionarTile();
                        CurrentSelect = null;
                    }
                }
                break;

            case 2:

                byte nave = 0;

                if(colliders[0].tag.Contains("Nave"))nave = 0;
                else if(colliders[1].tag.Contains("Nave")) nave = 1;
                //Se asume que uno de los 2 es una nave. Debería cumplirse siempre.
                //Si la nave no está ya seleccionada, seleccionala.
                if(colliders[nave].gameObject != CurrentSelect){
                    SeleccionarGO(colliders[nave].gameObject);
                } //De otro modo selecciona la construcción.
                else{
                    //Dependiendo de donde está la nave seleccionas el otro GO.
                    if(nave == 0) SeleccionarGO(colliders[1].gameObject);
                    if(nave == 1) SeleccionarGO(colliders[0].gameObject);
                }

                break;
        }
    }


    

    #region Metodos de seleccion y UI.
    

    void SeleccionarGO(GameObject Objeto)
    {
        CurrentSelect = Objeto;
        //Reiniciamos nodos.
        MostrarNodos.LimpiarNodos(); //T
        MarcarSeleccion.Marcarseleccion(MarcarSeleccion.Vector3Int_PorDefecto);

        if (Objeto.CompareTag("Nave"))
            Seleccionar.SeleccionarNave(Objeto);
        else if (Objeto.CompareTag("Construccion"))
            Seleccionar.SeleccionarConstruccion(Objeto);
    }   


    //Determina que tipo de tile se va a mostrar/marcar.
    void SeleccionarTile()
    {
        Vector3Int PosGrid = UIMetodosPaneles.DeScreenPosAGrid(Input.mousePosition, 0);
        //Si hay un nodo...
        if (Mapa.tileMapUI.HasTile(PosGrid) && Mapa.tileMapUI.GetTile(PosGrid).name.Contains("Nodo"))
        {
            MarcarSeleccion.Marcarseleccion(Vector3Int.zero);
            SeleccionarNodo(PosGrid);
        }
        //Hay tile.
        else if (Mapa.tileMap.HasTile(PosGrid)) 
        {
            //Reiniciamos cualquier otra cosa.
            if(MostrarNodos.MostrandoNodos) MostrarNodos.LimpiarNodos();
            MarcarSeleccion.MarcarSeleccionGO(null);

            //Si es la misma: Reinicia la seleccion.
            if (PosGrid == CurrentTile) MarcarSeleccion.ReiniciarSeleccion();

            //Si es una nueva: Mueve el panel inferior y marca en el tilemapUI la tile.
            else
            {
                MoverPaneles.MoverPanelInferiorUI(Tiles.GetTileInfo(Mapa.tileMap.GetTile(PosGrid).name));
                MarcarSeleccion.Marcarseleccion(PosGrid);
                CurrentTile = PosGrid;
            }
        } 
        //No hay nada, se reinicia UI.
        else MarcarSeleccion.ReiniciarSeleccion();
    }



    //Llamarás a este metodo cuando determines que se tocó un nodo (desde el metodo de arriba).
    void SeleccionarNodo(Vector3Int NodoPos)
    {
        LogicaNave Logica = CurrentSelect.GetComponent<LogicaNave>();
        NodoPos = new Vector3Int(NodoPos.x, NodoPos.y, 0);
        //Determina tipo de nodo y de ahí ejecuta "x" accion.
        if (Mapa.tileMapUI.GetSprite(NodoPos).name.Contains("movimiento")) Logica.CmdMover(NodoPos); /*Movimiento.MoverANodo(NodoPos, CurrentSelect)*/
        else if (Mapa.tileMapUI.GetSprite(NodoPos).name.Contains("ataque")) {
            #region Conseguir nave objetivo.
            Vector3 PosWorld = singletonKevin.mapa.grid_.CellToWorld(NodoPos);
            Collider2D[] Colliders = Physics2D.OverlapCircleAll(PosWorld, .1f);
            int NaveEnArray = 0;
            if (Colliders[0].CompareTag("Nave")) NaveEnArray = 0;
            else if (Colliders[1].CompareTag("Nave")) NaveEnArray = 1;
            GameObject NaveObjetivo = Colliders[NaveEnArray].gameObject; //Nave Objetivo
            InfoDeNave NaveSeleccionada = CurrentSelect.GetComponent<InfoDeNave>();
            #endregion
            NaveSeleccionada.Logica.CmdInteractuar(NaveObjetivo, TipoDeInteraccion.Atacar);   } }


    
    #endregion
}








//Metodos para mostrar la info de segun que objetos.
public static class Seleccionar
{
    static InfoParaPanelInferior Info = new InfoParaPanelInferior();

    //Muestra info de la nave y le cambia el material.
    public static void SeleccionarNave(GameObject Objeto)
    {
        InfoDeNave CN = Objeto.GetComponent<InfoDeNave>();
    
        if (CN.hasAuthority)    {
            CN.Logica.MostrarNodos(); //Mostar nodos.
            Info.Nombre = CN.TipoDeNaveSO.Nombre; Info.Descripcion = CN.TipoDeNaveSO.GetDescripcion(); Info.Autoridad = true;
        }
        else {
            Info.Nombre = CN.TipoDeNaveSO.Nombre; Info.Descripcion = "¡Este es el enemigo!"; Info.Autoridad = false;
        }

        MoverPaneles.MoverPanelInferiorUI(Info);
        MarcarSeleccion.MarcarSeleccionGO(Objeto.GetComponent<SpriteRenderer>());
        
        singletonKevin.Acciones.MostrarAcciones();
    }

    //Dependiendo de si es tuyo o no te da "x" o "y" información.
    public static void SeleccionarConstruccion(GameObject Objeto)
    {
        Construcciones Con = Objeto.GetComponent<Planetas>();

        if(Con is Planetas)
        {
            Planetas Controlador = Con as Planetas; //Saca la info a mostrar.
            PlanetaSO ConsSO = Controlador.Planeta_Tipo;
        
            if (Controlador.hasAuthority) {
                Info.Nombre = ConsSO.Nombre; Info.Descripcion = ConsSO.Descripcion;
                MoverPaneles.MoverPanelInferiorUI(Info);
            }
            else {
                Info.Nombre = ConsSO.Nombre; Info.Descripcion = "Captura este planeta con una de tus naves";
                MoverPaneles.MoverPanelInferiorUI(Info);
            }
            

            MarcarSeleccion.Marcarseleccion(singletonKevin.mapa.grid_.WorldToCell(Controlador.transform.position));
            singletonKevin.Acciones.MostrarAcciones();
        }
    }

}



//Metodos para mover los paneles.
public static class MoverPaneles
{
    static RectTransform PanelInferior = singletonKevin.AdminUI.PanelInferior;
    static RectTransform PanelDeSeleccion = singletonKevin.AdminUI.PanelDeSeleccion;
    static LeanTweenType Animacion = singletonKevin.AdminUI.Animacion_PanelInferior;

    private static InfoParaPanelInferior ProximaInfo = new InfoParaPanelInferior();


    //Mueve el panel inferior y por ello tambien al de selección.
    public static void MoverPanelInferiorUI(InfoParaPanelInferior Info)
    {
        //Si están vacias baja y no suba. Suba el panelInferior.
        if (Info == null) 
        {
            UIMetodosPaneles.CambiarInfoDePanelInferior(Info);

            LeanTween.move(PanelDeSeleccion, new Vector3(0, -135, 0), .2f).setEase(Animacion);
            LeanTween.move(PanelInferior, new Vector3(0, 0, 0), .2f).setEase(Animacion); //-385
            MostrarNodos.LimpiarNodos();
        }
        
        else
        {
            ProximaInfo.Nombre = Info.Nombre;
            ProximaInfo.Descripcion = Info.Descripcion;
            //Si no está abajo, bajalo y espera a que se llame la otra función. Si ya esta abajo, solo sube la info.
            if (Info.Autoridad)
            {
                if (PanelDeSeleccion.anchoredPosition.y != -135)
                  LeanTween.move(PanelDeSeleccion, new Vector3(0, -135, 0), .2f).setEase(Animacion).setOnComplete(SubirAcciones);
                else SubirAcciones();
            }
            else
            {
                if (PanelDeSeleccion.anchoredPosition.y != -135)
                  LeanTween.move(PanelDeSeleccion, new Vector3(0, -135, 0), .2f).setEase(Animacion).setOnComplete(SubirSoloInfo);
                else SubirSoloInfo();
            }
        }
    }
    
    //Solo sube el nombre y la descripcion.
    static void SubirSoloInfo()
    {
        //Al subir la info baje el "PanelInferior".
        LeanTween.move(PanelInferior, new Vector3(0, -385, 0), .2f).setEase(Animacion);

        InfoParaPanelInferior Info = new InfoParaPanelInferior(ProximaInfo.Nombre, ProximaInfo.Descripcion);
        UIMetodosPaneles.CambiarInfoDePanelInferior(Info);

        LeanTween.move(PanelDeSeleccion, new Vector3(0, 140, 0), .2f).setEase(Animacion);
    }
    //Sube el panel de seleccion con todo y acciones.
    static void SubirAcciones()
    {
        //Al subir la info baje el "PanelInferior".
        LeanTween.move(PanelInferior, new Vector3(0, -385, 0), .2f).setEase(Animacion);

        InfoParaPanelInferior Info = new InfoParaPanelInferior(ProximaInfo.Nombre, ProximaInfo.Descripcion);
        UIMetodosPaneles.CambiarInfoDePanelInferior(Info);

        LeanTween.move(PanelDeSeleccion, new Vector3(0, 375, 0), .2f).setEase(Animacion);
    }
}