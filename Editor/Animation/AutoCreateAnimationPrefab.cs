// ========================================================
// Autor：SYSTEM 
// CreateTime：2020/09/22 14:38:26 
// Des：
// ========================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;


public class AutoCreateAnimationPrefab : MonoBehaviour
{
    /// <summary>
    /// 生成出的Animation的路径
    /// </summary>
    private static string AnimationPath = "Assets/Game/AnimationController/AutoGenerateAnimations";
    
    private static string AnimationControllerPath = "Assets/Game/AnimationController/AutoGenerateAnimations";



    [MenuItem("Assets/MenuClick/AnimationPrefab/Image")]
    static void CreatePrefabImage()
    {
        Object[] selection = Selection.objects;


        foreach (var current in selection)
        {
            string objPath = AssetDatabase.GetAssetPath(current);
            
            Create(objPath);
        }

        

       
    }


//    [MenuItem("Assets/MenuClick/AnimationPrefab/FolderImage")]
//    static void CreatePrefab()
//    {
//        Object[] selection = Selection.objects;
//
//        if (selection.Length == 0)
//        {
//            return;
//        }
//
//        string objPath = AssetDatabase.GetAssetPath(selection[0]);
//
//        //拿到文件夹下的所有类型
//        DirectoryInfo raw = new DirectoryInfo(objPath);
//
//        foreach (DirectoryInfo directory in raw.GetDirectories())
//        {
//            Debug.LogError(directory.FullName);
//        }
//
//        for (int i = 1; i < raw.GetDirectories().Length; i++)
//        {
//        }
//    }


    static void Create(string Path)
    {
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(Path);

        string name = assets[0].name;
        
        GameObject InsGo = new GameObject(name);

        Sprite[] sprites  = new Sprite[assets.Length-1];
        
        for (int i = 1; i < assets.Length; i++)
        {
            sprites[i-1] = assets[i] as Sprite;
        }
        
        Animator animator = InsGo.AddComponent<Animator>();
        
        SpriteRenderer spriteRender = InsGo.AddComponent<SpriteRenderer>();
        
        spriteRender.sprite = sprites[0];
        
        List<AnimationClip> clips = new List<AnimationClip>();
        
        //clips.Add(BuildAnimationClip(new Sprite[]{sprites[0]},"Idle"));
        clips.Add(BuildAnimationClip(sprites,name));

        animator.runtimeAnimatorController = BuildAnimationController(clips,name,name);

      

      
        
      


        DirectoryInfo raw = new DirectoryInfo(Path);

        if (raw.Parent != null)
        {
            string folderPath =
                raw.Parent.FullName.Substring(raw.Parent.FullName.IndexOf("Assets\\", StringComparison.Ordinal));
            folderPath += "/Prefab";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            
           
            folderPath += "/" + name + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(InsGo, folderPath);
        }


        DestroyImmediate(InsGo);
    }
    

    static AnimatorController BuildAnimationController(List<AnimationClip> clips, string name, string fileName)
    {
        AnimatorController animatorController =
            UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(
                AnimationControllerPath + "/" + name + "/" + name + ".controller");

        AnimatorControllerLayer layer = animatorController.layers[0];
        AnimatorStateMachine sm = layer.stateMachine;
        animatorController.AddParameter("Attack", AnimatorControllerParameterType.Trigger);
        animatorController.AddParameter("Hurt", AnimatorControllerParameterType.Trigger);

        Dictionary<string, AnimatorState> animatorStates = new Dictionary<string, AnimatorState>();
        foreach (AnimationClip newClip in clips)
        {
            //AnimatorStateMachine machine = sm.AddStateMachine(newClip.name);
            AnimatorState state = sm.AddState(newClip.name);
            animatorStates.Add(newClip.name, state);
            state.motion = newClip;
            //AnimatorStateTransition trans = sm.AddAnyStateTransition(state);
            if (newClip.name == "Idle")
            {
                sm.defaultState = state;
            }
        }

        AssetDatabase.SaveAssets();
        return animatorController;
    }

