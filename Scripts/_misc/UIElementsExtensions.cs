using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
	public static class UIElementsExtensions
	{
		public static VisualElement GetDocumentRoot(this VisualElement ele)
		{
			return (ele.parent == null ? ele : ele.parent.GetDocumentRoot());
		}

		// ============================================================================================================
	}
}