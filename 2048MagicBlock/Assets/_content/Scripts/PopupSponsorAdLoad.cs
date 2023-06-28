
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening;
using System;

public class PopupSponsorAdLoad : MonoBehaviour
{
    [SerializeField]
    private Image Arrows;

    Vector3 rotationEuler;
    void Update()
    {
        //rotationEuler += Vector3.forward * 90 * Time.deltaTime; //increment 30 degrees every second
        //Arrows.transform.rotation = Quaternion.Euler(rotationEuler);
    }
    public void Show(Action callback)
    {
        GameManager.Instance.ShowBlackBG();
        StartCoroutine(ClosePopup(callback));
    }

    private IEnumerator ClosePopup(Action callback)
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.GetComponent<AnimationPanel>().Hide();
        GameManager.Instance.HideBlackBG();
        yield return new WaitForSeconds(0.5f);
        callback?.Invoke();
    }

}