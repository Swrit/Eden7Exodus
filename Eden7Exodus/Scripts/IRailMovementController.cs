using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRailMovementController
{
    public event EventHandler OnControlUp;
    public event EventHandler OnControlDown;

}
