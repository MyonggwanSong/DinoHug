using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ColorSelector : MonoBehaviour
{
    [SerializeField] List<ColorButton> colorButtons;
    [SerializeField] List<GameObject> models;
    [SerializeField] List<ColorModel> cms;
    [SerializeField] ColorModel selectedModel = null;

    [SerializeField] Transform modelPoint;

    public UnityAction<Material> OnUpdateSelectedMaterial;

    void Start()
    {
        for (int i = 0; i < colorButtons.Count; i++)
        {
            Color c = colorButtons[i].GetComponent<Image>().color;
            GameObject model = models[i];

            colorButtons[i].OnClickColorButton += UpdateSelectedColor;

            cms.Add(new(c, model));
        }

        Reset();

        selectedModel = cms[0];
        selectedModel.model.SetActive(true);
        UpdateMaterial();
    }

    void Reset()
    {
        foreach (var cm in cms)
        {
            cm.model.SetActive(false);
        }
    }

    void UpdateSelectedColor(Color color)
    {
        if (this.selectedModel.color == color) return;

        ColorModel model = cms.Find(c => c.color == color);

        selectedModel.model.SetActive(false);
        
        selectedModel = model;

        selectedModel.model.SetActive(true);
        UpdateMaterial();
    }

    void UpdateMaterial()
    {
        Material mat = selectedModel.model.GetComponentInChildren<SkinnedMeshRenderer>().materials[0];
        if (mat == null) return;
        OnUpdateSelectedMaterial?.Invoke(mat);
        
    }
}

[Serializable]
public class ColorModel
{
    public Color color;
    public GameObject model;

    public ColorModel(Color color, GameObject model)
    {
        this.color = color;
        this.model = model;
    }
}