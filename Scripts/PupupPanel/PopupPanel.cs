using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
	public class PopupPanel : VisualElement
	{
		[UnityEngine.Scripting.Preserve]
		public new class UxmlFactory : UxmlFactory<PopupPanel, UxmlTraits> { }

		[UnityEngine.Scripting.Preserve]
		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			private readonly UxmlBoolAttributeDescription startVisible = new UxmlBoolAttributeDescription { name = "start-visible", defaultValue = false };
			private readonly UxmlIntAttributeDescription fadeTime = new UxmlIntAttributeDescription { name = "fade-time", defaultValue = 30 };

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var item = ve as PopupPanel;
				var vis = startVisible.GetValueFromBag(bag, cc);
				item.FadeTime = fadeTime.GetValueFromBag(bag, cc);

				item.SetStartVisibility(vis);
			}
		}

		public int FadeTime { get; private set; } = 30;

		public override VisualElement contentContainer => mainPanel;

		protected BlurPanel mainPanel;
		protected IVisualElementScheduledItem task;

		private const string stylesResource = "GameUI/Styles/PopupPanelStyleSheet";
		private const string ussClassName = "popup-panel";
		private const string ussContainer = ussClassName + "__container";

		// ------------------------------------------------------------------------------------------------------------

		public PopupPanel()
		{
			styleSheets.Add(Resources.Load<StyleSheet>(stylesResource));
			AddToClassList(ussContainer);
			pickingMode = PickingMode.Position;

			// panel
			mainPanel = new BlurPanel();
			mainPanel.pickingMode = PickingMode.Position;
			mainPanel.focusable = true;
			mainPanel.AddToClassList(ussClassName);
			hierarchy.Add(mainPanel);

			SetStartVisibility(false);
		}

		// ------------------------------------------------------------------------------------------------------------

		public void SetBlurSource(Texture texture)
		{
			mainPanel.SetImage(texture);
		}

		public virtual void Show()
		{
			task?.Pause();
			task = null;

			if (FadeTime > 0.0f)
			{
				style.visibility = Visibility.Visible;
				style.opacity = 0f;
				task = schedule
					.Execute(() => style.opacity = Mathf.Clamp01(resolvedStyle.opacity + 0.1f))
					.Every(FadeTime) // ms	
					.Until(() => resolvedStyle.opacity >= 1.0f);
			}
			else
			{
				style.visibility = Visibility.Visible;
				style.opacity = 1f;
			}

			mainPanel.Focus();
		}

		public virtual void Hide()
		{
			task?.Pause();
			task = null;
			
			mainPanel.Blur();

			if (FadeTime > 0.0f)
			{
				task = schedule
					.Execute(() =>
					{
						var o = Mathf.Clamp01(resolvedStyle.opacity - 0.1f);
						style.opacity = o;
						if (o <= 0.0f) style.visibility = Visibility.Hidden;
					})
					.Every(FadeTime) // ms	
					.Until(() => resolvedStyle.opacity <= 0.0f);
			}
			else
			{
				style.visibility = Visibility.Hidden;
				style.opacity = 0f;
			}
		}

		protected void SetStartVisibility(bool isVisible)
		{
			if (isVisible)
			{
				style.visibility = Visibility.Visible;
				style.opacity = 1f;
			}
			else
			{
				style.visibility = Visibility.Hidden;
				style.opacity = 0f;
			}
		}

		// ============================================================================================================
	}
}