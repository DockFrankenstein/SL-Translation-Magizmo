using Project.Translation;
using Project.Translation.ImportAndExport;
using Project.Undo;
using System.Collections.Generic;
using UnityEngine;
using System;
using qASIC.Input;

namespace Project.GUI.ImportAndExport
{
    public class ImportAndExportManager : MonoBehaviour
    {
        [Label("Components")]
        [SerializeField] List<ImportAndExportBase> components = new List<ImportAndExportBase>();

        [Label("References")]
        [SerializeField] TranslationManager manager;
        [SerializeField] UndoManager undo;
        [SerializeField] ErrorWindow error;
        [SerializeField] NotificationManager notifications;

        [Label("Input")]
        public InputMapItemReference i_quickExport;

        public List<ImportAndExportBase> Components => components;

        public IImporter LastImporter { get; private set; } = null;
        public IExporter LastExporter { get; private set; } = null;

        private void Awake()
        {
            foreach (var item in components)
            {
                item.ImportAndExportManager = this;
                item.TranslationManager = manager;
                item.Undo = undo;
                item.Error = error;
                item.Notifications = notifications;
            }
        }

        private void Update()
        {
            if (i_quickExport.GetInputDown())
                QuickExport();
        }

        public void BeginImport(IImporter importer)
        {
            LastImporter = importer;
            importer.BeginImport();
        }

        public void BeginExport(IExporter exporter)
        {
            LastExporter = exporter;
            exporter.BeginExport();
        }

        public bool QuickExportAvaliable => 
            LastExporter != null;

        public void QuickExport()
        {
            if (!QuickExportAvaliable) return;

            try
            {
                LastExporter.Export();
            }
            catch (Exception e)
            {
                error.CreateExportExceptionPrompt(e);
            }
        }
    }
}