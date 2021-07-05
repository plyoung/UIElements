using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
	public class PopupTextField : PopupPanel
	{
		[UnityEngine.Scripting.Preserve]
		public new class UxmlFactory : UxmlFactory<PopupTextField, UxmlTraits> { }

		public string SubmitLabel { get; set; } = "Submit";
		public string CancelLabel { get; set; } = "Cancel";

		private Button submitButton;
		private Button cancelButton;
		private Label headingLabel;
		private Label messageLabel;
		private TextField textField;

		private System.Action<string> onSubmit;
		private System.Action onCancel;

		private const string stylesResource = "GameUI/Styles/PopupTextFieldStyleSheet";
		private const string ussClassName = "popup-textfield";
		private const string ussHeadingBack = ussClassName + "__heading-area";
		private const string ussMessageBack = ussClassName + "__text-area";
		private const string ussHeading = ussClassName + "__heading";
		private const string ussMessage = ussClassName + "__text";
		private const string ussTextField = ussClassName + "__edit";
		private const string ussButtonsBar = ussClassName + "__buttons-bar";
		private const string ussSubmitButton = ussClassName + "__submit-button";
		private const string ussCancelButton = ussClassName + "__cancel-button";

		// ------------------------------------------------------------------------------------------------------------

		public PopupTextField()
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

			textField = new TextField();
			textField.AddToClassList(ussTextField);
			ele.Add(textField);

			// button bar
			ele = new VisualElement();
			ele.AddToClassList(ussButtonsBar);
			mainPanel.Add(ele);

			submitButton = new Button() { text = "Submit" };
			cancelButton = new Button() { text = "Cancel" };
			submitButton.AddToClassList(ussSubmitButton);
			cancelButton.AddToClassList(ussCancelButton);
			ele.Add(cancelButton);
			ele.Add(submitButton);

			submitButton.clicked += OnSubmitButton;
			cancelButton.clicked += OnCancelButton;
		}

		private void OnSubmitButton()
		{
			onSubmit?.Invoke(textField.value);
		}

		private void OnCancelButton()
		{
			onCancel?.Invoke();
		}

		// ------------------------------------------------------------------------------------------------------------

		public void Show(string heading, string message, string initText, int maxLen, System.Action<string> onAccept = null, System.Action onCancel = null)
		{
			Show(heading, message, initText, maxLen, SubmitLabel, CancelLabel, onAccept, onCancel);
		}

		public void Show(string heading, string message, string initText, int maxLen, string acceptLabel, string cancelLabel, System.Action<string> onAccept, System.Action onCancel = null)
		{
			this.onSubmit = onAccept;
			this.onCancel = onCancel;

			headingLabel.text = heading;
			messageLabel.text = message;
			
			textField.SetValueWithoutNotify(initText);
			textField.maxLength = maxLen;

			submitButton.text = acceptLabel;
			cancelButton.text = cancelLabel;

			submitButton.style.display = onAccept == null ? DisplayStyle.None : DisplayStyle.Flex;
			cancelButton.style.display = onCancel == null ? DisplayStyle.None : DisplayStyle.Flex;

			task?.Pause();
			task = null;

			base.Show();
		}

		public override void Hide()
		{
			onSubmit = null;
			onCancel = null;

			base.Hide();
		}

		// ============================================================================================================
	}
}