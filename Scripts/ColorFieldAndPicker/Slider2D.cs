using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
	public class Slider2D : BaseField<Vector2>
	{
		[UnityEngine.Scripting.Preserve]
		public new class UxmlFactory : UxmlFactory<Slider2D, UxmlTraits> { }

		public Vector2 MinValue { get; private set; }
		public Vector2 MaxValue { get; private set; }

		private VisualElement dragElement;
		private VisualElement dragBorderElement;
		private DragManipulator dragger;
		private Vector2 dragElementStartPos;

		private const string stylesResource = "GameUI/Styles/Slider2DStyleSheet";
		private const string ussFieldName = "slider-2d";
		private const string ussDragger = ussFieldName + "__dragger";
		private const string ussDraggerBorder = ussFieldName + "__dragger-border";

		// ------------------------------------------------------------------------------------------------------------

		public Slider2D()
			: this(Vector2.zero, Vector2.one)
		{ }

		public Slider2D(Vector2 min, Vector2 max)
			: base(null, null)
		{
			MinValue = min;
			MaxValue = max;

			styleSheets.Add(Resources.Load<StyleSheet>(stylesResource));
			AddToClassList(ussFieldName);

			dragBorderElement = new VisualElement() { name = "dragger-border", pickingMode = PickingMode.Ignore };
			dragBorderElement.AddToClassList(ussDraggerBorder);
			Add(dragBorderElement);

			dragElement = new VisualElement() { name = "dragger", pickingMode = PickingMode.Ignore };
			dragElement.AddToClassList(ussDragger);
			dragElement.RegisterCallback<GeometryChangedEvent>(UpdateDraggerPosition);
			Add(dragElement);

			dragger = new DragManipulator(OnDraggerClicked, OnDraggerDragged);
			pickingMode = PickingMode.Position;
			this.AddManipulator(dragger);
		}

		public override void SetValueWithoutNotify(Vector2 newValue)
		{
			var clampedValue = GetClampedValue(newValue);
			base.SetValueWithoutNotify(clampedValue);
			UpdateDraggerPosition();
		}

		private void UpdateDraggerPosition(GeometryChangedEvent evt)
		{
			// Only affected by dimension changes
			if (evt.oldRect.size == evt.newRect.size) return;
			UpdateDraggerPosition();
		}

		private void UpdateDraggerPosition()
		{
			if (panel == null) return; // skip the position calculation and wait for a layout pass if no panel

			var normalizedVal = NormalizeValue();
			float newLeft = normalizedVal.x * resolvedStyle.width;
			float newTop = (1f - normalizedVal.y) * resolvedStyle.height;
			if (float.IsNaN(newLeft) || float.IsNaN(newTop)) return;

			var currTop = dragElement.transform.position.y;
			var currLeft = dragElement.transform.position.x;

			if (!Similar(currLeft, newLeft) || !Similar(currTop, newTop))
			{
				var newPos = new Vector3(newLeft - dragElement.resolvedStyle.width * 0.5f, newTop - dragElement.resolvedStyle.height * 0.5f, 0f);
				dragElement.transform.position = newPos;
				dragBorderElement.transform.position = newPos;
			}
		}

		private Vector2 NormalizeValue()
		{
			return (value - MinValue) / (MaxValue - MinValue);
		}

		private bool Similar(float a, float b)
		{
			return Mathf.Abs(b - a) < 1f;
		}

		private Vector2 GetClampedValue(Vector2 newValue)
		{
			Vector2 lowest = MinValue, highest = MaxValue;

			if (lowest.x.CompareTo(highest.x) > 0)
			{
				var t = lowest.x;
				lowest.x = highest.x;
				highest.x = t;
			}

			if (lowest.y.CompareTo(highest.y) > 0)
			{
				var t = lowest.y;
				lowest.y = highest.y;
				highest.y = t;
			}

			return new Vector2(Clamp(newValue.x, lowest.x, highest.x), Clamp(newValue.y, lowest.y, highest.y));
		}

		private float Clamp(float value, float lowBound, float highBound)
		{
			float result = value;

			if (lowBound.CompareTo(value) > 0)
			{
				result = lowBound;
			}
			else if (highBound.CompareTo(value) < 0)
			{
				result = highBound;
			}

			return result;
		}

		private void OnDraggerClicked()
		{
			if (dragger.FreeMoving) return;

			var x = dragger.StartMousePosition.x - (dragElement.resolvedStyle.width * 0.5f);
			var y = dragger.StartMousePosition.y - (dragElement.resolvedStyle.height * 0.5f);
			dragElementStartPos = new Vector2(x, y);

			ComputerValueFrom(dragElementStartPos);
		}

		private void OnDraggerDragged()
		{
			if (dragger.FreeMoving)
			{
				ComputerValueFrom(dragElementStartPos + dragger.Delta);
			}
		}

		private void ComputerValueFrom(Vector2 pos)
		{
			var totalWdith = resolvedStyle.width - dragElement.resolvedStyle.width;
			var totalHeight = resolvedStyle.height - dragElement.resolvedStyle.height;
			if (Mathf.Abs(totalWdith) < Mathf.Epsilon || Mathf.Abs(totalHeight) < Mathf.Epsilon)
			{
				return;
			}

			float nPosX = Mathf.Max(0f, Mathf.Min(pos.x, totalWdith)) / totalWdith;
			float nPosY = 1f - Mathf.Max(0f, Mathf.Min(pos.y, totalHeight)) / totalHeight;

			value = new Vector2(Mathf.LerpUnclamped(MinValue.x, MaxValue.x, nPosX),
								Mathf.LerpUnclamped(MinValue.y, MaxValue.y, nPosY));

		}

		// ============================================================================================================
	}
}