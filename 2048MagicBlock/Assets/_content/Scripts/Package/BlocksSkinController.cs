using System;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct BlockSkin
{
    public Image uiImage;

    public Sprite sprite;

    public Button button;

    public Sprite[] sprites;

    public int openedSprite;

    public bool selected;

    public Vector3 scale;
}

public class BlocksSkinController : MonoBehaviourSingleton<BlocksSkinController>
{
    public BlockSkin[] skins;

    [SerializeField] private BlocksSkinControllerNew _blocksSkinControllerNew;

    public Sprite unknownSprite;

    public int openedSkin = 0;

    public event Action<int> OnSkinSelected;

    private void Awake()
    {
        skins[0].selected = true;
    }

    public void SelectSkin(int id)
    {
        skins[id].selected = !skins[id].selected;
        OnSkinSelected?.Invoke(id);
    }

    public void UnlockNewSkin()
    {
        if(openedSkin >= 2)
            return;

        openedSkin++;

        UpdateUI();
    }

    public void UpdateUI()
    {
        for (int i = 0; i < skins.Length; i++)
        {
            var opened = i <= openedSkin;
            skins[i].button.interactable = opened;
            skins[i].uiImage.sprite = opened ? skins[i].sprite : unknownSprite;

        }
    }

    public Sprite GetSpriteByValue(int data, out Vector3 scale)
    {
        if (data == 0 || data >= 25)
        {
            scale = Vector3.one;
            return null;
        }

      
        BlockSkin selSkin = skins[0];
        
        /*for (int i = skins.Length-1; i > 0; i--)
        {
            var skin = skins[i];

            if (!skin.selected) continue;

            if(skin.openedSprite >= data)
            {
                selSkin = skin;
                break;
            }    
        }*/
        
        scale = selSkin.scale;
        //return selSkin.sprites[data - 1];

        /* M.I. code ->*/
        return _blocksSkinControllerNew.GetSpriteFromSkinPacks(data);
        /* M.I. code <-*/
    }
}

