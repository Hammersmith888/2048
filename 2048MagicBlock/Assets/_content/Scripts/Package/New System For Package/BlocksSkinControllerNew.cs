using UnityEngine;
using System;
using UnityEngine.UI;

public class BlocksSkinControllerNew : MonoBehaviour
{
    /////////////////////////////////////////////////
    [System.Serializable] public struct SkinBlock
    {
        public GameObject lockObject;
        public Animator lockAnimator;
        public GameObject backgorundSpriteObject;
        public GameObject skinSpriteObject;
    }

    [System.Serializable] public struct SkinBlockSprite
    {
        public Sprite skinBlockSprite;
    }

    [System.Serializable] public struct BlockPack
    {
        public GameObject blockPackParentObject;
        public GameObject tick;
        public GameObject spriteObjectPack;
        public Sprite skinPreviewSprite;
        public SkinBlockSprite[] skinBlocksSprites;
    }

    [System.Serializable] public struct SkinPacksInfoUnlock
    {
        public bool[] skinHasUnlock;
        public int[] positionOpened;
        public int count;
        public int countOpenedSkinFree;
        public bool hasOpened;
    }
    /////////////////////////////////////////////////

    [Header("Anothers")]
    [SerializeField] private SnapScrolling _snapScrolling;
    [SerializeField] private Button _coinsPriceButtonComponent;
    [SerializeField] private Image _blockPreviewImgComp;
    [SerializeField] private GameObject _packageGameObject;
    [SerializeField] private GameObject _keyOpenSkinAnimObj;
    [SerializeField] private Transform _keyPositionInstantiated;
    [Header("Animation elements")]
    [SerializeField] private AnimationExpenseCoins _AnimationExpenseCoins;
    [SerializeField] private float _timeAnimationPlay = 0.3f;
    [SerializeField] private float _editAnimPosFinalY = 150; 
    [SerializeField] private Animator _keyAnimator;
    [SerializeField] private string _baseNameKeyAnimation;
    [SerializeField] private string _nameUnlockAnimation;
    [SerializeField] private float _timeBeforeUnlockAnim = 1;
    [SerializeField] private float _timeBeforeShowOpenedSkin = 1;
    [SerializeField] private RectTransform _finalPositionCoinsAnim;
    [Header("Buttons open skin")]
    [SerializeField] private GameObject _btnPriceGameObj;
    [SerializeField] private GameObject _btnFreeGameObj;
    [Header("Coins count references")]
    [SerializeField] private Text _coinsPriceOpenSkinText;
    [SerializeField] private CoinPurse _coinsPurse;
    [Header("Skins proprety")]
    [SerializeField] private Sprite _spriteBackgroundSkinBlockUnlock;
    [SerializeField] private Sprite _goldSpriteBackgroundSkinBlockUnlock;
    [SerializeField] private Sprite _spriteBackgroundSkinBlockLock;
    [SerializeField] private Sprite _goldSpriteBackgroundSkinBlockLock;
    [SerializeField] private BlockPack[] _blocksPack;
    [SerializeField] private SkinBlock[] _skinBlocks;
    [SerializeField] private GameObject[] _questionsBlocksPack;
    [Header("No. Items = No. Skins")]
    [SerializeField] private int[] _priceToOpenSkin;

    private SkinPacksInfoUnlock[] _skinPacksInfoUnlock;
    private int _currentSelectedSkinPack = 0;
    private int _skinElementsIn1Page = 8;
    private int _freeKeyForSkin = 0;
    private int countUnlockBlock = 0;
    private int tmpInteger = 0;
    private int _animLockNrToPlay = 0;
    [HideInInspector]
    public SkinPacksInfoUnlock[] SkinPacksInfoUnlock1 => _skinPacksInfoUnlock;

    [HideInInspector] public static BlocksSkinControllerNew Instance; 

    public bool _isOpenedPackage;

    /////////////////////////////////////////////////
    
