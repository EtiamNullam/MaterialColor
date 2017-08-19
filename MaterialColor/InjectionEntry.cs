﻿using MaterialColor.Common.Json;
using MaterialColor.Core.Extensions;
using MaterialColor.Core.Helpers;
using MaterialColor.Core.IO;
using System;
using System.IO;

namespace MaterialColor.Core
{
    public static class InjectionEntry
    {
        private static bool Initialized = false;

        private static bool ElementColorInfosChanged = false;
        private static bool TypeColorOffsetsChanged = false;

        private static JsonManager _jsonManager = new JsonManager();
        private static ConfiguratorStateManager _configuratorStateManager = new ConfiguratorStateManager(_jsonManager);
        private static ElementColorInfosManager _elementColorInfosManager = new ElementColorInfosManager(_jsonManager);
        private static TypeColorOffsetsManager _typeColorOffsetsManager = new TypeColorOffsetsManager(_jsonManager);

        public static void EnterOnce()
        {
            //if (State.ConfiguratorState.ShowDetailedErrorInfo)
            //{
            //    Debug.LogError("Enter Once");
            //}

            try
            {
                Components.BuildingCompletes.OnAdd += OnBuildingsCompletesAdd;

                ReloadAll();

                if (!Initialized) Initialize();
            }
            catch (Exception e)
            {
                var message = "Injection failed\n" + e.Message + '\n';

                if (State.ConfiguratorState.ShowDetailedErrorInfo)
                {
                    message += '\n' + e.StackTrace;
                }

                Debug.LogError(message);
            }
        }

        private static void ReloadAll()
        {
            TryLoadElementColorInfos();
            TryLoadTypeColorOffsets();
            TryLoadConfiguratorState();
        }

        private static void Initialize()
        {
            // unsubscribe?
            OverlayScreen.OnOverlayChanged += OnOverlayChanged;

            StartFileChangeNotifier();
            Initialized = true;
        }

        public static void EnterEveryUpdate()
        {
            var changed = false;

            if (ElementColorInfosChanged)
            {
                TryLoadElementColorInfos();
                changed = true;
            }

            if (TypeColorOffsetsChanged)
            {
                TryLoadTypeColorOffsets();
                changed = true;
            }

            if (changed)
            {
                UpdateBuildingsColors();
                ElementColorInfosChanged = TypeColorOffsetsChanged = false;
            }
        }

        public static UnityEngine.Color EnterCell(Rendering.BlockTileRenderer blockRenderer, int cellIndex)
        {
            var material = GetMaterialFromCell(cellIndex);
            var materialColor = material.ToCellMaterialColor();

            if (blockRenderer.highlightCell == cellIndex)
            {
                return materialColor * 1.25f;
            }
            else if (blockRenderer.selectedCell == cellIndex)
            {
                return materialColor * 1.5f;
            }
            return materialColor;
        }

        private static SimHashes GetMaterialFromCell(int cellIndex)
        {
            if (!Grid.IsValidCell(cellIndex))
            {
                return SimHashes.Vacuum;
            }

            var cellElementIndex = Grid.Cell[cellIndex].elementIdx;
            var element = ElementLoader.elements[cellElementIndex];

            return element.id;
        }

        private static void OnOverlayChanged(SimViewMode obj)
        {
            if (obj == SimViewMode.None)
            {
                UpdateBuildingsColors();
            }
        }

        private static void StartFileChangeNotifier()
        {
            if (_fileChangeNotifier == null)
            {
                _fileChangeNotifier = _fileChangeNotifier ?? new FileChangeNotifier();
                _fileChangeNotifier.ElementColorInfosChanged += OnElementColorsInfosChanged;
                _fileChangeNotifier.TypeColorOffsetsChanged += OnTypeColorOffsetsChanged;
                _fileChangeNotifier.ConfiguratorStateChanged += OnConfiguratorStateChanged;
            }
            // convert to instance class and dispose properly of events and FileChangeNotifier
        }

        private static void OnConfiguratorStateChanged(object sender, FileSystemEventArgs e)
        {
            TryLoadConfiguratorState();
        }

        private static void TryLoadConfiguratorState()
        {
            try
            {
                State.ConfiguratorState = _configuratorStateManager.LoadState();
            }
            catch (Exception ex)
            {
                var message = "Can't load configurator state.\n" + ex.Message + '\n';

                if (State.ConfiguratorState.ShowDetailedErrorInfo)
                {
                    message += '\n' + ex.StackTrace;
                }
                Debug.LogError(message);

                return;
            }
        }

        private static void TryLoadElementColorInfos()
        {
            try
            {
                State.ElementColorInfos = _elementColorInfosManager.LoadElementColorInfos();
            }
            catch (Exception e)
            {
                var message = "Can't load ElementColorInfos\n" + e.Message + '\n';

                if (State.ConfiguratorState.ShowDetailedErrorInfo)
                {
                    message += '\n' + e.StackTrace;
                }
                Debug.LogError(message);
            }
        }

        private static void TryLoadTypeColorOffsets()
        {
            try
            {
                State.TypeColorOffsets = _typeColorOffsetsManager.LoadTypeColorOffsets();
            }
            catch (Exception e)
            {
                var message = "Can't load TypeColorOffsets\n" + e.Message + '\n';

                if (State.ConfiguratorState.ShowDetailedErrorInfo)
                {
                    message += '\n' + e.StackTrace;
                }

                Debug.LogError(message);
            }
        }

        private static void UpdateBuildingsColors()
        {
            foreach (var building in Components.BuildingCompletes)
            {
                OnBuildingsCompletesAdd(building);
            }
        }

        private static void OnElementColorsInfosChanged(object sender, FileSystemEventArgs e)
        {
            Debug.LogError("Element color infos changed.");

            ElementColorInfosChanged = true;
        }

        private static void OnTypeColorOffsetsChanged(object sender, FileSystemEventArgs e)
        {
            Debug.LogError("Type colors changed.");

            TypeColorOffsetsChanged = true;
        }

        private static FileChangeNotifier _fileChangeNotifier;

        private static void OnBuildingsCompletesAdd(BuildingComplete building)
            => ColorHelper.UpdateBuildingColor(building);

    }
}
