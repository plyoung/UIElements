using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
	public class DragManipulator : Clickable
	{
        public bool FreeMoving { get; set; }
        public Vector2 StartMousePosition { get; set; }
        public Vector2 Delta => (lastMousePosition - StartMousePosition);

        private event System.Action onDragging;

        public DragManipulator(System.Action clickHandler, System.Action dragHandler)
            : base(clickHandler, 250, 30)
        {
            FreeMoving = false;
            onDragging += dragHandler;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.RegisterCallback<PointerCancelEvent>(OnPointerCancel);
            base.RegisterCallbacksOnTarget();
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            target.UnregisterCallback<PointerCancelEvent>(OnPointerCancel);
            base.UnregisterCallbacksFromTarget();
        }

        protected override void ProcessDownEvent(EventBase evt, Vector2 localPosition, int pointerId)
        {
            FreeMoving = false;
            StartMousePosition = localPosition;
            base.ProcessDownEvent(evt, localPosition, pointerId);
        }

        protected override void ProcessMoveEvent(EventBase evt, Vector2 localPosition)
        {
            FreeMoving = true;

            base.ProcessMoveEvent(evt, localPosition);

            if (evt.eventTypeId == PointerMoveEvent.TypeId())
            {
                evt.PreventDefault();
            }

            onDragging?.Invoke();
        }

        protected void OnPointerDown(PointerDownEvent evt)
        {
            if (!CanStartManipulation(evt)) return;

            if (IsNotMouseEvent(evt))
            {
                ProcessDownEvent(evt, evt.localPosition, evt.pointerId);
                evt.PreventDefault();
            }
            else
            {
                evt.StopImmediatePropagation();
            }
        }

        protected void OnPointerMove(PointerMoveEvent evt)
        {
            if (IsNotMouseEvent(evt) && active)
            {
                ProcessMoveEvent(evt, evt.localPosition);
            }
        }

        protected void OnPointerUp(PointerUpEvent evt)
        {
            if (IsNotMouseEvent(evt) && active && CanStopManipulation(evt))
            {
                ProcessUpEvent(evt, evt.localPosition, evt.pointerId);
            }
        }

        protected void OnPointerCancel(PointerCancelEvent evt)
        {
            if (IsNotMouseEvent(evt) && CanStopManipulation(evt))
            {
                ProcessCancelEvent(evt, evt.pointerId);
            }
        }

		private static bool IsNotMouseEvent<T>(T evt) where T : PointerEventBase<T>, new()
        {
            // We need to ignore temporarily mouse callback on mobile because they are sent with the wrong type.
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            return true;
#else
            return evt.pointerId != PointerId.mousePointerId;
#endif
        }

        // ============================================================================================================
    }
}