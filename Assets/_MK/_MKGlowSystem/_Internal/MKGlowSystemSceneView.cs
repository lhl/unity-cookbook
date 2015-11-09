///////////////////////////////////////////////
// MKGlowSystem	Editor						 //
//											 //
// Created by Michael Kremmel on 23.12.2014  //
// Copyright © 2015 All rights reserved.     //
///////////////////////////////////////////////

#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MKGlowSystem;

namespace MKGlowSystemSV
{
    public class MKGlowSystemSceneView : EditorWindow
    {
        private const string m_Style = "box";
        //private static Texture2D m_SceneViewLabel;

        private const string FILENAME = "/_MK/_MKGlowSystem/_Internal/Settings.MKGlow";
        private static SceneView m_SceneView;
        private static Camera m_SceneViewCamera;
        private static MKSceneGlow m_MKGlowComponent;

        private static GameObject m_BufferObject;
        private static MKGlowSystemSceneView m_Window;

        private static SerializedProperty m_GlowLayerProperty;
        private static MKGlowSystemSceneView SceneViewWindow
        {
            get
            {
                if (m_Window == null)
                    m_Window = (MKGlowSystemSceneView)EditorWindow.GetWindow<MKGlowSystemSceneView>("MKGlow Scene View");

                return m_Window;
            }
        }

        [System.Serializable]
        public struct SaveSceneViewMKGlowBlock
        {
            public bool sve;
            public int m_GlowLayer;
            public int m_GlowMode;
            public int m_GlowType;
            public int m_GlowQuality;
            public int m_BlurIterations;
            public int m_Samples;
            public float m_FullScreenGlowTintX;
            public float m_FullScreenGlowTintY;
            public float m_FullScreenGlowTintZ;
            public float m_FullScreenGlowTintW;

            public float m_BlurSpread;
            public float m_GlowIntensity;
            public float m_BlurOffset;

            public bool m_ShowCutoutGlow;
            public bool m_ShowTransparentGlow;

            public int glowCurve;
            public SaveSceneViewMKGlowBlock(bool sve, int m_GlowLayer, int m_GlowMode, int m_GlowType, int m_GlowQuality, int m_BlurIterations, int m_Samples,
                                            float m_FullScreenGlowTintX, float m_FullScreenGlowTintY, float m_FullScreenGlowTintZ, float m_FullScreenGlowTintW,
                                            float m_BlurSpread, float m_GlowIntensity, float m_BlurOffset, bool m_ShowCutoutGlow, bool m_ShowTransparentGlow, int glowCurve)
            {
                this.sve = sve;
                this.m_GlowLayer = m_GlowLayer;
                this.m_GlowMode = m_GlowMode;
                this.m_GlowType = m_GlowType;
                this.m_GlowQuality = m_GlowQuality;
                this.m_BlurIterations = m_BlurIterations;
                this.m_Samples = m_Samples;
                this.m_FullScreenGlowTintX = m_FullScreenGlowTintX;
                this.m_FullScreenGlowTintY = m_FullScreenGlowTintY;
                this.m_FullScreenGlowTintZ = m_FullScreenGlowTintZ;
                this.m_FullScreenGlowTintW = m_FullScreenGlowTintW;
                this.m_BlurSpread = m_BlurSpread;
                this.m_GlowIntensity = m_GlowIntensity;
                this.m_BlurOffset = m_BlurOffset;
                this.m_ShowCutoutGlow = m_ShowCutoutGlow;
                this.m_ShowTransparentGlow = m_ShowTransparentGlow;
                this.glowCurve = glowCurve;
            }
        }
        public static void Load()
        {
            LoadSceneViewMKGlowSettings();
        }
        public static void Save()
        {
            SaveSceneViewMKGlowSettings();
        }
        public static void Save(ref SaveSceneViewMKGlowBlock save)
        {
            SaveSceneViewMKGlowSettings(ref save);
        }
        private static void SaveSceneViewMKGlowSettings()
        {
            GetGlowLayerProperty();
            SaveSceneViewMKGlowBlock save = new SaveSceneViewMKGlowBlock(MKGlowComponent.enabled, (int)MKGlowComponent.GlowRenderLayer, (int)MKGlowComponent.GlowResolution, (int)MKGlowComponent.GlowType,
                                                                         (int)MKGlowComponent.GlowQuality, (int)MKGlowComponent.BlurIterations, (int)MKGlowComponent.Samples,
                                                                         MKGlowComponent.FullScreenGlowTint.r, MKGlowComponent.FullScreenGlowTint.g, MKGlowComponent.FullScreenGlowTint.b, MKGlowComponent.FullScreenGlowTint.a,
                                                                         MKGlowComponent.BlurSpread, MKGlowComponent.GlowIntensity, MKGlowComponent.BlurOffset, MKGlowComponent.ShowCutoutGlow, MKGlowComponent.ShowTransparentGlow, (int)MKGlowComponent.GlowCurve);
            FileStream stream;
            stream = new FileStream(Application.dataPath + FILENAME, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, save);
            stream.Close();
        }
        private static void SaveSceneViewMKGlowSettings(ref SaveSceneViewMKGlowBlock save)
        {
            GetGlowLayerProperty();
            FileStream stream;
            stream = new FileStream(Application.dataPath + FILENAME, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, save);
            stream.Close();
        }

