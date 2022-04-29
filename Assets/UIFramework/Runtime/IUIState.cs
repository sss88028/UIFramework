using System.Threading.Tasks;

namespace CCTU.UIFramework
{
	public interface IUIState
	{
		Task OnEnter();

		Task OnPause();

		Task OnResume();

		Task OnExit();
	}

}