
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    Image HpImage;

    public float CurrenthealthPercentage
    {
        get  =>HpImage.fillAmount;
        set
        {
            if (value <= 0) HpImage.fillAmount = 0;
            else HpImage.fillAmount = value;
        }
    }
    
    void Start()
    {
        HpImage = GetComponent<Image>();
    }
}
