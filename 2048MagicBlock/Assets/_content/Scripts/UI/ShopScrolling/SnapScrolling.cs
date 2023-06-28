using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SnapScrolling : MonoBehaviour
{
    [SerializeField] private float _snapSpeed;

    [SerializeField] private int _scrollSpeed;

    [SerializeField] private GameObject[] _instPans;
    [SerializeField] private GameObject[] _paging;

    [SerializeField] private ScrollRect _scrollRect;

    [SerializeField] private float _comtentVectorPosition;

    private int _selectedPanID;
    private int _panCount;

    public int selectPanID => _selectedPanID;

    private Vector2[] _panPos;
    private Vector2 _contentVector;
    private Vector2 _choisePanPos;

    private RectTransform _contentTransform;

    private bool _isScrolling;
    private bool _lockScrolling;

    private void Start()
    {
        _panCount = _instPans.Length;

        _contentTransform = GetComponent<RectTransform>();

        _panPos = new Vector2[_panCount];

        for (int i = 0; i < _panCount; i++)
        {
            _panPos[i] = -_instPans[i].transform.localPosition;
        }
    }

    private void FixedUpdate()
    {
        if (_contentTransform.anchoredPosition.x >= _panPos[0].x && !_isScrolling || _contentTransform.anchoredPosition.x <= _panPos[_panPos.Length - 1].x && !_isScrolling)
            _scrollRect.inertia = false;

        float nearestPos = float.MaxValue;

        for (int i = 0; i < _panCount; i++)
        {
            float distance = Mathf.Abs(_contentTransform.anchoredPosition.x - _panPos[i].x);

            if (distance < nearestPos)
            {
                nearestPos = distance;
                _selectedPanID = i;
            }
        }

        float scrollVelocity = Mathf.Abs(_scrollRect.velocity.x);
        if (scrollVelocity < _scrollSpeed && !_isScrolling) _scrollRect.inertia = false;

        if (_isScrolling || scrollVelocity > _scrollSpeed) return;

        if (_lockScrolling != true)
        {
            _contentVector.x = Mathf.SmoothStep(_contentTransform.anchoredPosition.x, _panPos[_selectedPanID].x, _snapSpeed * Time.fixedDeltaTime);
            _contentVector.y = _comtentVectorPosition;
            _contentTransform.anchoredPosition = _contentVector;
            PagingController(_selectedPanID);
        }

        else
        {
            _contentVector.x = Mathf.SmoothStep(_contentTransform.anchoredPosition.x, _choisePanPos.x, _snapSpeed * Time.fixedDeltaTime);
            _contentVector.y = _comtentVectorPosition;
            _contentTransform.anchoredPosition = _contentVector;
            PagingController(_selectedPanID);
        }
    }

    public void Scrolling(bool scroll)
    {
        _isScrolling = scroll;

        if (scroll) _scrollRect.inertia = true;
    }

    private void PagingController(int pagingID)
    {
        for (int i = 0; i < _paging.Length; i++)
        {
            _paging[i].gameObject.SetActive(false);
        }
        _paging[pagingID].gameObject.SetActive(true);
    }

    public void ChoicePaging(int indexPaging)
    {
        _lockScrolling = true;
        _selectedPanID = indexPaging;
        _choisePanPos = _panPos[_selectedPanID];
        StartCoroutine(LockScrolling(1));
    }

    private IEnumerator LockScrolling(float lockTime)
    {
        yield return new WaitForSeconds(lockTime);
        _lockScrolling = false;
    }
}
