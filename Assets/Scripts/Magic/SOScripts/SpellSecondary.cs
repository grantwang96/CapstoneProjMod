﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSecondary : ScriptableObject {

    public virtual void MessUp(Transform user, Missile projectile) // manipulate how the projectile is created
    {

    }

    public virtual void OnHit(Transform user, Missile projectile) // when the projectile hits something
    {

    }
}
