using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarPlayerEnergy : MonoBehaviour
{

    private RectTransform bar;
    public Gradient gradient;
    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        bar = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }
 
    // Update is called once per frame
    void Update()
    {
        if (PlayerStats.soulEnergyValue > 0)
        {
            SetSize((float)(PlayerStats.soulEnergyValue * 0.001)); // common amount of maxEnergy; Remember: hardcoding is bad.
        }
        else
        {
            SetSize(0f);
        }
    }

    public void SetSize(float size)
    {
        bar.localScale = new Vector3(size, 1f);
        image.color = gradient.Evaluate(size);
    }
}
