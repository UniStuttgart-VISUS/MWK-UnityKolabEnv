using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vec3Sender : VisParamSender<List<Vector3>>
{
    public Vec3Sender(string name, bool init) : base(name, init) { }
}
