#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// FBXのAnimationClipの設定を自動化するスクリプト
/// </summary>
public class AnimationProcessor : AssetPostprocessor
{
    static void SetAnimationImporterSettings(ModelImporter importer)
    {
        //rigの設定
        importer.animationType = ModelImporterAnimationType.Human;
        importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
        importer.avatarSetup = ModelImporterAvatarSetup.CopyFromOther;
        var avatar = Resources.Load<GameObject>("Avatar/PlayerCharacter");
        Debug.Log(avatar);
        if (avatar != null)
        {
            importer.sourceAvatar = avatar.GetComponent<Animator>().avatar;
        }
        //animationClipの設定
        var clips = importer.clipAnimations;

        if (clips.Length == 0) clips = importer.defaultClipAnimations;

        foreach (var clip in clips)
        {
            
            clip.name = System.IO.Path.GetFileNameWithoutExtension(importer.assetPath);
            clip.keepOriginalOrientation = true;
            clip.keepOriginalPositionY = true;
            clip.keepOriginalPositionXZ = true;

            clip.lockRootRotation = true;
            clip.lockRootHeightY = true;
            clip.lockRootPositionXZ = false;
            
            
            clip.lockRootHeightY = true;
            clip.heightFromFeet = false;
            clip.keepOriginalPositionY = true;
            
            clip.heightOffset = 0f;
            clip.cycleOffset = 0f;
            clip.rotationOffset = 0f;
        }

        importer.clipAnimations = clips;
    }
    
    static void SetAnimationLoopSettings(ModelImporter importer)
    {
        //animationClipの設定
        var clips = importer.clipAnimations;

        if (clips.Length == 0) clips = importer.defaultClipAnimations;

        foreach (var clip in clips)
        {
            clip.loop = true;
        }
        importer.clipAnimations = clips;
    }

    void OnPreprocessAnimation()
    {
        // 自動的に実行
        //SetAnimationImporterSettings(assetImporter as ModelImporter);
    }

    [MenuItem("Assets/Set Animation Options", true)]
    static bool SetAnimationOptionsValidate()
    {
        return Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets).Length > 0;
    }

    [MenuItem("Assets/Set Animation Options")]
    static void SetAnimationOptions()
    {
        var filtered = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);
        foreach (var go in filtered)
        {
            var path = AssetDatabase.GetAssetPath(go);
            var importer = AssetImporter.GetAtPath(path);
            SetAnimationImporterSettings(importer as ModelImporter);
            AssetDatabase.ImportAsset(path);
        }

        Selection.activeObject = null;
    }
    
    [MenuItem("Assets/Set Loop", true)]
    static bool SetLoopValidate()
    {
        return Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets).Length > 0;
    }

    [MenuItem("Assets/Set Loop")]
    static void SetLoop()
    {
        var filtered = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);
        foreach (var go in filtered)
        {
            var path = AssetDatabase.GetAssetPath(go);
            var importer = AssetImporter.GetAtPath(path);
            SetAnimationLoopSettings(importer as ModelImporter);
            AssetDatabase.ImportAsset(path);
        }

        Selection.activeObject = null;
    }
}
#endif