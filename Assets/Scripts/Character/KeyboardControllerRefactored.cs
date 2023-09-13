using Character;
using UI;
using UnityEngine;

namespace Character
{
    public class KeyboardControllerRefactored : MonoBehaviour
    {
        private UIInventory _uiInventory;
        private Player _player;

        private void Start()
        {
            _uiInventory = UIInventory.Instance;
            _player = Player.Instance;
        }

        private void Update()
        {
            
            _player.Move(Input.GetAxis("Horizontal"));

            if (Input.GetMouseButtonDown(1))
                _player.Attack();

            if (Input.GetKeyDown(KeyCode.R))
                _player.ThrowWeapon();

            if (Input.GetKeyDown(KeyCode.Space))
                _player.Jump();

            if (Input.GetKeyDown(KeyCode.LeftShift))
                _player.Dash();
            
            if (_player && _player.IsAlive && _uiInventory)
            {
                if (Input.GetKeyDown(KeyCode.I))
                {
                    _uiInventory.ToggleBagDisplay();
                }
                
                //TODO: make item using from active slots

                // if (Input.GetKeyDown(KeyCode.E))
                // {
                //     _player.Inventory.UseActiveItem();
                // }
                
            }

        }
    }
}