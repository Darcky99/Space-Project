using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelAcciones : MonoBehaviour
{
    [SerializeField]
    List<BotonEImage> Botones = new List<BotonEImage>();
    [SerializeField]
    List<Sprite> SpriteBotones = new List<Sprite>();
    
    
    private void OnEnable() => singletonKevin.Acciones = this;
    private void Start() {
        DesactivarTodosLosBotones();
    }


    public void MostrarAcciones()
    {
        GameObject Seleccion = AdministradorDeUI.CurrentSelect;

        if (Seleccion.CompareTag("Nave")) AccionesDeNave(Seleccion);
        else if (Seleccion.CompareTag("Construccion")) AccionesDeConstruccion(Seleccion);
    }


    
    //Determina el tipo de nave y muestra sus acciones correspondientes.
    private void AccionesDeNave(GameObject Nave)
    {
        DesactivarTodosLosBotones();
        InfoDeNave Info = Nave.GetComponent<InfoDeNave>();

     //Después, comprobamos las condiciones para cada acción y activamos/desactivamos la interacción del botón dependiendo.
        int i = 0;
        foreach(MyAccionespDelegate Accion in Info.Acciones){
            //Activamos tantos botones como tenga la nave.
            Botones[i].boton.gameObject.SetActive(true); 

            switch(Accion.Method.Name){
                case "Reparar":
                    Botones[i].boton.onClick.AddListener(delegate {Accion();}); //Asignar lógica. [mover fuera de switch?]
                    Botones[i].icono.sprite = SpriteBotones[0];                //Asignar imagen.
                    Botones[i].Texto.text = Accion.Method.Name;               //Asigna nombre.
                    //Condición de uso: Si hay ataque disponible, y aun no llega a su vida máxima, se puede usar "Reparar"
                    if (Info.EstadoAtaque == Ataque.Disponible && Info.VidaDiponible < Info.TipoDeNaveSO.Vida) Botones[i].boton.interactable = true;
                    else Botones[i].boton.interactable = false;
                break;
                case "Colonizar":
                    Botones[i].boton.onClick.AddListener(delegate {Accion();});
                    Botones[i].icono.sprite = SpriteBotones[1];
                    Botones[i].Texto.text = Accion.Method.Name;
                    //Condición: Si hay tile y es de un planeta entonces puedes colonizar.
                    if(AccionesUI.CondicionalColonizar()) Botones[i].boton.interactable = true;
                    else Botones[i].boton.interactable = false;
                break;
            }
            i++;
        }
    }





    
    private void AccionesDeConstruccion(GameObject Construccion)
    {
        DesactivarTodosLosBotones();
        
    //Activamos un botón para crear la nave fundadora. Aquí en un futuro cambiará dependiendo de la construcción que
    //sea, y de las tecnologías que se tengan, en cuanto al uso de acciones puede que aplique para algunas construcciones

        Botones[0].boton.gameObject.SetActive(true); 

        Botones[0].boton.onClick.AddListener(AccionesUI.Fundadora);
        Botones[0].icono.sprite = SpriteBotones[2];
        Botones[0].Texto.text = "Fundadora";

        if(AccionesUI.CondicionalNaves(Construccion.transform.position)) Botones[0].boton.interactable = true;
        else Botones[0].boton.interactable = false;

    }










    public void DesactivarTodosLosBotones(){ 
        //Desactiva el GO de todos los botones; además de reiniciar todos los valores del botón.
        foreach (BotonEImage Boton in Botones){
            Boton.boton.onClick.RemoveAllListeners(); //Remover logica
            Boton.icono.sprite = null;               //Remover icono
            Boton.Texto.text = "";                  //Remover texto
            Boton.boton.gameObject.SetActive(false); 
        } 
    }

    public void NoInteracuable(){
        foreach (BotonEImage Boton in Botones){
            Boton.boton.interactable = false; 
        } 
    }

}

[Serializable]
public class BotonEImage{
    [SerializeField]
    public Button boton;
    [SerializeField]
    public Image icono;
    public TextMeshProUGUI Texto;
}