    private void Awake()
    {
        Instance = this;

        _currentSelectedSkinPack = PlayerPrefs.GetInt("LastSelectedSkinPack", 1);
        _freeKeyForSkin = PlayerPrefs.GetInt("FreeKeyForSkin", 0);

        if (PlayerPrefs.GetInt("TickStateBool", 0) == 1)
            _blocksPack[_currentSelectedSkinPack].tick.SetActive(true);
        else
            _blocksPack[_currentSelectedSkinPack].tick.SetActive(false);
        
        //trebuie de controlat daca sa modificat structura la SkinPacksInfoUnlock atunci adugam informatia noua

        if (PlayerPrefs.HasKey("SkinPacksInfoUnlock"))
        {
            LoadSkinPacksInfoUnlock();
        }
        else
        {
            _skinPacksInfoUnlock = new SkinPacksInfoUnlock[_blocksPack.Length];

            for (int i = 0; i < _skinPacksInfoUnlock.Length; i++)
            {
                _skinPacksInfoUnlock[i].skinHasUnlock = new bool[_blocksPack[i].skinBlocksSprites.Length];
                _skinPacksInfoUnlock[i].positionOpened = new int[_blocksPack[i].skinBlocksSprites.Length];

                if (i == 0 || i == 1)
                    _skinPacksInfoUnlock[i].hasOpened = true;
            }

            SaveSkinPacksInfoUnlock();
        }
        SelectPackWithSkins(_currentSelectedSkinPack);
        
        ReRenderBtnsWithSelectPack();

        SetActualPriceToBuySkinBtn(GetLastPackToOpenSkin());
    }

    private void SetActualPriceToBuySkinBtn(int currentSelectedSkinPack)
    {
        if (_skinPacksInfoUnlock[currentSelectedSkinPack].skinHasUnlock.Length
            > _skinPacksInfoUnlock[currentSelectedSkinPack].count)
        {
            EnableInteractableBtnOpenSkin();
            SetPriceOpenSkinButton(_priceToOpenSkin[
                        _skinPacksInfoUnlock[currentSelectedSkinPack].count
                        - _skinPacksInfoUnlock[currentSelectedSkinPack].countOpenedSkinFree
                        ]);
        }
        else
        {
            DisableInteractableBtnOpenSkin();
        }
    }
    
    public void SelectPackWithSkins(int idPack)
    {
        PlayerPrefs.SetInt("LastSelectedSkinPack", idPack);

        _blockPreviewImgComp.sprite = _blocksPack[GetLastPackToOpenSkin()].skinPreviewSprite;

        if(_animLockNrToPlay != 24)
            ReRenderSkinsBlocks(idPack);

        if (_animLockNrToPlay == 24)
        {
            ReRenderSkinsBlocks(idPack - 1);
            _animLockNrToPlay = 0;
        }

        if (_skinPacksInfoUnlock.Length == idPack + 1)
        {
            ReRenderSkinsBlocks(idPack);
        }
    }
    
    private void ReRenderSkinsBlocks(int idPackSelected)
    {
        ReRenderCodeBlock(idPackSelected);
        //deleted code)
    }
    
    public void TickCheck(int idPack)
    {
        if (_blocksPack[idPack].tick.activeSelf)
            _blocksPack[idPack].tick.SetActive(false);
        else
            _blocksPack[idPack].tick.SetActive(true);
        //-------------------------------------------
        for (int i = 1; i < _blocksPack.Length; i++)
        {
            if (_blocksPack[i].tick.activeSelf &&
                _blocksPack[i].tick != null &&
                i != idPack
                )
                _blocksPack[i].tick.SetActive(false);
        }
        //-------------------------------------------
        if (_blocksPack[PlayerPrefs.GetInt("LastSelectedSkinPack", 1)].tick.activeSelf)
            PlayerPrefs.SetInt("TickStateBool", 1);
        else
            PlayerPrefs.SetInt("TickStateBool", 0);
        //-------------------------------------------
        if (PlayerPrefs.GetInt("IsEnterInGame", 0) == 1 
            && GameManager.Instance.CurrentScreen.Equals(GameManager.GameScreen.Game))
        {
            Debug.LogError("-------SaveAndLoadSave() packageNew");
            GameManager.Instance.SaveAndLoadSave();
        }

        GameManager.Instance._btnClickSound.PlaySound();
    }
    
