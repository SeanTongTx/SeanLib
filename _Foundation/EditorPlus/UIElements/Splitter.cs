using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace UIElements
{
    public class Splitter : VisualElement
    {
        //工厂
        public class Factory : UxmlFactory<Splitter, Traits> { }
        //Xml描述
        public class Traits : UxmlTraits
        {
            //XML属性
            UxmlStringAttributeDescription Target = new UxmlStringAttributeDescription() { name = "Target" };
            //XML属性
            UxmlBoolAttributeDescription Horizon = new UxmlBoolAttributeDescription() { name = "Horizon" };
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                //这表明StatusBar没有子项
                get { yield break; }
            }
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                Splitter splitter = ((Splitter)ve);
                splitter.TargetName = Target.GetValueFromBag(bag, cc);
                splitter.isHorizon = Horizon.GetValueFromBag(bag, cc);
                splitter.Target = splitter.Q(splitter.TargetName);

            }
        }
        //鼠标事件处理器
        private class SquareResizer : MouseManipulator
        {
            private Vector2 m_Start;

            protected bool m_Active;

            private Splitter m_Splitter;

            public SquareResizer(Splitter splitter)
            {
                this.m_Splitter = splitter;
                base.activators.Add(new ManipulatorActivationFilter
                {
                    button = MouseButton.LeftMouse
                });
                this.m_Active = false;
            }

            protected override void RegisterCallbacksOnTarget()
            {
                base.target.RegisterCallback<MouseDownEvent>(new EventCallback<MouseDownEvent>(this.OnMouseDown), TrickleDown.NoTrickleDown);
                base.target.RegisterCallback<MouseMoveEvent>(new EventCallback<MouseMoveEvent>(this.OnMouseMove), TrickleDown.NoTrickleDown);
                base.target.RegisterCallback<MouseUpEvent>(new EventCallback<MouseUpEvent>(this.OnMouseUp), TrickleDown.NoTrickleDown);
            }

            protected override void UnregisterCallbacksFromTarget()
            {
                base.target.UnregisterCallback<MouseDownEvent>(new EventCallback<MouseDownEvent>(this.OnMouseDown), TrickleDown.NoTrickleDown);
                base.target.UnregisterCallback<MouseMoveEvent>(new EventCallback<MouseMoveEvent>(this.OnMouseMove), TrickleDown.NoTrickleDown);
                base.target.UnregisterCallback<MouseUpEvent>(new EventCallback<MouseUpEvent>(this.OnMouseUp), TrickleDown.NoTrickleDown);
            }

            protected void OnMouseDown(MouseDownEvent e)
            {
                if (this.m_Active)
                {
                    e.StopImmediatePropagation();
                }
                else if (base.CanStartManipulation(e))
                {
                    this.m_Start = e.localMousePosition;
                    this.m_Active = true;
                    base.target.CaptureMouse();
                    e.StopPropagation();
                }
            }

            protected void OnMouseMove(MouseMoveEvent e)
            {
                if (this.m_Active && base.target.HasMouseCapture())
                {
                    Vector2 vector = e.localMousePosition - this.m_Start;
                    this.m_Splitter.Target.style.width = this.m_Splitter.Target.layout.width + vector.x;
                    e.StopPropagation();
                }
            }

            protected void OnMouseUp(MouseUpEvent e)
            {
                if (this.m_Active && base.target.HasMouseCapture() && base.CanStopManipulation(e))
                {
                    this.m_Active = false;
                    base.target.ReleaseMouse();
                    e.StopPropagation();
                 //  this.m_Splitter.leftPaneWidth = this.m_Splitter.leftPane.resolvedStyle.width;
                    /*  this.m_Splitter.SaveViewData();*/
                }
            }
        }

        public bool isHorizon;
        public string TargetName;
        public VisualElement Target;
        public Splitter()
        {
            var dragBar = (new VisualElement() { name = "SplitterDragBar" });
            dragBar.AddManipulator(new SquareResizer(this));
            this.Add(dragBar);
        }
    }
}