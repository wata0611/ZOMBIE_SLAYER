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
            if (Input.GetKey(KeyCode.R) && warehouse.WarehouseHP < warehouse.GetmaxWarehouseHP() && player.GetRepairScore() <= gameManager.Score)
            {
                StartCoroutine(player.RepairTimer());
            }
            if (supplyEnabled)
            {
                if (Input.GetKeyDown(KeyCode.Z) && player.GetSupplyScore() <= gameManager.Score)
                {
                    StartCoroutine(player.SupplyTimer());
                }
                if (Input.GetKeyDown(KeyCode.U))
                {
                    StartCoroutine(player.UnlockedTimer());
                }
                if (Input.GetKey(KeyCode.H) && player.PlayerHP < player.GetmaxPlayerHP() && player.GetHeelScore() <= gameManager.Score)
                {
                    StartCoroutine(player.HeelTimer());
                }   
            }
        }
    }

    void OnTrigerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            discriptionText.enabled = true;
            discriptionText.text = "r:repair\n" + "z:supply\n" + "h:heel\n" + "u:unlocked";
        }
    }

    void OnTrigerExit(Collider collider)
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
        discriptionText.enabled = false;
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
