using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BorderMaxPlayerEnergy : MonoBehaviour
{

    private RectTransform border;

    // Start is called before the first frame update
    void Start()
    {
        border = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        SetSize((float)(PlayerStats.maxSoulEnergyValue * 0.001)); // common amount of maxEnergy; Remember: hardcoding is bad.
    }

    public void SetSize(float size)
    {
        if (size > 0)
        {
            size += 0.04f; // for beutifull border (approximatly)
        }
        border.localScale = new Vector3(size, 1f);
    }
}
