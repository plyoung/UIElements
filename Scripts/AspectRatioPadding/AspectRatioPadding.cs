using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
	public class AspectRatioPadding : VisualElement
	{
		[UnityEngine.Scripting.Preserve]
		public new class UxmlFactory : UxmlFactory<AspectRatioPadding, UxmlTraits> { }

		[UnityEngine.Scripting.Preserve]
		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			UxmlIntAttributeDescription width = new UxmlIntAttributeDescription { name = "width", defaultValue = 16 };
			UxmlIntAttributeDescription height = new UxmlIntAttributeDescription { name = "height", defaultValue = 9 };

			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get { yield break; }
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				AspectRatioPadding ele = ve as AspectRatioPadding;
				ele.RatioWidth = width.GetValueFromBag(bag, cc);
				ele.RatioHeight = height.GetValueFromBag(bag, cc);
			}
		}

		public int RatioWidth { get; private set; } = 16;
		public int RatioHeight { get; private set; } = 9;

		private VisualElement leftPadding;
		private VisualElement rightPadding;

		// ------------------------------------------------------------------------------------------------------------

		public AspectRatioPadding()
		{
			style.flexDirection = FlexDirection.Row;

			leftPadding = new VisualElement() { name = "AspectRatioPadding-Left" };
			rightPadding = new VisualElement() { name = "AspectRatioPadding-Right" };

			Add(leftPadding);
			Add(rightPadding);

			RegisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
			RegisterCallback<AttachToPanelEvent>(OnAttachToPanelEvent);
		}

		private void OnGeometryChangedEvent(GeometryChangedEvent e)
		{
			UpdateElements();
		}

		private void OnAttachToPanelEvent(AttachToPanelEvent e)
		{
			UpdateElements();
		}

		public void UpdateElements()
		{
			if (RatioWidth <= 0.0f || RatioHeight <= 0.0f)
			{
				leftPadding.style.width = 0;
				rightPadding.style.width = 0;
				Debug.LogError($"[AspectRatioPadding] Invalid width:{RatioWidth} or height:{RatioHeight}");
				return;
			}

			if (float.IsNaN(resolvedStyle.width) || float.IsNaN(resolvedStyle.height))
			{
				return;
			}

			var designRatio = (float)RatioWidth / RatioHeight;
			var currRatio = resolvedStyle.width / resolvedStyle.height;
			var diff = currRatio - designRatio;

			if (diff > 0.01f)
			{
				var w = (resolvedStyle.width - (resolvedStyle.height * designRatio)) * 0.5f;
				leftPadding.style.width = w;
				rightPadding.style.width = w;
			}
			else
			{
				leftPadding.style.width = 0;
				rightPadding.style.width = 0;
			}

			// make sure the padding elements are at corerct positions in hierarchy
			leftPadding.SendToBack();
			rightPadding.BringToFront();
		}

		// ============================================================================================================
	}
}