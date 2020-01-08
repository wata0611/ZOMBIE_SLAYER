using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SupplyZone : MonoBehaviour
{

    [SerializeField] Text discriptionText;
    [SerializeField] bool supplyEnabled = true;
    int tmpWarehouseHP = 0;
    FirstPersonGunController player;
    WarehouseController warehouse;
    GameManager gameManager;

    void OnTriggerStay(Collider collider) {
        if (collider.gameObject.tag == "Player") {
            discriptionText.enabled = true;
            discriptionText.text = "C:Repair Warehouse(" + player.GetRepairScore().ToString() + ")\n"
                + "V:Supply Magzine(" + player.GetSupplyScore().ToString() + ")\n"
                + "X:Unlocked Weapon Level("+player.GetUnlockedScore()+")\n"
                + "Z:Heel Player("+player.GetHeelScore().ToString()+")";
            if (Input.GetKey(KeyCode.C) && warehouse.WarehouseHP < warehouse.GetmaxWarehouseHP() && player.GetRepairScore() <= gameManager.Score)
            {
                StartCoroutine(player.RepairTimer());
            }
            if (supplyEnabled)
            {
                if (Input.GetKeyDown(KeyCode.V) && player.GetSupplyScore() <= gameManager.Score)
                {
                    StartCoroutine(player.SupplyTimer());
                }
                if (Input.GetKeyDown(KeyCode.X))
                {
                    StartCoroutine(player.UnlockedTimer());
                }
                if (Input.GetKey(KeyCode.Z) && player.PlayerHP < player.GetmaxPlayerHP() && player.GetHeelScore() <= gameManager.Score)
                {
                    StartCoroutine(player.HeelTimer());
                }   
            }
        }
    }


    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player"){
            discriptionText.text = "";
            discriptionText.enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
        warehouse = GameObject.FindGameObjectWithTag("Warehouse").GetComponentInChildren<WarehouseController>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        //discriptionText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        SupplyChanger();
    }

    public void SupplyChanger()
    {
        if (tmpWarehouseHP != warehouse.WarehouseHP)
        {
            if (warehouse.WarehouseHP <= 0)
                supplyEnabled = false;
            if (warehouse.WarehouseHP >= warehouse.GetMaxWarehouseHP() / 5)
                supplyEnabled = true;
            tmpWarehouseHP = warehouse.WarehouseHP;
        }
    }
}
