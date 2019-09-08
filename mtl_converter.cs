#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEditor;
using UnityEngine;

public class MTLConverterScriptableWizard : UnityEditor.ScriptableWizard
{
    public string directoryPath = "Assets";
    public Shader targetShader = null;
    Dictionary<string, List<string>> GetMTLDictionary(string str)
    {
        Dictionary<string, List<string>> MTLDictionary = new Dictionary<string, List<string>>();
        string[] MTLLines = new string[0];

        string[] delimiters = {"\r\n","\n" };
        MTLLines = str.Split(delimiters, System.StringSplitOptions.None);
        for(int i = 0; i < MTLLines.Length; i++)
        {
            string MTLLine = MTLLines[i];
            string[] MTLKeyValPair = MTLLine.Split(' ');

            if(MTLKeyValPair.Length > 2)
            {
                return new Dictionary<string, List<string> >(); // ???
            }

            if (MTLKeyValPair.Length == 2)
            {
                if (!MTLDictionary.ContainsKey(MTLKeyValPair[0]))
                {
                    MTLDictionary.Add(MTLKeyValPair[0], new List<string>());
                }
                string foo = MTLKeyValPair[1];
                MTLDictionary[MTLKeyValPair[0]].Add(MTLKeyValPair[1]);
            }
        }

        return MTLDictionary;
    }

    private string GetAsPathToPNG(string path) {
        string strippedPath = Path.GetFileNameWithoutExtension(path);
        return strippedPath + ".png";
    }

    private void ConvertMTLs()
    {
        string[] mtlFiles = Directory.GetFiles(directoryPath);
        for (int i = 0; i < mtlFiles.Length; i++)
        {
            if(mtlFiles[i].EndsWith(".mtl"))
            {
                StreamReader fs = new StreamReader(mtlFiles[i]);
                Dictionary<string, List<string> > MTLDictionary = GetMTLDictionary(fs.ReadToEnd());
                
                if (MTLDictionary.Keys.Count > 1)
                {
                    Shader shader = targetShader;
                    if (shader == null)
                        return;

                    Material material = new Material(shader);
                    AssetDatabase.CreateAsset(material, directoryPath + "/" + Path.GetFileNameWithoutExtension(mtlFiles[i]) + ".mat");

                    if (MTLDictionary.ContainsKey("map_Kd")) {
                        List<string> diffuseTextures = MTLDictionary["map_Kd"];

                        Texture2D t1 = null;
                        Texture2D t2 = null;

                        if (diffuseTextures.Count == 1)
                        {
                            string tPath = GetAsPathToPNG(directoryPath + "/" + diffuseTextures[0]);
                            Texture2D t = (Texture2D)AssetDatabase.LoadAssetAtPath(tPath, typeof(Texture2D));
                            t1 = t;
                            t2 = t;
                        }
                        else if (diffuseTextures.Count == 2)
                        {
                            string tPath1 = GetAsPathToPNG(directoryPath + "/" + diffuseTextures[0]);
                            string tPath2 = GetAsPathToPNG(directoryPath + "/" + diffuseTextures[1]);

                            t1 = (Texture2D)AssetDatabase.LoadAssetAtPath(tPath1, typeof(Texture2D));
                            t2 = (Texture2D)AssetDatabase.LoadAssetAtPath(tPath2, typeof(Texture2D));
                        }

                        material.SetTexture("_Albedo0", t1);
                        material.SetTexture("_Albedo1", t2);
                    }
                    if (MTLDictionary.ContainsKey("bump")) {
                        List<string> bumpTextures = MTLDictionary["bump"];

                        Texture2D t1 = null;
                        Texture2D t2 = null;

                        if (bumpTextures.Count == 1)
                        {
                            string tPath = GetAsPathToPNG(directoryPath + "/" + bumpTextures[0]);
                            Texture2D t = (Texture2D)AssetDatabase.LoadAssetAtPath(tPath, typeof(Texture2D));
                            if (t)
                            {
                                TextureImporter tImporter = (TextureImporter)TextureImporter.GetAtPath(tPath);
                                if (tImporter)
                                {
                                    tImporter.textureType = TextureImporterType.NormalMap;
                                    AssetDatabase.ImportAsset(tPath, ImportAssetOptions.ForceUpdate);
                                }
                            }
                            t1 = t;
                            t2 = t;
                        }
                        else if (bumpTextures.Count == 2)
                        {
                            string tPath1 = GetAsPathToPNG(directoryPath + "/" + bumpTextures[0]);
                            string tPath2 = GetAsPathToPNG(directoryPath + "/" + bumpTextures[1]);

                            t1 = (Texture2D)AssetDatabase.LoadAssetAtPath(tPath1, typeof(Texture2D));
                            t2 = (Texture2D)AssetDatabase.LoadAssetAtPath(tPath2, typeof(Texture2D));

                            if (t1)
                            {
                                TextureImporter tImporter = (TextureImporter)TextureImporter.GetAtPath(tPath1);
                                if (tImporter)
                                {
                                    tImporter.textureType = TextureImporterType.NormalMap;
                                    AssetDatabase.ImportAsset(tPath1, ImportAssetOptions.ForceUpdate);
                                }
                            }

                            if (t2)
                            {
                                TextureImporter tImporter = (TextureImporter)TextureImporter.GetAtPath(tPath2);
                                if (tImporter)
                                {
                                    tImporter.textureType = TextureImporterType.NormalMap;
                                    AssetDatabase.ImportAsset(tPath2, ImportAssetOptions.ForceUpdate);
                                }
                            }
                        }

                        material.SetTexture("_NormalMap0", t1);
                        material.SetTexture("_NormalMap1", t2);
                    }
                    if (MTLDictionary.ContainsKey("map_Ks")) {
                        List<string> specularTextures = MTLDictionary["map_Ks"];

                        Texture2D t1 = null;
                        Texture2D t2 = null;

                        if (specularTextures.Count == 1)
                        {
                            string tPath = GetAsPathToPNG(directoryPath + "/" + specularTextures[0]);
                            Texture2D t = (Texture2D)AssetDatabase.LoadAssetAtPath(tPath, typeof(Texture2D));
                            t1 = t;
                            t2 = t;
                        }
                        else if (specularTextures.Count == 2)
                        {
                            string tPath1 = GetAsPathToPNG(directoryPath + "/" + specularTextures[0]);
                            string tPath2 = GetAsPathToPNG(directoryPath + "/" + specularTextures[1]);

                            t1 = (Texture2D)AssetDatabase.LoadAssetAtPath(tPath1, typeof(Texture2D));
                            t2 = (Texture2D)AssetDatabase.LoadAssetAtPath(tPath2, typeof(Texture2D));
                        }

                        material.SetTexture("_Metallic0", t1);
                        material.SetTexture("_Metallic1", t2);
                    }
                }
            }
        }
    }

    [UnityEditor.MenuItem("Saracen/Tools/Convert MTL files")]
    static void CreateWizard()
    {
        UnityEditor.ScriptableWizard.DisplayWizard<MTLConverterScriptableWizard>("Convert MTL files", "Apply");
    }

    void OnWizardCreate()
    {
        ConvertMTLs();
    }

    void OnWizardUpdate()
    {

    }
};

#endif