using Project.GUI.Top;
using Project.Translation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.AppTools
{
    public class AppToolManager : TopMenu
    {
        public TranslationManager translationManager;

        [Space]
        [EditorButton(nameof(PopulateTools))]
        [SerializeField] List<AppToolBase> tools = new List<AppToolBase>();

        protected override string ButtonName => "tools";

        protected override void Awake()
        {
            base.Awake();
            foreach (var item in tools)
            {
                item.SetManager(this);
                item.RunSetup();
            }
        }

        protected override void CreateMenu()
        {
            foreach (var item in tools)
            {
                menu.AppendAction(item.ToolName, args =>
                {
                    item.ShowTool();
                });
            }
        }

#if UNITY_EDITOR
        void PopulateTools()
        {
            var newTools = GetComponentsInChildren<AppToolBase>()
                .Except(tools);

            tools = tools
                .Concat(newTools)
                .ToList();

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