    private void SaveSkinPacksInfoUnlock()
    {
        string jsonText = "";
        
        for (int i = 0; i < _skinPacksInfoUnlock.Length; i++)
        {
            jsonText += JsonUtility.ToJson(_skinPacksInfoUnlock[i]);

            if (_skinPacksInfoUnlock.Length - 1 != i)
                jsonText += ",";
        }
        PlayerPrefs.SetString("SkinPacksInfoUnlock", "[" + jsonText + "]");
        Debug.Log("SaveSkinPacksInfoUnlock to Json");
    }
    
    private void LoadSkinPacksInfoUnlock()
    {
        _skinPacksInfoUnlock = GSFUJsonHelper.JsonArray<SkinPacksInfoUnlock>(PlayerPrefs.GetString("SkinPacksInfoUnlock"));
        Debug.Log("LoadSkinPacksInfoUnlock from Json");
    }
    
    public void Add1KeyToOpenSkin()
    {
        _freeKeyForSkin++;
        Debug.LogError("+1 key");
        PlayerPrefs.SetInt("FreeKeyForSkin", _freeKeyForSkin);
        SetPriceOpenSkinButton();
    }

    public void SetPriceOpenSkinButton(int price = 1000065, bool enableInteractableBtn = true)
    {
        if (_freeKeyForSkin > 0 || price == 1000065)
        {
            if (enableInteractableBtn)
                EnableInteractableBtnOpenSkin();
            _btnPriceGameObj.SetActive(false);
            _btnFreeGameObj.SetActive(true);
        }
        else
        {
            _btnPriceGameObj.SetActive(true);
            _btnFreeGameObj.SetActive(false);
            if (enableInteractableBtn)
                EnableInteractableBtnOpenSkin();
            _coinsPriceOpenSkinText.text = price.ToString();
        }

        int lastPackToOpenSkin = GetLastPackToOpenSkin();

        if (!(_skinPacksInfoUnlock[lastPackToOpenSkin].skinHasUnlock.Length
                > _skinPacksInfoUnlock[lastPackToOpenSkin].count))
        {
            DisableInteractableBtnOpenSkin();
        }
    }
    
    public void OpenSkin()
    {
        int lastPackToOpenSkin = GetLastPackToOpenSkin();

        ReRenderCodeBlock(lastPackToOpenSkin);

        if (_priceToOpenSkin.Length 
            > _skinPacksInfoUnlock[lastPackToOpenSkin].count 
            - _skinPacksInfoUnlock[lastPackToOpenSkin].countOpenedSkinFree)
        {
            GameManager.Instance.DisableEventSystem();

            if (_freeKeyForSkin > 0)
            {
                AnalyticsManager.Instance.LogEvent(" block_free");
               
                RandomizeNumberForOpenSkin(lastPackToOpenSkin);

                SetActualPriceToBuySkinBtn(lastPackToOpenSkin);

                _freeKeyForSkin--;
                PlayerPrefs.SetInt("FreeKeyForSkin", _freeKeyForSkin);
                _skinPacksInfoUnlock[lastPackToOpenSkin].countOpenedSkinFree++;

                SaveSkinPacksInfoUnlock();

                ReRenderBtnsWithSelectPack();
            }
            else if (
                _coinsPurse.CoinsCount 
                >= _priceToOpenSkin[_skinPacksInfoUnlock[lastPackToOpenSkin].count
                - _skinPacksInfoUnlock[lastPackToOpenSkin].countOpenedSkinFree]
                )
            {

                SetActualPriceToBuySkinBtn(lastPackToOpenSkin);

                _AnimationExpenseCoins.SetFinalPosition(_finalPositionCoinsAnim);
                _AnimationExpenseCoins.SetEditAnimPosFinalY(_editAnimPosFinalY);
                _AnimationExpenseCoins.SetTimeToMove(_timeAnimationPlay);
                _AnimationExpenseCoins.StartAnimation(0);

                Invoke("SectionCodeOpenSkin", _AnimationExpenseCoins.GetTimeToMove() + 0.6f);
            }
            else
            {
                GameManager.Instance.EnableEventSystem();
                HidePackage();
                Invoke("ShowShop", 0.4f);
            }
            if (GameManager.Instance.CurrentScreen.Equals(GameManager.GameScreen.Game))
            {
                GameManager.Instance.SaveAndLoadSave();
            }
        }
    }

