using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public class ButtonHandler : MonoBehaviour
{
    public Action onClick;

    void OnMouseDown()
    {
        onClick?.Invoke();
    }
}