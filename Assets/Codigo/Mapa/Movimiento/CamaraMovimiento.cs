using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CamaraMovimiento : MonoBehaviour
{
    public EventSystem EventSystemCanvas;

    Vector3 StartPos;
    bool ComienzoValido;
    public float MinZoom = 1.5f;
    public float MaxZoom = 5f;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Almacenar posicion al tocar pantalla.
            ComienzoValido = !EventSystemCanvas.IsPointerOverGameObject();
        }
        //Zoom de camara en celular.
        if (Input.touchCount == 2)
        {
            Touch Touch0 = Input.GetTouch(0);
            Touch Touch1 = Input.GetTouch(1);

            Vector2 Touch0PrevPosition = Touch0.position - Touch0.deltaPosition;
            Vector2 Touch1PrevPosition = Touch1.position - Touch1.deltaPosition;

            float MagnitudAnterior = (Touch0PrevPosition - Touch1PrevPosition).magnitude;
            float MagnitudActual = (Touch0.position - Touch1.position).magnitude;

            float diferencia = MagnitudActual - MagnitudAnterior;

            Zoom(diferencia * 0.01f);
        }
        else if (Input.GetMouseButton(0) && ComienzoValido) //Movimiento de camara. ComienzoValido es para ver si inici� el "clic" sobre un objeto de la UI.
        {
            Vector3 direccion = StartPos - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            Camera.main.transform.position += direccion;
        }
        Zoom(Input.GetAxis("Mouse ScrollWheel")); //Zoom de camara en PC.
    }

    void Zoom(float Incremento)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - Incremento, MinZoom, MaxZoom);
    }

}
