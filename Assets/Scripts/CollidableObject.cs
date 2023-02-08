using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollidableObject : PoolableObject
{
    public abstract void Collide(Player _player);
}
