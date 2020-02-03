using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumSender : VisParamSender<List<string>>
{
    public EnumSender(string name, bool init) : base(name, init) { }
}
