using EasyButtons;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ThemeType
{
    NONE,
    DEFAULT,
    EGYPT,
    ICE,
    JUNGLES,
    LAVA,
    NEON,
    SKY,
    SPACE,
    UNDERWATER
}

[System.Serializable]
public class Theme
{
    public ThemeType type;

    public Sprite background;
    public Sprite grid;
    public Sprite panel1;
    public Sprite panel2;
    //public Image backgroundImgBlock;
    public TextMeshProUGUI nameThemeText;
}

public class ThemeController : MonoBehaviourSingleton<ThemeController>
{
    private int themeIndex = 0;
    private ThemesLogicBlock _choosedTheme;
    private bool firstDefaultSelect = false;

    private void Awake()
    {
        for (int i = 1; i < themes.Length; i++)
        {
            themes[i].nameThemeText.color = colorTextDontSelectedTheme;
        }

        themeIndex = PlayerPrefs.GetInt("CurrentThemeApplied", 1);
        currentTheme = (ThemeType)themeIndex;

        ApplyTheme();
    }
    public void SetTextColorDefault()
    {
        themes[themeIndex].nameThemeText.color = colorTextDontSelectedTheme;
    }
    public void SetNewTheme(ThemesLogicBlock choosedTheme)
    {
        if(_choosedTheme!=null)
            _choosedTheme.CancelTheme();
        _choosedTheme = choosedTheme;

        if (firstDefaultSelect)
            GameManager.Instance._btnClickSound.PlaySound();

        firstDefaultSelect = true;
    }

    [Button("ApplyTheme")]
    public void ApplyTheme()
    {
        if (currentTheme == ThemeType.NONE) return;

        

        var theme = themes.FirstOrDefault(n => n.type == currentTheme);

        if (theme == null) return;

        startFromBG.sprite = backgroundImage.sprite = theme.background;
        startFromGrid.sprite = grid.sprite = theme.grid;

        for (int i = 0; i < panel1.Length; i++)
        {
            panel1[i].sprite = theme.panel1;
        }

        for (int i = 0; i < panel2.Length; i++)
        {
            panel2[i].sprite = theme.panel2;
        }

        theme.nameThemeText.color = colorTextSelectedTheme;

        themeIndex = (int)currentTheme;

        PlayerPrefs.SetInt("CurrentThemeApplied", themeIndex);
    }

    public Theme[] themes;

    public ThemeType currentTheme;
    public Color colorTextDontSelectedTheme;
    public Color colorTextSelectedTheme;
    //public Sprite bgSpriteSelectedTheme;
    //public Sprite bgSpriteNonSelectedTheme;

    [SerializeField]
    private Image backgroundImage;
    [SerializeField]
    private Image grid;
    [SerializeField]
    private Image[] panel1;
    [SerializeField]
    private Image[] panel2;
    [SerializeField]
    private Image startFromBG;
    [SerializeField]
    private Image startFromGrid;

}
