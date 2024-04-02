﻿#if CVR_CCK_EXISTS
using System.Reflection;
using ABI.CCK.Components;
using UnityEngine;

namespace NAK.AASEmulator.Runtime.SubSystems
{
    public static class AdvancedTagging
    {
        #region Static Initialization

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            AASEmulatorCore.runtimeInitializedDelegate += runtime =>
            {
                if (AASEmulatorCore.Instance == null || !AASEmulatorCore.Instance.EmulateAdvancedTagging)
                    return;

                CVRAvatar avatar = runtime.m_avatar;
                if (avatar == null || !avatar.enableAdvancedTagging)
                {
                    SimpleLogger.LogError("Unable to run Advanced Tagging: CVRAvatar is missing or advanced tagging disabled");
                    return;
                }

                RunAdvancedTagging(avatar);
            };
        }
        
        #endregion Static Initialization

        #region Private Methods

        private static void RunAdvancedTagging(CVRAvatar avatar)
        {
            var advTaggingList = avatar != null ? avatar.advancedTaggingList : null;
            if (advTaggingList == null || advTaggingList.Count == 0)
            {
                SimpleLogger.LogWarning("Advanced Tagging List is empty or null");
                return;
            }
            
            SimpleLogger.Log($"Executing {advTaggingList.Count} Advanced Tagging entries for {avatar.name}");

            foreach (CVRAvatarAdvancedTaggingEntry advTaggingEntry in advTaggingList)
                ExecuteAdvancedTagging(advTaggingEntry);
        }

        private static void ExecuteAdvancedTagging(CVRAvatarAdvancedTaggingEntry advTaggingEntry)
        {
            if (advTaggingEntry == null)
            {
                SimpleLogger.LogError("Unable to execute Advanced Tagging: Entry is missing");
                return;
            }

            foreach (CVRAvatarAdvancedTaggingEntry.Tags tag in System.Enum.GetValues(typeof(CVRAvatarAdvancedTaggingEntry.Tags)))
            {
                if ((advTaggingEntry.tags & tag) == 0 || IsTagAllowed(tag)) 
                    continue;
                
                if (advTaggingEntry.gameObject != null) Object.DestroyImmediate(advTaggingEntry.gameObject);
                if (advTaggingEntry.fallbackGameObject != null) advTaggingEntry.fallbackGameObject.SetActive(true);
                break;
            }
        }
        
        private static bool IsTagAllowed(CVRAvatarAdvancedTaggingEntry.Tags tag)
        {
            AASEmulatorCore.AdvancedTags advTagging = AASEmulatorCore.Instance.advTagging;
            FieldInfo field = advTagging.GetType().GetField(tag.ToString());
            if (field != null && field.FieldType == typeof(bool)) 
                return (bool)field.GetValue(advTagging);
            
            return false;
        }
        
        #endregion Public Methods
    }
}
#endif
