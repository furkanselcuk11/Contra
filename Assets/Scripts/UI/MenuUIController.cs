using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.SceneManagement;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] private Transform _backGround;
    [SerializeField] private float _backGroundDuration = 2f; // Hareket süresi (saniye)
    [SerializeField] private Vector3 _backGroundPos; // Hedef pozisyon (Canvas üzerindeki hedef konumu)

    [SerializeField] private Image _selectImage;
    private Color _selectImageFirstColor;

    [SerializeField] private GameObject _gameEntryUI;
    [SerializeField] private float _gameEntyDuration = 2f;
    bool _isGameEntry = false;
    void Start()
    {
        AudioManager.Instance.PlayMusic("TitleScreen");

        // DOTween kullanarak RectTransform'in pozisyonunu deðiþtir
        _backGround.GetComponent<RectTransform>().DOAnchorPos(_backGroundPos, _backGroundDuration); // Hedef pozisyona 2 saniyede ulaþ        
        StartCoroutine(SelectImageCoroutine());
    }
    void Update()
    {
        GameEntry();
    }

    private void GameEntry()
    {
        if (_isGameEntry && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            // "Enter" tuþuna basýldýðýnda oyunu baþlat
            StartCoroutine(GameStartButton());
        }
    }
    IEnumerator GameStartButton()
    {
        GameEntryUI();
        yield return new WaitForSeconds(_gameEntyDuration);
        GameManager.Instance.GameStart();
    }
    void GameEntryUI()
    {
        _gameEntryUI.SetActive(true);
        _backGround.gameObject.SetActive(false);
        _selectImage.gameObject.SetActive(false);
    }
    IEnumerator SelectImageCoroutine()
    {
        yield return new WaitForSeconds(_backGroundDuration);
        _isGameEntry = true;
        _selectImage.gameObject.SetActive(true);
        _selectImageFirstColor = _selectImage.color;
        while (true)
        {
            // Image'i siyah yap
            yield return DOTween.To(() => _selectImage.color, x => _selectImage.color = x, Color.black, 1.0f).WaitForCompletion();

            // bekle
            yield return new WaitForSeconds(0.5f);

            // Baþlangýç rengine geri dön
            yield return DOTween.To(() => _selectImage.color, x => _selectImage.color = x, _selectImageFirstColor, 1.0f).WaitForCompletion();
        }
    }
}
