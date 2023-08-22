using qASIC.Input.Map;
using UnityEngine;
using qASIC.Input.Players;
using qASIC.Input.Devices;

namespace qASIC.Input.Prompts
{
    //TODO: update text when player or prompt index changes
    //TODO: make prompt index do something
    //TODO: add prompt index animation
    [AddComponentMenu("qASIC/Input/Prompts/Key Text Prompt Display")]
    public class KeyTextPromptDisplay : MonoBehaviour
    {
        public PromptLibrary library;
        public InputMapItemReference item;

        [Header("Type")]
        public int playerIndex;
        public int promptIndex;

        [Header("Text")]
        public TMPro.TMP_Text text;
        public bool useTextFormat;
        public string textFormat;
        public string notFoundText = "Unknown";

        private void OnValidate()
        {
            
        }

        private void Awake()
        {

        }

        private void OnEnable()
        {
            UpdatePrompt();
            InputPlayerManager.OnPlayerCreated += InputPlayerManager_OnPlayerCreated;
            InputPlayerManager.OnPlayerRemoved += InputPlayerManager_OnPlayerRemoved;        
        }

        private void OnDisable()
        {
            InputPlayerManager.OnPlayerCreated -= InputPlayerManager_OnPlayerCreated;
            InputPlayerManager.OnPlayerRemoved -= InputPlayerManager_OnPlayerRemoved;
        }

        private void InputPlayerManager_OnPlayerRemoved(InputPlayer player)
        {
            //Check for correct player index
            if (InputPlayerManager.Players.IndexOf(player) != playerIndex)
                return;

            player.OnLastDeviceChanged -= Player_OnLastDeviceChanged;
        }

        private void InputPlayerManager_OnPlayerCreated(InputPlayer player)
        {
            //Check for correct player index
            if (InputPlayerManager.Players.IndexOf(player) != playerIndex)
                return;

            player.OnLastDeviceChanged += Player_OnLastDeviceChanged;
        }

        private void Player_OnLastDeviceChanged(IInputDevice device)
        {
            UpdatePrompt();
        }

        void UpdatePrompt()
        {
            Debug.Log(InputManager.Map);
            if (library == null)
                return;

            var mapItem = item.GetItem();

            if (mapItem == null)
                return;

            if (!(mapItem is ISupportsPrompts promptItem))
                return;


            var promptData = promptItem.GetPromptData();
            var promptText = notFoundText;
            var isPlayerConnected = InputPlayerManager.Players.IndexInRange(playerIndex) ||
                InputPlayerManager.Players[playerIndex] == null;

            if (promptData.promptGroups.Count > 1 && isPlayerConnected)
            {
                var index = Mathf.Clamp(promptIndex, 0, promptData.promptGroups.Count - 1);

                var keys = library
                    .ForDevice(InputManager.Players[playerIndex].LastDevice ?? InputManager.Players[playerIndex].CurrentDevice)?
                    .GetPromptsFromPaths(promptData.promptGroups[index].keyPaths)
                    .ToDisplayNames();

                promptText = promptItem.KeysToPromptText(keys);
            }

            if (text != null)
                text.text = useTextFormat ?
                    string.Format(textFormat, promptText) :
                    promptText.ToString();
        }
    }
}