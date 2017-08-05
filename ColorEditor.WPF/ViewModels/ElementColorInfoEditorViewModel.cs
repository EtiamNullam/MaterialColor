using Prism.Mvvm;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Common;
using System.Collections;
using System.Collections.ObjectModel;

namespace ColorEditor.WPF.ViewModels
{
    // add seperate view for TypeColors?
    public class ElementColorInfoEditorViewModel : BindableBase
    {
        public ElementColorInfoEditorViewModel()
        {
            LoadObjectsCommand = new DelegateCommand(LoadTypeColors);
            LoadElementsCommand = new DelegateCommand(LoadElementColorInfos);

            SaveCommand = new DelegateCommand(Save);

            ExitCommand = new DelegateCommand(Exit);

            // load last dictionary used
        }

        public List<KeyValuePair<SimHashes, ElementColorInfo>> ElementColorInfos
        {
            get { return _elementColorInfos; }
            set { SetProperty(ref _elementColorInfos, value); }
        }

        private List<KeyValuePair<SimHashes, ElementColorInfo>> _elementColorInfos;

        public Dictionary<string, Color32> TypeColors
        {
            get { return _typeColors; }
            set { SetProperty(ref _typeColors, value); }
        }

        private Dictionary<string, Color32> _typeColors;

        public bool UnsavedChanges
        {
            get { return _unsavedChanges; }
            set { SetProperty(ref _unsavedChanges, value); }
        }

        private bool _unsavedChanges = false;

        public DelegateCommand LoadObjectsCommand { get; private set; }
        public DelegateCommand LoadElementsCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand RefreshInGameCommand { get; private set; }

        public DelegateCommand OpenFileCommand { get; private set; }
        public DelegateCommand SaveAsCommand { get; private set; }

        public DelegateCommand RestoreDefaultCommand { get; private set; }

        public DelegateCommand ExitCommand { get; private set; }

        public DelegateCommand UndoCommand { get; private set; }
        public DelegateCommand CutCommand { get; private set; }
        public DelegateCommand CopyCommand { get; private set; }
        public DelegateCommand PasteCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }

        public DelegateCommand AddCommand { get; private set; }
        public DelegateCommand RemoveCommand { get; private set; }

        public DelegateCommand ShowAboutCommand { get; private set; }

        private void LoadElementColorInfos()
        {
            // show dialog box if any unchanged saves would be lost
            ElementColorInfos = new Common.Json.ElementColorInfosManager().LoadElementColorInfos().ToList();

        }

        private void LoadTypeColors()
        {
            // show dialog box if any unchanged saves would be lost
        }

        private void Save()
        {
            var dictionary = ElementColorInfos.ToDictionary(pair => pair.Key, pair => pair.Value);

            try
            {
                new Common.Json.ElementColorInfosManager().SaveElementsColorInfo(dictionary);
            }
            catch (Exception e)
            {
                return;
            }

            UnsavedChanges = false;
        }

        private void Exit()
        {
            // show dialog box if any unchanged saves would be lost
            App.Current.Shutdown();
        }
    }
}
