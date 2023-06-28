using System.Collections;
using UnityEngine;

public class BlockSkinsSelector : MonoBehaviour
{
    /*
     
    public GameObject HeadSkinUnknown;
    public GameObject HeadSkin1;
    public GameObject HeadSkin2;
    public GameObject HeadSkin3;
     */

    public GameObject[] choiceObjects;

    private void OnEnable()
    {
        for (int i = 0; i < choiceObjects.Length; i++)
        {
            int id = i;
            choiceObjects[id].SetActive(BlocksSkinController.Instance.skins[id].selected);
        }

        BlocksSkinController.Instance.UpdateUI();
    }

    public void SelectSkin(int id)
    {
        if (id == 0) return;

        BlocksSkinController.Instance.SelectSkin(id);
        choiceObjects[id].SetActive(BlocksSkinController.Instance.skins[id].selected);
    }
}
