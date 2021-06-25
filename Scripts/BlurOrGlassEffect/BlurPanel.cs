using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
	public class BlurPanel : Image
	{
		public new class UxmlFactory : UxmlFactory<BlurPanel, UxmlTraits> { }

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription { get { yield break; } }
		}

		private VisualElement rootParent;

		// ------------------------------------------------------------------------------------------------------------

		public BlurPanel()
			: base()
		{
			RegisterCallback<GeometryChangedEvent>(UpdateImageRect);
		}

		public void SetImage(Texture img, VisualElement geometryChangedWatch = null)
		{
			// this is to get events when a parent panel is animated.
			// registering to this element alone for that event was not enough.
			geometryChangedWatch?.RegisterCallback<GeometryChangedEvent>(UpdateImageRect);

			image = img;
			scaleMode = ScaleMode.StretchToFill;
			UpdateImageRect(null);
		}

		private void UpdateImageRect(GeometryChangedEvent ev)
		{
			if (image == null)
			{
				return;
			}

			if (rootParent == null)
			{
				rootParent = this.GetDocumentRoot();
			}

			float rw = image.width / rootParent.worldBound.width;
			float rh = image.height / rootParent.worldBound.height;

			float x = worldBound.x * rw;
			float y = worldBound.y * rh;
			float w = worldBound.width * rw;
			float h = worldBound.height * rh;

			sourceRect = new Rect(x, y, w, h);

			MarkDirtyRepaint();
		}

		// ============================================================================================================
	}
}