        private static void LoadSceneViewMKGlowSettings()
        {
            if (File.Exists(Application.dataPath + FILENAME))
            {
                SaveSceneViewMKGlowBlock load;
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(Application.dataPath + FILENAME, FileMode.Open);
                load = (SaveSceneViewMKGlowBlock)formatter.Deserialize(stream);
                stream.Close();

                GetGlowLayerProperty();

                MKGlowComponent.GlowRenderLayer = load.m_GlowLayer;
                MKGlowComponent.GlowResolution = (MKGlowMode)load.m_GlowMode;
                MKGlowComponent.ShowCutoutGlow = load.m_ShowCutoutGlow;
                MKGlowComponent.ShowTransparentGlow = load.m_ShowTransparentGlow;
                MKGlowComponent.GlowType = (MKGlowType)load.m_GlowType;
                MKGlowComponent.GlowQuality = (MKGlowQuality)load.m_GlowQuality;
                MKGlowComponent.FullScreenGlowTint = new Color(load.m_FullScreenGlowTintX, load.m_FullScreenGlowTintY, load.m_FullScreenGlowTintZ, load.m_FullScreenGlowTintW);
                MKGlowComponent.BlurSpread = load.m_BlurSpread;
                MKGlowComponent.BlurIterations = load.m_BlurIterations;
                MKGlowComponent.GlowIntensity = load.m_GlowIntensity;
                MKGlowComponent.BlurOffset = load.m_BlurOffset;
                MKGlowComponent.Samples = load.m_Samples;
                MKGlowComponent.GlowCurve = (GlowBlurCurve)load.glowCurve;
                MKGlowComponent.enabled = load.sve;
                SceneView.RepaintAll();
            }
            else
            {
                ResetMKGlowSceneView();
                MKGlowComponent.enabled = true;
            }
        }

        private static void GetGlowLayerProperty()
        {
            var serializedObject = new UnityEditor.SerializedObject(MKGlowComponent);
            m_GlowLayerProperty = serializedObject.FindProperty("m_GlowRenderLayer");
        }
        private static void DrawProperties()
        {
            GetGlowLayerProperty();
            GUILayout.BeginVertical(m_Style);
            if ((int)MKGlowComponent.GlowType == 0)
                EditorGUILayout.PropertyField(m_GlowLayerProperty);

            MKGlowComponent.GlowResolution = (MKGlowMode)EditorGUILayout.EnumPopup("Glow Resolution", MKGlowComponent.GlowResolution);
            MKGlowComponent.GlowType = (MKGlowType)EditorGUILayout.EnumPopup("Glow Type", MKGlowComponent.GlowType);

            if ((int)MKGlowComponent.GlowType == 1)
                MKGlowComponent.FullScreenGlowTint = (Color)EditorGUILayout.ColorField("Glow Tint", MKGlowComponent.FullScreenGlowTint);

            MKGlowComponent.GlowQuality = (MKGlowQuality)EditorGUILayout.EnumPopup("Glow Quality", MKGlowComponent.GlowQuality);
            MKGlowComponent.GlowCurve = (GlowBlurCurve)EditorGUILayout.EnumPopup("Glow Curve", MKGlowComponent.GlowCurve);

            MKGlowComponent.BlurSpread = EditorGUILayout.Slider("Blur Spread", MKGlowComponent.BlurSpread, 0.2f, 1f);
            MKGlowComponent.BlurIterations = EditorGUILayout.IntSlider("Blur Iterations", MKGlowComponent.BlurIterations, 0, 11);
            MKGlowComponent.BlurOffset = EditorGUILayout.Slider("Blur Offset", MKGlowComponent.BlurOffset, 0f, 1f);
            MKGlowComponent.Samples = EditorGUILayout.IntSlider("Blur Samples", MKGlowComponent.Samples, 2, 16);
            MKGlowComponent.GlowIntensity = EditorGUILayout.Slider("Glow Intensity", MKGlowComponent.GlowIntensity, 0f, 1f);
            MKGlowComponent.ShowTransparentGlow = EditorGUILayout.Toggle("Show Transparent Glow", MKGlowComponent.ShowTransparentGlow);
            MKGlowComponent.ShowCutoutGlow = EditorGUILayout.Toggle("Show Cutout Glow", MKGlowComponent.ShowCutoutGlow);
            GUILayout.EndVertical();
        }