    private void SectionCodeOpenSkin()
    {
        int lastPackToOpenSkin = GetLastPackToOpenSkin();

        if (_skinPacksInfoUnlock[lastPackToOpenSkin].skinHasUnlock.Length
            > _skinPacksInfoUnlock[lastPackToOpenSkin].count)
        {
            _coinsPurse.ChangeCoinsValueForPackageSystem(-_priceToOpenSkin[_skinPacksInfoUnlock[lastPackToOpenSkin].count]);
        }

        RandomizeNumberForOpenSkin(lastPackToOpenSkin);
        EnableInteractableBtnOpenSkin();
        SaveSkinPacksInfoUnlock();

        SetActualPriceToBuySkinBtn(lastPackToOpenSkin);

        ReRenderBtnsWithSelectPack();
    }
    
    private void RandomizeNumberForOpenSkin(int currentSkinPackPar)
    {
        int currentSkinPack = currentSkinPackPar;

        if (_skinPacksInfoUnlock[currentSkinPack].count >= _skinPacksInfoUnlock[currentSkinPack].skinHasUnlock.Length)
            return;

        _snapScrolling.ChoicePaging((int)Math.Floor((decimal)(_skinPacksInfoUnlock[currentSkinPack].count / _skinElementsIn1Page)));

        int localCountVar = _skinPacksInfoUnlock[currentSkinPack].skinHasUnlock.Length;

        for (int i = 0; i < _skinPacksInfoUnlock[currentSkinPack].skinHasUnlock.Length; i++)
        {
            if (_skinPacksInfoUnlock[currentSkinPack].skinHasUnlock[i])
                localCountVar--;
        }

        if (localCountVar == 0)
            return;

        int[] freeSkinForUnlocks = new int[localCountVar];
        localCountVar = 0;

        for (int i = 0; i < _skinPacksInfoUnlock[currentSkinPack].skinHasUnlock.Length; i++)
        {
            if (!_skinPacksInfoUnlock[currentSkinPack].skinHasUnlock[i])
            {
                freeSkinForUnlocks[localCountVar] = i;
                localCountVar++;
            }
        }

        int generatedNumber = GetRandomNumberFromArray(freeSkinForUnlocks);
        Debug.LogError("Skin opened "+generatedNumber);

        _skinPacksInfoUnlock[currentSkinPack].
            skinHasUnlock[generatedNumber] = true;
        _skinPacksInfoUnlock[currentSkinPack].
            positionOpened[_skinPacksInfoUnlock[currentSkinPack].count] = generatedNumber;

        if (_skinPacksInfoUnlock[currentSkinPack].count < _skinBlocks.Length)
            _skinPacksInfoUnlock[currentSkinPack].count++;

        int animNumber = _skinPacksInfoUnlock[currentSkinPack].count;

        if (_skinPacksInfoUnlock[currentSkinPack].count > _skinElementsIn1Page)
        {
            animNumber = (int)(_skinPacksInfoUnlock[currentSkinPack].count 
                - _skinElementsIn1Page 
                * (int)Mathf.Floor(_skinPacksInfoUnlock[currentSkinPack].count / _skinElementsIn1Page));
            if (animNumber == 0) animNumber = _skinElementsIn1Page;
        }

        if (_skinPacksInfoUnlock[currentSkinPack].count >= _skinPacksInfoUnlock[currentSkinPack].skinHasUnlock.Length)
        {
            if (_skinPacksInfoUnlock.Length > currentSkinPack + 1 || _skinPacksInfoUnlock.Length == currentSkinPack + 1)
            {
                if(_skinPacksInfoUnlock.Length > currentSkinPack + 1)
                    _skinPacksInfoUnlock[currentSkinPack + 1].hasOpened = true;
                
                animNumber = 9;
                _animLockNrToPlay = 24;
            }
            
            ReRenderBtnsWithSelectPack();
        }
         
        if (_skinPacksInfoUnlock[currentSkinPack].count 
            >= _skinPacksInfoUnlock[currentSkinPack].skinHasUnlock.Length)
            animNumber = 9;

        AnimateKeyOpenSkin(animNumber, currentSkinPack);
    }

