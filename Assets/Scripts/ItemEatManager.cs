using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemEatManager : MonoBehaviour
{
    public Text textUI;
    private RaycastHit hitInfo;
    public int getCoinCount;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Ray screenRay = Camera.main.ScreenPointToRay(mousePos);

            if(Physics.Raycast(screenRay.origin, screenRay.direction * 1000f, out hitInfo))
            {
                if (hitInfo.collider.CompareTag("Coin"))
                {
                    hitInfo.collider.gameObject.SetActive(false);
                    getCoinCount++;

                    textUI.text = "È¹µæÇÑ ÄÚÀÎÀÇ ¼ö :" + getCoinCount;
                }
            }
        }
    }
}
