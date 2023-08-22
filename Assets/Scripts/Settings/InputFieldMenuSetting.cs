using qASIC.SettingsSystem;
using qASIC.SettingsSystem.Menu;
using TMPro;
using UnityEngine;

public class InputFieldMenuSetting : MenuOption
{
    private TMP_InputField inputField;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(x => SetValue(x, false));
            inputField.onEndEdit.AddListener(x => SetValue(x, true));
        }
    }

    public override string GetLabel()
    {
        if (inputField == null) return string.Empty;
        return $"{optionLabelName}{inputField.text}";
    }

    public override void LoadOption()
    {
        if (inputField == null) return;
        if (!OptionsController.TryGetOptionValue(optionName, out string value)) return;
        
        inputField.SetTextWithoutNotify(value);
    }
}