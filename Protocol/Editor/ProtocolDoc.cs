using EditorPlus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CustomSeanLibEditor("Protocol")]
public class ProtocolDoc : EditorMarkDownWindow
{
    public override string RelativePath => "../../Doc";
    public override bool SearchField => true;
}