    private void ReRenderForKeyAnimation()
    {
        int lastPackToOpenSkin = GetLastPackToOpenSkin();

        if (_skinPacksInfoUnlock[lastPackToOpenSkin].skinHasUnlock.Length
            > _skinPacksInfoUnlock[lastPackToOpenSkin].count)
        {
            SelectPackWithSkins(lastPackToOpenSkin);
        }
        else if(_skinPacksInfoUnlock[lastPackToOpenSkin].skinHasUnlock.Length
            == _skinPacksInfoUnlock[lastPackToOpenSkin].count
            && _skinPacksInfoUnlock.Length == lastPackToOpenSkin + 1)
        {
            SelectPackWithSkins(lastPackToOpenSkin);
        }
    }

    public Sprite GetSpriteFromSkinPacks(int data)
    {
        if (_skinPacksInfoUnlock[PlayerPrefs.GetInt("LastSelectedSkinPack", 1)].skinHasUnlock[data] && PlayerPrefs.GetInt("TickStateBool", 0) == 1)
        {
            return _blocksPack[PlayerPrefs.GetInt("LastSelectedSkinPack", 1)].skinBlocksSprites[data].skinBlockSprite;
        }
        else
        {
            return _blocksPack[0].skinBlocksSprites[data].skinBlockSprite;
        }
        throw new NotImplementedException();
    }


    #region Help private Functions

    private int GetRandomNumberFromArray(int[] numbers)
    {
        System.Random random = new System.Random();
        int index = random.Next(0, numbers.Length);
        return numbers[index];
    }
   
    private void ReRenderBtnsWithSelectPack()
    {
        for (int i = 1; i < _skinPacksInfoUnlock.Length; i++)
        {
            if (_skinPacksInfoUnlock[i].hasOpened && _skinPacksInfoUnlock[i].count != 0)
            {
                _blocksPack[i].blockPackParentObject.SetActive(true);
                _questionsBlocksPack[i - 1].SetActive(false);
            }
            else
            {
                _blocksPack[i].blockPackParentObject.SetActive(false);
                _questionsBlocksPack[i - 1].SetActive(true);
            }
        }

        SetActualPriceToBuySkinBtn(GetLastPackToOpenSkin());
    }
    
    private void ReRenderCodeBlock(int idPackSelected)
    {
        countUnlockBlock = 0;

        for (int i = 0; i < _blocksPack[idPackSelected].skinBlocksSprites.Length; i++)
        {
            _skinBlocks[i].lockObject.SetActive(true);

            _skinBlocks[i].backgorundSpriteObject.
                GetComponent<Image>().sprite = _spriteBackgroundSkinBlockLock;

            if (_blocksPack[idPackSelected].skinBlocksSprites.Length - 1 == i)
                _skinBlocks[i].backgorundSpriteObject.
                    GetComponent<Image>().sprite = _goldSpriteBackgroundSkinBlockLock;

            _skinBlocks[i].skinSpriteObject.SetActive(false);
            _skinBlocks[i].skinSpriteObject.GetComponent<Image>().sprite = null;

            if (_skinPacksInfoUnlock[idPackSelected].skinHasUnlock[i])
            {
                if (_skinPacksInfoUnlock[idPackSelected].count-1 > countUnlockBlock)
                    _skinBlocks[countUnlockBlock].lockObject.SetActive(false);
                else
                    Invoke("SetFalseLockObject", 0.2f);
                
                tmpInteger = countUnlockBlock;

                _skinBlocks[countUnlockBlock].backgorundSpriteObject.
                    GetComponent<Image>().sprite = _spriteBackgroundSkinBlockUnlock;
                
                if (_blocksPack[idPackSelected].skinBlocksSprites.Length - 1 == i)
                    _skinBlocks[i].backgorundSpriteObject.
                        GetComponent<Image>().sprite = _goldSpriteBackgroundSkinBlockUnlock;

                _skinBlocks[countUnlockBlock].skinSpriteObject.
                    GetComponent<Image>().sprite 
                    = _blocksPack[idPackSelected].
                    skinBlocksSprites[_skinPacksInfoUnlock[idPackSelected].
                    positionOpened[countUnlockBlock]].skinBlockSprite;

                _skinBlocks[countUnlockBlock].skinSpriteObject.SetActive(true);
               
                countUnlockBlock++;
            }
        }
    }

