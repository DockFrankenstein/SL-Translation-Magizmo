using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Threading;

namespace Project.Utility.UI
{
    public class FieldDropdown : MonoBehaviour
    {
        public TMP_InputField field;
        public TMP_Dropdown dropdown;

        [Label("Field")]
        public string postfix;
        public bool validateTextOnSelect = true;

        string _savedLabel = null;
        string _savedTextValue = null;

        private void Awake()
        {
            if (field != null)
            {
                field.onSelect.AddListener(Field_OnSelect);
                field.onEndEdit.AddListener(Field_OnEndEdit);
            }

            if (dropdown != null)
            {
                dropdown.onValueChanged.AddListener(Dropdown_OnValueChanged);
                SelectDropdownValueFromField();
            }
        }

        private void Field_OnSelect(string text)
        {
            _savedLabel = text;
            if (text.EndsWith(postfix))
                text = text.Substring(0, text.Length - postfix.Length);

            field.SetTextWithoutNotify(text);

            if (validateTextOnSelect)
                ValidateText();

            _savedTextValue = field.text;
        }

        private void Field_OnEndEdit(string text)
        {
            if (!text.EndsWith(postfix))
                field.SetTextWithoutNotify($"{text}{postfix}");

            switch (_savedTextValue == text)
            {
                case true:
                    field.SetTextWithoutNotify(_savedLabel);
                    break;
                case false:
                    SelectDropdownValueFromField();
                    break;
            }

            _savedLabel = null;
            _savedTextValue = null;
        }

        private void Dropdown_OnValueChanged(int index)
        {
            field.text = dropdown.options[index].text;
        }

        void SelectDropdownValueFromField()
        {
            var index = dropdown.options
                .Select(x => x.text)
                .ToList()
                .IndexOf(field.text);

            dropdown.SetValueWithoutNotify(index);
        }

        public void ValidateText()
        {
            var text = "";
            for (int i = 0; i < field.text.Length; i++)
                ValidateCharacter(ref text, field.text[0]);

            field.SetTextWithoutNotify(text);
        }

        bool ValidateCharacter(ref string text, char ch)
        {
            bool correct = false;

            switch (field.characterValidation)
            {
                case TMP_InputField.CharacterValidation.Digit:
                    correct = ch >= '0' && ch <= '9';
                    break;
                case TMP_InputField.CharacterValidation.Decimal:
                case TMP_InputField.CharacterValidation.Integer:
                    var separator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    correct |= text.Length == 0 && ch == '-';
                    correct |= ch >= '0' && ch <= '9';
                    correct |= field.characterValidation == TMP_InputField.CharacterValidation.Integer && ch == separator[0] && !text.Contains(ch);
                    break;
                case TMP_InputField.CharacterValidation.Alphanumeric:
                    correct |= ch >= 'A' && ch <= 'Z';
                    correct |= ch >= 'a' && ch <= 'z';
                    correct |= ch >= '0' && ch <= '9';
                    break;
                //TODO: As I stare into the abyss of visual studio I ask myself the question: why?
                //What's the point in copying the rest of Text Mesh Pro's messy decompiled code?
                //This section of code will probably never be used. And if it will, I can just leave
                //this to myself or someone else in the future. Maybe by that point someone will create
                //the complex technology of letting us use tmpro's validation methods and all this
                //work would go to waste.
                case TMP_InputField.CharacterValidation.Name:
                case TMP_InputField.CharacterValidation.Regex:
                case TMP_InputField.CharacterValidation.EmailAddress:
                case TMP_InputField.CharacterValidation.CustomValidator:
                    throw new System.NotImplementedException("If you're seeing this, good luck.");
            }

            if (correct)
                text = $"{text}{ch}";

            return correct;
        }
    }
}