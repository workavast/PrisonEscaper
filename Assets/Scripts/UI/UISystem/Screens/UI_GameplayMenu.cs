using PlayerInventory;

namespace UI
{
    public class UI_GameplayMenu : UI_ScreenBase
    {
        public override void _LoadScene(int sceneBuildIndex)
        {
            Inventory.Clear();
            base._LoadScene(sceneBuildIndex);
        }
    }
}