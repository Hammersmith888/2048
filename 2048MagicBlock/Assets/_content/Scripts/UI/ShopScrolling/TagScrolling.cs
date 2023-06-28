using UnityEngine;

public class TagScrolling : MonoBehaviour
{
    [SerializeField] private float _snapSpeed;
    [SerializeField] private GameObject[] _instTag = new GameObject[2];
    [SerializeField] private GameObject[] _tagBttn = new GameObject[2];

    private int _selectedTagID;
    private int _tagCount;

    private Vector2[] _tagPos;
    private Vector2 _contentVector;
    private Vector2 _choisePanPos;

    private RectTransform _contentTransform;

    private void Start()
    {
        _tagCount = _instTag.Length;

        _contentTransform = GetComponent<RectTransform>();

        _tagPos = new Vector2[_tagCount];

        for (int i = 0; i < _tagCount; i++)
        {
            _tagPos[i] = -_instTag[i].transform.localPosition;
        }

        ChoiseTag(0);
    }

    private void FixedUpdate()
    {
        _contentVector.x = Mathf.SmoothStep(_contentTransform.anchoredPosition.x, _choisePanPos.x, _snapSpeed * Time.fixedDeltaTime);
        _contentVector.y = 370;
        _contentTransform.anchoredPosition = _contentVector;
    }

    public void ChoiseTag(int indexTag)
    {
        _selectedTagID = indexTag;
        _choisePanPos = _tagPos[_selectedTagID];
    }
}
