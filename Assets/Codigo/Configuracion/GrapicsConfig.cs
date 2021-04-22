using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GrapicsConfig : MonoBehaviour
{
    public VolumeProfile VolumeProf;
    VolumeComponent Bloom;

    [Header("Paneles")]
    public GameObject Panel;

    [Header("Botones")]
    public Image BotonBloom;

    [Header("Sprites")]
    public Sprite BotonOnn;
    public Sprite BotonOff;


    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        foreach (VolumeComponent volumen in VolumeProf.components) if (volumen.name == "Bloom") Bloom = volumen;
        if (Bloom.active) CambiarEstadoOnnOff(BotonBloom, true);
        else CambiarEstadoOnnOff(BotonBloom, false);
    }

    public void CambiarBlom()
    {
        if (Bloom.active) { Bloom.active = false; CambiarEstadoOnnOff(BotonBloom, false); }
        else { Bloom.active = true; CambiarEstadoOnnOff(BotonBloom, true); }
    }

    public void CambiarEstadoOnnOff(Image Boton, bool NuevoEstado)
    {
        if (NuevoEstado == true) Boton.sprite = BotonOnn;
        else Boton.sprite = BotonOff;
    }

    public void Mostrar_Ocultar_Panel()
    {
        if (Panel.activeInHierarchy) Panel.SetActive(false);
        else Panel.SetActive(true);
    }

}
