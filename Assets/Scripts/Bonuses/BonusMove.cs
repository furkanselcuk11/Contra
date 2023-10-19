using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class BonusMove : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private Vector3 _endPos;
    [SerializeField] private float _moveDuration = 2f; // süresi    

    public string Name { get => _name; set => _name = value; }

    public void Move()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(_endPos.x, _endPos.y, _endPos.z);
        transform.DOMove(endPos, _moveDuration);
    }

}
