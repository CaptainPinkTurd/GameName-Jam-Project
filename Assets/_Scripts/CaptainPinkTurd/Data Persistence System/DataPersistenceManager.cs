using System.Collections.Generic;
using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.DesignPattern.Singleton;
using CaptainPinkTurd.DataPersistence.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZLinq;

#if  UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace CaptainPinkTurd.DataPersistence
{
    public class DataPersistenceManager : Singleton<DataPersistenceManager>
    {
        [Header("File Storage Config")] 
        [SerializeField] private string fileName;
        [SerializeField] private bool useEncryption;

        [Header("Debug")] 
        [SerializeField] private bool initializeDataIfNull;
        
        private GameData gameData;
        private List<IDataPersistence> dataPersistenceObjects;
        private FileDataHandler dataHandler;
        
        private string selectedProfileId = "Default"; //for multiple save slots, haven't implemented yet
        
        public bool HasGameData => gameData != null; //should be used to disable continue or new game button on menu

        protected override void Awake()
        {
            base.Awake();
            
            dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        }
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private List<IDataPersistence> FindAllDataPersistenceObjects()
        {
            var dataPersistenceObjects =
                FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                    .AsValueEnumerable().OfType<IDataPersistence>().ToList();

            dataPersistenceObjects.AddRange(
                Resources.FindObjectsOfTypeAll<ScriptableObject>()
                    .AsValueEnumerable().OfType<IDataPersistence>().ToList());

            return dataPersistenceObjects;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            dataPersistenceObjects = FindAllDataPersistenceObjects();
            LoadGame();
        }
        public void NewGame()
        {
            gameData = new GameData();
        }
        public void LoadGame()
        {
            gameData = dataHandler.Load(selectedProfileId);
            
            if (gameData == null)
            {
                if (initializeDataIfNull)
                {
                    NewGame();
                }
                else
                {
                    Debug.Log("No data was found. A new game needs to be started before data can be loaded.");
                    return;
                }
            }

            foreach (var dataPersistenceObj in dataPersistenceObjects)
            {
                dataPersistenceObj.LoadData(gameData);
            }
        }
        public void SaveGame()
        {
            if (gameData == null)
            {
                Debug.LogWarning("No data was found. A new game needs to be started before data can be saved.");
                return;
            }
            
            foreach (var dataPersistenceObj in dataPersistenceObjects)
            {
                //Debug.Log("Save game data for " + dataPersistenceObj.Name);
                dataPersistenceObj.SaveData(gameData);
            }
            
            //save that data to a file using the data handler
            dataHandler.Save(gameData, selectedProfileId);
        }

        public void ChangeSelectedProfileId(string newProfileId)
        {
            selectedProfileId = newProfileId;
            
            //load the game, which will use that profile, updating our game data accordingly 
            LoadGame();
        }
        public Dictionary<string, GameData> GetAllProfilesGameData()
        {
            return dataHandler.LoadAllProfiles();
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }
        
#if UNITY_EDITOR
        [Button("Open Save File Folder Location")]
        public void OpenSaveFileFolderLocation()
        {
            // Opens the folder or highlights the specified item native to the OS
            EditorUtility.RevealInFinder(Path.Combine(Application.persistentDataPath)); 
        }
#endif
    }
}