        public static MKGlow MKGlowComponent
        {
            get
            {
                if (m_MKGlowComponent == null)
                {
					try
					{
	                    if (SceneViewCamera.GetComponent<MKGlowSystem.MKGlow>() == null)
	                        m_MKGlowComponent = SceneViewCamera.gameObject.AddComponent<MKGlowSystemSV.MKSceneGlow>();
	                    else
	                        m_MKGlowComponent = SceneViewCamera.GetComponent<MKGlowSystemSV.MKSceneGlow>();
					}
					catch{}
                }
                return m_MKGlowComponent;
            }
        }
        private static Camera SceneViewCamera
        {
            get
            {
                if (m_SceneViewCamera == null)
                {
                    m_SceneViewCamera = SceneView.camera;
                }
                return m_SceneViewCamera;
            }
        }
        private static SceneView SceneView
        {
            get
            {
                if (m_SceneView == null)
                {
                    m_SceneView = EditorWindow.GetWindow<SceneView>();
                }
                return m_SceneView;
            }
        }

        [MenuItem("Window/MKGlowSystem/Scene Glow Settings")]
        private static void Init()
        {
			/*
            var filePath = Application.dataPath + "/_MK/_MKGlowSystem/_Internal/Editor/_Image/MKGlowSystemSceneViewSettingsTitle.png";
            if (System.IO.File.Exists(filePath))
            {
                var bytes = System.IO.File.ReadAllBytes(filePath);
                if (m_SceneViewLabel == null)
                {
                    m_SceneViewLabel = new Texture2D(128, 128);
                    m_SceneViewLabel.LoadImage(bytes);
                }
            }
            */
            SceneViewWindow.Show();
            SceneViewWindow.minSize = new Vector2(400, 275);
            SceneViewWindow.maxSize = new Vector2(400, 275);

            Load();
        }

        [MenuItem("Window/MKGlowSystem/Add MK Glow System To Selection")]
        private static void AddMKGlowToObject()
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                if (obj.GetComponent<MKGlow>() == null)
                    obj.AddComponent<MKGlow>();
            }
        }

        private void OnGUI()
        {
			/*
            if (m_SceneViewLabel != null)
            {
                GUILayout.BeginHorizontal(m_Style);
                GUILayout.FlexibleSpace();
                GUILayout.Label(m_SceneViewLabel);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            */
            DrawProperties();
            string s;
            GUILayout.BeginVertical(m_Style);
            if (MKGlowComponent.enabled == true)
            {
                s = "Disable SceneView Glow";
            }
            else
            {
                s = "Enable SceneView Glow";
            }
            if (GUILayout.Button(s))
            {
                MKGlowComponent.enabled = !MKGlowComponent.enabled;
                Save();
            }

            if (GUILayout.Button("Reset Settings"))
            {
                ResetMKGlowSceneView();
            }

            if (EditorPrefs.HasKey(""))
            {
                Debug.Log("dsfadf");
            }
            GUILayout.EndHorizontal();
            SceneView.RepaintAll();
        }

        private void OnDestroy()
        {
            Save();
        }

        private static void ResetMKGlowSceneView()
        {
            MKGlowComponent.GlowQuality = MKGlowQuality.High;
            MKGlowComponent.GlowResolution = MKGlowMode.High;
            MKGlowComponent.BlurIterations = 7;
            MKGlowComponent.BlurSpread = 0.35f;
            MKGlowComponent.Samples = 2;
            MKGlowComponent.BlurOffset = 0.0f;
            MKGlowComponent.GlowIntensity = 0.3f;
            MKGlowComponent.ShowTransparentGlow = true;
            MKGlowComponent.ShowCutoutGlow = false;
            MKGlowComponent.FullScreenGlowTint = new Color(1, 1, 1, 0);
            Save();
            Load();
        }
    }
}
#endif