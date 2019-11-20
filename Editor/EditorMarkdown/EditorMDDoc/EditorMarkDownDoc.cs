using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorPlus
{
    [CustomSeanLibEditor("EditorPlus/EditorMD", order = 100)]
    public class EditorMarkDownDoc : EditorMarkDownWindow
    {
        public override string RelativePath => "../../../../Doc/EditorPlus/EditorMarkdown";
        public override bool SearchField
        {
            get
            {
                return true;
            }
        }
    }
}