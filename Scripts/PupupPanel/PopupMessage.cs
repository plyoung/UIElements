using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
	public class PopupMessage : PopupPanel
	{
		[UnityEngine.Scripting.Preserve]
		public new class UxmlFactory : UxmlFactory<PopupMessage, UxmlTraits> { }

		public string AcceptLabel { get; set; } = "Yes";
		public string CancelLabel { get; set; } = "No";

		private Button confirmButton;
		private Button cancelButton;
		private Label headingLabel;
		private Label messageLabel;

		private System.Action onAccept;
		private System.Action onCancel;

		private const string stylesResource = "GameUI/Styles/PopupMessageStyleSheet";
		private const string ussClassName = "popup-message";
		private const string ussHeadingBack = ussClassName + "__heading-area";
		private const string ussMessageBack = ussClassName + "__text-area";
		private const string ussHeading = ussClassName + "__heading";
		private const string ussMessage = ussClassName + "__text";
		private const string ussButtonsBar = ussClassName + "__buttons-bar";
		private const string ussConfirmButton = ussClassName + "__confirm-button";
		private const string ussCancelButton = ussClassName + "__cancel-button";

		// ------------------------------------------------------------------------------------------------------------

		public PopupMessage()
			: base()
		{
			styleSheets.Add(Resources.Load<StyleSheet>(stylesResource));

			// panel
			mainPanel.AddToClassList(ussClassName);

			// heading
			var ele = new VisualElement();
			ele.AddToClassList(ussHeadingBack);
			mainPanel.Add(ele);

			headingLabel = new Label() { text = "Heading Here" };
			headingLabel.AddToClassList(ussHeading);
			ele.Add(headingLabel);

			// message
			ele = new VisualElement();
			ele.AddToClassList(ussMessageBack);
			mainPanel.Add(ele);

			messageLabel = new Label() { text = "Message Here" };
			messageLabel.AddToClassList(ussMessage);
			ele.Add(messageLabel);

			// button bar
			ele = new VisualElement();
			ele.AddToClassList(ussButtonsBar);
			mainPanel.Add(ele);

			confirmButton = new Button() { text = "Yes" };
			cancelButton = new Button() { text = "No" };
			confirmButton.AddToClassList(ussConfirmButton);
			cancelButton.AddToClassList(ussCancelButton);
			ele.Add(cancelButton);
			ele.Add(confirmButton);

			confirmButton.clicked += OnAcceptButton;
			cancelButton.clicked += OnCancelButton;
		}

		private void OnAcceptButton()
		{
			onAccept?.Invoke();

		}

		private void OnCancelButton()
		{
			onCancel?.Invoke();
		}

		// ------------------------------------------------------------------------------------------------------------

		public void Show(string heading, string message, string cancelLabel)
		{
			Show(heading, message, null, cancelLabel, null, Hide);
		}

		public void Show(string heading, string message, System.Action onAccept = null, System.Action onCancel = null)
		{
			Show(heading, message, AcceptLabel, CancelLabel, onAccept, onCancel);
		}

		public void Show(string heading, string message, string acceptLabel, string cancelLabel, System.Action onAccept, System.Action onCancel = null)
		{
			this.onAccept = onAccept;
			this.onCancel = onCancel;

			headingLabel.text = heading;
			messageLabel.text = message;

			confirmButton.text = acceptLabel;
			cancelButton.text = cancelLabel;

			confirmButton.style.display = onAccept == null ? DisplayStyle.None : DisplayStyle.Flex;
			cancelButton.style.display = onCancel == null ? DisplayStyle.None : DisplayStyle.Flex;

			task?.Pause();
			task = null;

			base.Show();
		}

		public override void Hide()
		{
			onAccept = null;
			onCancel = null;

			base.Hide();
		}

		// ============================================================================================================
	}
}