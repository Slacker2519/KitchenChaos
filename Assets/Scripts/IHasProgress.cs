using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasProgress
{
    public event EventHandler<OnPogressChangedArgs> OnPogressChanged;
    public class OnPogressChangedArgs : EventArgs
    {
        public float progressNormalized;
    }
}
