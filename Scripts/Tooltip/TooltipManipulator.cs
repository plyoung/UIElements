using UnityEngine.UIElements;

namespace Game.UI
{
	public class TooltipManipulator : Manipulator
	{
		private Tooltip tooltip;

		public TooltipManipulator(Tooltip tooltip)
		{
			this.tooltip = tooltip;
		}

		protected override void RegisterCallbacksOnTarget()
		{
			target.RegisterCallback<MouseEnterEvent>(MouseIn);
			target.RegisterCallback<MouseOutEvent>(MouseOut);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			target.UnregisterCallback<MouseEnterEvent>(MouseIn);
			target.UnregisterCallback<MouseOutEvent>(MouseOut);
		}

		private void MouseIn(MouseEnterEvent e)
		{
			tooltip.Show(target);
		}

		private void MouseOut(MouseOutEvent e)
		{
			tooltip.Close();
		}

		// ============================================================================================================
	}
}