    private void SetFalseLockObject()
    {
        _skinBlocks[tmpInteger].lockObject.SetActive(false);
    }

    public void AnimateKeyOpenSkin(int animNumber, int currentSkinPack = 0)
    {
        Invoke("DisableInteractableBtnOpenSkin", 0.02f);

        _keyAnimator.Play(_baseNameKeyAnimation + animNumber.ToString());
        Invoke("PlayAnimUnlockSkin", _timeBeforeUnlockAnim);
        Invoke("ReRenderForKeyAnimation", _timeBeforeShowOpenedSkin);

        if (currentSkinPack == 0)
        {
            if (_skinPacksInfoUnlock[currentSkinPack].count < _skinBlocks.Length)
                Invoke("EnableInteractableBtnOpenSkin", _timeBeforeShowOpenedSkin);
        }
        else if(_skinPacksInfoUnlock[GetLastPackToOpenSkin()].count < _skinBlocks.Length)
            Invoke("EnableInteractableBtnOpenSkin", _timeBeforeShowOpenedSkin);
    }

    private void PlayAnimUnlockSkin()
    {
        int lastPackToOpenSkin = GetLastPackToOpenSkin();
        
        int indexAnim = _skinPacksInfoUnlock[lastPackToOpenSkin].count - 1;
        
        if (indexAnim < 0) indexAnim = 0;

        if (_animLockNrToPlay == 24)
        {
            indexAnim = _animLockNrToPlay;
            _skinBlocks[indexAnim].lockAnimator.Play(_nameUnlockAnimation);
            GameManager.Instance._skinUnlockedSound.PlaySound();
        }

        if (_skinPacksInfoUnlock[lastPackToOpenSkin].skinHasUnlock.Length
            > _skinPacksInfoUnlock[lastPackToOpenSkin].count)
        {
            _skinBlocks[indexAnim].lockAnimator.Play(_nameUnlockAnimation);
            GameManager.Instance._skinUnlockedSound.PlaySound();
        }

        GameManager.Instance.EnableEventSystem(_skinBlocks[indexAnim].lockAnimator.GetCurrentAnimatorStateInfo(0).length*60);
    }

    private void EnableInteractableBtnOpenSkin() 
    {
        if (!_coinsPriceButtonComponent.interactable)
        {
            //Debug.LogError("InteractableEnable");
            _coinsPriceButtonComponent.interactable = true;
        }
        
    }
    private void DisableInteractableBtnOpenSkin()
    {
        if (_coinsPriceButtonComponent.interactable)
        {
            //Debug.LogError("InteractableDisable");
            _coinsPriceButtonComponent.interactable = false;
        }
    }

    private void HidePackage()
    {
        _packageGameObject.GetComponent<PopupShow>().HidePopup();
    }
    
    private void ShowShop()
    {
        //_shopGameObj.GetComponent<AnimationPanel>().Show();
        GameManager.Instance.OpenShop();
    }

    private int GetLastPackToOpenSkin()
    {
        int lastPackToOpenSkin = -1;

        foreach (var item in _skinPacksInfoUnlock)
            if (item.hasOpened)
                lastPackToOpenSkin++;

        if (lastPackToOpenSkin < 0) lastPackToOpenSkin = 0;

        return lastPackToOpenSkin;
    }
    #endregion
 
}
