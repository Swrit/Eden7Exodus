using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIWindowBase : MonoBehaviour
{
    public abstract void Show();

    public abstract void Hide();

    public abstract bool ShouldPause();

    public virtual void UIManagerRegister(UIManager uIManager) { }
}
