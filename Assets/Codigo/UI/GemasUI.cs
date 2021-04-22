using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GemasUI : MonoBehaviour
{
    public TextMeshProUGUI GemasCuenta;
    int Cuenta = 0;

    private void OnEnable() => singletonKevin.GemasDisplay = this;

    public void ActualizarValor(){
        //Animación que lleva al nuevo valor de las gemas.
        int GemasDelJugador = SmartBehaviour.local.cristales;
        //Si las gemas que tiene el jugador son menores a las que se están mostrando, entonces es un decremento.
        //No se anima el cambio.
        if(GemasDelJugador < Cuenta){
            StopAllCoroutines();    //Detendriamos una animación de aumento de llegar aquí mientras una se ejectua.
            GemasCuenta.text = GemasDelJugador.ToString();
            Cuenta = GemasDelJugador;
        }
        //De otro modo hay un aumento. Por lo tanto animar.
        else{
            StartCoroutine(AnimarAumento(GemasDelJugador));
        }
    }
    
    IEnumerator AnimarAumento(int ValorObjetivo){
        while(Cuenta < ValorObjetivo){
            Cuenta ++;
            GemasCuenta.text = Cuenta.ToString();
            yield return new WaitForSeconds(0.04f);
        }
    }

}
