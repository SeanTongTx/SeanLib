using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EditorPlus
{
    [CustomSeanLibEditor("EditorPlus")]
    public class EditorPlusDoc : EditorMarkDownWindow
    {
        public override string RelativePath => "../../Doc/EditorPlus";
        public override bool EditScript
        {
            get
            {
                return true;
            }
        }
    }
}