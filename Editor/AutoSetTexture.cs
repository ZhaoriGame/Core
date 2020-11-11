// ========================================================
// Autor：Zhaori 
// CreateTime：2019/10/24 15:01:20 
// Des：导入自动设置图片格式
// ========================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Object = UnityEngine.Object;


public class AutoSetTexture : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        //自动设置类型;
        TextureImporter textureImporter = (TextureImporter) assetImporter;
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.filterMode = FilterMode.Point;
        //压缩设置为不压缩
        textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
    }
}

public class AutoSetTextureMenu
{
    [MenuItem("Assets/MenuClick/SetSpriteConvert")]
    static void TestMenu()
    {
        object[] selection = (object[]) Selection.objects;
        //判断是否有对象被选中
        if (selection.Length == 0)
            return;

        string objPath = AssetDatabase.GetAssetPath((Object) selection[0]);

        string pathRoot = Application.dataPath;
        pathRoot = pathRoot.Substring(0, pathRoot.LastIndexOf("/"));
        objPath = pathRoot + "/" + objPath;

        DirectoryInfo directoryInfo = new DirectoryInfo(objPath);
        FileInfo[] fileInfos1 = directoryInfo.GetFiles("*.png", SearchOption.AllDirectories);
        FileInfo[] fileInfos2 = directoryInfo.GetFiles("*.jpg", SearchOption.AllDirectories);

        Set(fileInfos1);
        Set(fileInfos2);


        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/MenuClick/SetPivotToBottom")]
    static void SetPivotToBottom()
    {
        object[] selection = (object[]) Selection.objects;
        //判断是否有对象被选中
        if (selection.Length == 0)
            return;
        foreach (var current in selection)
        {
            string objPath = AssetDatabase.GetAssetPath((Object) current);

            TextureImporter textureImporter = AssetImporter.GetAtPath(objPath) as TextureImporter;

            TextureImporterSettings textureImporterSettings = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(textureImporterSettings);

            textureImporterSettings.textureType = TextureImporterType.Sprite;
            textureImporterSettings.filterMode = FilterMode.Point;
            textureImporterSettings.spritePixelsPerUnit = 32;
            textureImporterSettings.spriteAlignment = (int) SpriteAlignment.BottomCenter;
            textureImporterSettings.spritePivot = new Vector2(0.5f, 0);
            
            
            List<SpriteMetaData> spriteMetas = new List<SpriteMetaData>();
            

            for (int i = 0; i < textureImporter.spritesheet.Length; i++)
            {
                SpriteMetaData spriteMetaData = new SpriteMetaData();
                spriteMetaData.rect = textureImporter.spritesheet[i].rect;
                spriteMetaData.pivot = new Vector2(0.5f, 0);
                spriteMetaData.alignment = (int) SpriteAlignment.BottomCenter;
                spriteMetaData.name = textureImporter.spritesheet[i].name;
                spriteMetas.Add(spriteMetaData);

            }
            
            textureImporter.spritesheet = spriteMetas.ToArray();
            
            for (int i = 0; i < textureImporter.spritesheet.Length; i++)
            {
                Debug.LogError(textureImporter.spritesheet[i].pivot);
            }
            
            textureImporter.SetTextureSettings(textureImporterSettings);
            AssetDatabase.ImportAsset(objPath, ImportAssetOptions.ForceUncompressedImport);
            
            //textureImporter.SaveAndReimport();
            
            for (int i = 0; i < textureImporter.spritesheet.Length; i++)
            {

                Debug.LogError(textureImporter.spritesheet[i].pivot);
            }
            
           
            
           
        }
    }

    [MenuItem("Assets/MenuClick/设置Mulitple图片")]
    static void TestMenu1()
    {
        object[] selection = (object[]) Selection.objects;
        //判断是否有对象被选中
        if (selection.Length == 0)
            return;

        string objPath = AssetDatabase.GetAssetPath((Object) selection[0]);

        string pathRoot = Application.dataPath;
        pathRoot = pathRoot.Substring(0, pathRoot.LastIndexOf("/"));
        objPath = pathRoot + "/" + objPath;

        DirectoryInfo directoryInfo = new DirectoryInfo(objPath);
        FileInfo[] fileInfos1 = directoryInfo.GetFiles("*.png", SearchOption.AllDirectories);
        FileInfo[] fileInfos2 = directoryInfo.GetFiles("*.jpg", SearchOption.AllDirectories);

        Set(fileInfos1, 1);
        Set(fileInfos2, 1);


        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/MenuClick/设置Sigleton图片")]
    static void TestMenu2()
    {
        object[] selection = (object[]) Selection.objects;
        //判断是否有对象被选中
        if (selection.Length == 0)
            return;

        string objPath = AssetDatabase.GetAssetPath((Object) selection[0]);

        string pathRoot = Application.dataPath;
        pathRoot = pathRoot.Substring(0, pathRoot.LastIndexOf("/"));
        objPath = pathRoot + "/" + objPath;

        DirectoryInfo directoryInfo = new DirectoryInfo(objPath);
        FileInfo[] fileInfos1 = directoryInfo.GetFiles("*.png", SearchOption.AllDirectories);
        FileInfo[] fileInfos2 = directoryInfo.GetFiles("*.jpg", SearchOption.AllDirectories);

        Set(fileInfos1, 2);
        Set(fileInfos2, 2);

        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/MenuClick/DeletMeta")]
    static void DeleMeta()
    {
        object[] selection = (object[]) Selection.objects;
        //判断是否有对象被选中
        if (selection.Length == 0)
            return;

        string objPath = AssetDatabase.GetAssetPath((Object) selection[0]);

        string pathRoot = Application.dataPath;
        pathRoot = pathRoot.Substring(0, pathRoot.LastIndexOf("/"));
        objPath = pathRoot + "/" + objPath;

        DirectoryInfo directoryInfo = new DirectoryInfo(objPath);
        FileInfo[] fileInfos1 = directoryInfo.GetFiles("*.meta", SearchOption.AllDirectories);


        foreach (var file in fileInfos1)
        {
            File.Delete(file.FullName);
        }

        AssetDatabase.Refresh();
    }

    static void Set(FileInfo[] fileInfos, int num = 0)
    {
        foreach (var file in fileInfos)
        {
            string path = file.FullName;
            path = path.Substring(path.IndexOf("Assets\\", StringComparison.Ordinal));
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter.spritePixelsPerUnit == 32)
            {
                continue;
            }

            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.filterMode = FilterMode.Point;
            textureImporter.spritePixelsPerUnit = 32;
            if (num == 1)
            {
                textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            }

            if (num == 2)
            {
                textureImporter.spriteImportMode = SpriteImportMode.Single;
            }

            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            textureImporter.SaveAndReimport();
        }
    }

    public static void SetSpritePivot(FileInfo[] fileInfos)
    {
        foreach (var file in fileInfos)
        {
            string path = file.FullName;
            path = path.Substring(path.IndexOf("Assets\\"));

            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.filterMode = FilterMode.Point;
            textureImporter.spritePixelsPerUnit = 32;
            textureImporter.spritePivot = new Vector2(0f, 0);
            textureImporter.SaveAndReimport();
        }
    }
}