using EditorPlus;
using UnityEngine;

public class SeanLibEditorYields
{
    public class WaitForEditorEnabled : CustomYieldInstruction
    {
        private SeanLibEditor editor;
        private SeanLibManager window;
        public WaitForEditorEnabled(SeanLibManager window,SeanLibEditor editor)
        {
            this.window = window;
            this.editor = editor;
        }
        public override bool keepWaiting => window.CurrentEditor!=editor;
    }
}