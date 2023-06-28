using System.Collections.Generic;
using UnityEngine;

public class PackageDictionary : MonoBehaviour
{
    private List<ThemeType> unlockedThemes = new List<ThemeType>();

    private List<ThemesBlock> themesBlocks = new List<ThemesBlock>();

    private void Awake()
    {
        themesBlocks = new List<ThemesBlock>();

        unlockedThemes.Add(ThemeType.DEFAULT);
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void Refresh() 
    {
        for (int i = 0; i < themesBlocks.Count; i++)
        {
            var themeBlock = themesBlocks[i];
            if (unlockedThemes.Contains(themeBlock.type))
                themesBlocks[i].OpenBlock();
        }
    }

    
}