    static AnimationClip BuildAnimationClip(Sprite[] images ,string name)
    {
        //获取的是 单个动作的名称
        string animationName = name;
        //查找所有图片

        AnimationClip clip = new AnimationClip();
        AnimationUtility.SetAnimationType(clip, ModelImporterAnimationType.Generic);
        EditorCurveBinding curveBinding = new EditorCurveBinding();
        curveBinding.type = typeof(SpriteRenderer);
        curveBinding.path = "";
        curveBinding.propertyName = "m_Sprite";

        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[images.Length];

        //动画长度是按秒为单位，1/10就表示1秒切10张图片
        float frameTime = 1 / 10f;

        for (int i = 0; i < images.Length; i++)
        {
            //根据名字 读取到sprite
            Sprite sprite = images[i];
            // sprite.pixelsPerUnit 
            keyFrames[i] = new ObjectReferenceKeyframe();
            keyFrames[i].time = frameTime * i;
            keyFrames[i].value = sprite;
        }

        if (keyFrames.Length == 1)
        {
            keyFrames = new ObjectReferenceKeyframe[5];
            Sprite sprite = images[0];
            for (int i = 0; i < 5; i++)
            {
                keyFrames[i] = new ObjectReferenceKeyframe();
                keyFrames[i].time = frameTime * i;
                keyFrames[i].value = sprite;
            }
        }

        //动画帧率，30比较合适
        clip.frameRate = 30;


        //动画名字里包含Idle的一般设置为循环
        if (animationName.ToLower().IndexOf("idle", StringComparison.Ordinal) >= 0)
        {
            //设置idle文件为循环动画
            SerializedObject serializedClip = new SerializedObject(clip);
            AnimationClipSettings clipSettings =
                new AnimationClipSettings(serializedClip.FindProperty("m_AnimationClipSettings")) {loopTime = true};
            serializedClip.ApplyModifiedProperties();
        }

        string parentName = name;
        Directory.CreateDirectory(AnimationPath + "/" + parentName);
        AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);
        AssetDatabase.CreateAsset(clip, AnimationPath + "/" + parentName + "/" + animationName + ".anim");
        AssetDatabase.SaveAssets();
        return clip;
    }

    public static string DataPathToAssetPath(string path)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            return path.Substring(path.IndexOf("Assets\\"));
        else
            return path.Substring(path.IndexOf("Assets/"));
    }

    class AnimationClipSettings
    {
        private readonly SerializedProperty m_Property;

        private SerializedProperty Get(string property)
        {
            return m_Property.FindPropertyRelative(property);
        }

        public AnimationClipSettings(SerializedProperty prop)
        {
            m_Property = prop;
        }

        public float startTime
        {
            get => Get("m_StartTime").floatValue;
            set => Get("m_StartTime").floatValue = value;
        }

        public float stopTime
        {
            get => Get("m_StopTime").floatValue;
            set => Get("m_StopTime").floatValue = value;
        }

        public float orientationOffsetY
        {
            get => Get("m_OrientationOffsetY").floatValue;
            set => Get("m_OrientationOffsetY").floatValue = value;
        }

        public float level
        {
            get => Get("m_Level").floatValue;
            set => Get("m_Level").floatValue = value;
        }

        public float cycleOffset
        {
            get => Get("m_CycleOffset").floatValue;
            set => Get("m_CycleOffset").floatValue = value;
        }

        public bool loopTime
        {
            get => Get("m_LoopTime").boolValue;
            set => Get("m_LoopTime").boolValue = value;
        }

        public bool loopBlend
        {
            get => Get("m_LoopBlend").boolValue;
            set => Get("m_LoopBlend").boolValue = value;
        }

        public bool loopBlendOrientation
        {
            get => Get("m_LoopBlendOrientation").boolValue;
            set => Get("m_LoopBlendOrientation").boolValue = value;
        }

        public bool loopBlendPositionY
        {
            get => Get("m_LoopBlendPositionY").boolValue;
            set => Get("m_LoopBlendPositionY").boolValue = value;
        }

        public bool loopBlendPositionXZ
        {
            get => Get("m_LoopBlendPositionXZ").boolValue;
            set => Get("m_LoopBlendPositionXZ").boolValue = value;
        }

        public bool keepOriginalOrientation
        {
            get => Get("m_KeepOriginalOrientation").boolValue;
            set => Get("m_KeepOriginalOrientation").boolValue = value;
        }

        public bool keepOriginalPositionY
        {
            get => Get("m_KeepOriginalPositionY").boolValue;
            set => Get("m_KeepOriginalPositionY").boolValue = value;
        }

        public bool keepOriginalPositionXZ
        {
            get => Get("m_KeepOriginalPositionXZ").boolValue;
            set => Get("m_KeepOriginalPositionXZ").boolValue = value;
        }

        public bool heightFromFeet
        {
            get => Get("m_HeightFromFeet").boolValue;
            set => Get("m_HeightFromFeet").boolValue = value;
        }

        public bool mirror
        {
            get => Get("m_Mirror").boolValue;
            set => Get("m_Mirror").boolValue = value;
        }
    }
}