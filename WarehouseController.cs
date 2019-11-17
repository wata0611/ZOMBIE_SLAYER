using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarehouseController : MonoBehaviour
{
    [SerializeField] int maxWarehouseHP = 10;
    [SerializeField] Image warehouseHPGauge;

    int warehouseHP = 0;

    public int GetMaxWarehouseHP()
    {
        return maxWarehouseHP;
    }

    public int WarehouseHP
    {
        set
        {
            warehouseHP = Mathf.Clamp(value,0,maxWarehouseHP);

            float scaleX = (float)warehouseHP / maxWarehouseHP;
            warehouseHPGauge.rectTransform.localScale = new Vector3(scaleX, 1, 1);
        }

        get
        {
            return warehouseHP;
        }
    }

    public int GetmaxWarehouseHP()
    {
        return maxWarehouseHP;
    }

    // Start is called before the first frame update
    void Start()
    {
        WarehouseHP = maxWarehouseHP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
