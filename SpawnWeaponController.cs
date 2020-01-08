using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnWeaponController : MonoBehaviour
{
    FirstPersonGunController player;
    WeaponBoxController weaponBox;
    Text discription;
    M4A1Controller M4A1;
    LMGController LMG;
    ScifiGunController ScifiGun;
    AudioSource audioSource;
    public AudioClip getSE;
    bool got = false;
    bool textChangeEnabled = true;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
        weaponBox = GameObject.FindGameObjectWithTag("WeaponBox").GetComponentInChildren<WeaponBoxController>();
        discription = GameObject.Find("Discription").GetComponent<Text>();
        if (weaponBox.choosedIndex == 0)
            M4A1 = player.M4A1;
        if (weaponBox.choosedIndex == 1)
            LMG = player.LMG;
        if (weaponBox.choosedIndex == 2)
            ScifiGun = player.ScifiGun;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!weaponBox.GetOpening())
            Destroy(this.gameObject);
    }

    void OnTriggerStay(Collider collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            discription.enabled = true;
            if (textChangeEnabled)
            {
                textChangeEnabled = false;
                discription.text = "C:Get Weapon";
            }
            if (Input.GetKeyDown(KeyCode.C) && !got)
            {
                got = true;
                audioSource.PlayOneShot(getSE);
                GetWeapon();
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            textChangeEnabled = true;
            discription.text = "";
            discription.enabled = false;
        }
    }

    void DeleteText()
    {
        textChangeEnabled = true;
    }

    void GetWeapon()
    {
        if (this.gameObject.tag == "M4A1" && player.haveWeapon1 != FirstPersonGunController.HaveWeapon.M4A1 && player.haveWeapon2 != FirstPersonGunController.HaveWeapon.M4A1)
        {
            if (player.haveWeapon1 == FirstPersonGunController.HaveWeapon.LMG)
                player.LMGObj.SetActive(false);
            else if (player.haveWeapon1 == FirstPersonGunController.HaveWeapon.HandGun)
            {
                player.HandGunObj.SetActive(false);
                player.HandGun2Obj.SetActive(false);
            }
            else if(player.haveWeapon1 == FirstPersonGunController.HaveWeapon.ScifiGun)
                player.ScifiGunObj.SetActive(false);
            else if (player.haveWeapon1 == FirstPersonGunController.HaveWeapon.Knife)
                player.Knife.SetActive(false);
            player.haveWeapon1 = FirstPersonGunController.HaveWeapon.M4A1;
            player.M4A1Obj.SetActive(true);
            if (player.gotM4A1)
            {
                M4A1.InitGun();
                M4A1.unlockedLevel = 0;
            }
            else
                player.gotM4A1 = true;
            //audioSource.PlayOneShot(getSE);
            textChangeEnabled = true;
            discription.text = "";
            discription.enabled = false;
            Destroy(this.gameObject);
        }

        else if (this.gameObject.tag == "LMG" && player.haveWeapon1 != FirstPersonGunController.HaveWeapon.LMG && player.haveWeapon2 != FirstPersonGunController.HaveWeapon.LMG)
        {
            if (player.haveWeapon1 == FirstPersonGunController.HaveWeapon.M4A1)
                player.M4A1Obj.SetActive(false);
            if (player.haveWeapon1 == FirstPersonGunController.HaveWeapon.HandGun)
            {
                player.HandGunObj.SetActive(false);
                player.HandGun2Obj.SetActive(false);
            }
            if (player.haveWeapon1 == FirstPersonGunController.HaveWeapon.ScifiGun)
                player.ScifiGunObj.SetActive(false);
            if (player.haveWeapon1 == FirstPersonGunController.HaveWeapon.Knife)
                player.Knife.SetActive(false);
            player.haveWeapon1 = FirstPersonGunController.HaveWeapon.LMG;
            player.LMGObj.SetActive(true);
            if (player.gotLMG)
            {
                LMG.InitGun();
                LMG.unlockedLevel = 0;
            }
            else
                player.gotLMG = true;
            //audioSource.PlayOneShot(getSE);
            textChangeEnabled = true;
            discription.text = "";
            discription.enabled = false;
            Destroy(this.gameObject);
        }

        else if (this.gameObject.tag == "ScifiGun" && player.haveWeapon1 != FirstPersonGunController.HaveWeapon.ScifiGun && player.haveWeapon2 != FirstPersonGunController.HaveWeapon.ScifiGun)
        {
            if (player.haveWeapon1 == FirstPersonGunController.HaveWeapon.M4A1)
                player.M4A1Obj.SetActive(false);
            if (player.haveWeapon1 == FirstPersonGunController.HaveWeapon.LMG)
                player.LMGObj.SetActive(false);
            if (player.haveWeapon1 == FirstPersonGunController.HaveWeapon.HandGun)
            {
                player.HandGunObj.SetActive(false);
                player.HandGun2Obj.SetActive(false);
            }
            if (player.haveWeapon1 == FirstPersonGunController.HaveWeapon.Knife)
                player.Knife.SetActive(false);
            player.haveWeapon1 = FirstPersonGunController.HaveWeapon.ScifiGun;
            player.ScifiGunObj.SetActive(true);
            if (player.gotScifiGun)
            {
                ScifiGun.InitGun();
                ScifiGun.unlockedLevel = 0;
            }
            else
                player.gotScifiGun = true;
            //audioSource.PlayOneShot(getSE);
            textChangeEnabled = true;
            discription.text = "";
            discription.enabled = false;
            Destroy(this.gameObject);
        }
        else
        {
            discription.text = "You Already Have This Weapon!";
            Invoke("DeleteText", 1.5f);
        }
    }
}
