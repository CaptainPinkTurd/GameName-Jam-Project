#if UNITY_EDITOR
using CaptainPinkTurd.Core.SO;
using CaptainPinkTurd.Core.Utils;
using UnityEditor;

namespace CaptainPinkTurd.EffectSystem.AdaptiveFootstepSFX
{
    [CustomEditor(typeof(FootstepAudio))]
    public class FootstepAudioEditor : Editor
    {
        private FootstepAudio footstepAudio;
        
        private void OnEnable()
        {
            footstepAudio = (FootstepAudio) target;
            UpdateDatabase();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        private void UpdateDatabase()
        {
            var terrainTypes = ScriptableObjectUtils.GetAllScriptableObjects<TerrainType>();
            var tmpTDB = footstepAudio.terrainDatabase;

            for (int i = 0; i < terrainTypes.Length; i++)
            {
                bool found = false;
                foreach (var sound in tmpTDB)
                {
                    if(sound.terrainType == terrainTypes[i].name)
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    tmpTDB.Insert(i, new FootstepAudio.Sounds(terrainTypes[i].name));
                }
            }
            
            int deleteIndex = 0;
            while (deleteIndex < tmpTDB.Count)
            {
                bool found = false;
                
                //loop through all available terrain types and check if it exists in the current database or not, if an element in the database doesn't 
                //contain the terrain type, remove it
                foreach (var terrainType in terrainTypes) 
                {
                    if (terrainType.name != tmpTDB[deleteIndex].terrainType) continue;
                    
                    deleteIndex++;
                    found = true;
                    break;
                }
                if (!found)
                {
                    //don't need to increase deleteIndex cause once database has removed an element the list will have shrunk to match the current index already
                    tmpTDB.RemoveAt(deleteIndex);
                }
            }
            footstepAudio.terrainDatabase = tmpTDB;
        }
    }
}
#endif