///////////////////////////////////////////////
// MKGlowSystem	Editor						 //
//											 //
// Created by Michael Kremmel on 23.12.2014  //
// Copyright © 2015 All rights reserved.     //
///////////////////////////////////////////////

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using MKGlowSystem;
using MKGlowSystemSV;

namespace MKGlowSystemEditor
{
    [CustomEditor(typeof(MKGlow))]
    public class MKGlowEditor : Editor
    {
        private const string m_Style = "box";
        //private static Texture2D m_ComponentLabel;

        private SerializedProperty glowCurve;
        private SerializedProperty samples;
        private SerializedProperty blurSpread;
        private SerializedProperty blurIterations;
        private SerializedProperty blurOffset;
        private SerializedProperty glowIntensity;
        private SerializedProperty glowQuality;
        private SerializedProperty glowType;
        private SerializedProperty glowMode;
        private SerializedProperty fullScreenGlowTint;
        private SerializedProperty showTransparent;
        private SerializedProperty showCutout;
        private SerializedProperty glowLayer;
        //private bool helpToggle = false;

        private void OnEnable()
        {
			/*
            var filePath = Application.dataPath + "/_MK/_MKGlowSystem/_Internal/Editor/_Image/MKGlowSystemComponentTitle.png";
            if (System.IO.File.Exists(filePath))
            {
                var bytes = System.IO.File.ReadAllBytes(filePath);
                if (m_ComponentLabel == null)
                {
                    m_ComponentLabel = new Texture2D(128, 128);
                    m_ComponentLabel.LoadImage(bytes);
                }
            }
            */

            glowCurve = serializedObject.FindProperty("m_GlowCurve");
            samples = serializedObject.FindProperty("m_Samples");
            blurSpread = serializedObject.FindProperty("m_BlurSpread");
            blurIterations = serializedObject.FindProperty("m_BlurIterations");
            blurOffset = serializedObject.FindProperty("m_BlurOffset");
            glowIntensity = serializedObject.FindProperty("m_GlowIntensity");
            glowQuality = serializedObject.FindProperty("m_GlowQuality");
            glowType = serializedObject.FindProperty("m_GlowType");
            glowMode = serializedObject.FindProperty("m_GlowResolution");
            fullScreenGlowTint = serializedObject.FindProperty("m_FullScreenGlowTint");
            showTransparent = serializedObject.FindProperty("m_ShowTransparentGlow");
            showCutout = serializedObject.FindProperty("m_ShowCutoutGlow");
            glowLayer = serializedObject.FindProperty("m_GlowRenderLayer");

            MKGlowSystemSceneView.Load();
        }

        public override void OnInspectorGUI()
        {
			/*
            if (m_ComponentLabel != null)
            {
                GUILayout.BeginHorizontal(m_Style);
                GUILayout.FlexibleSpace();
                GUILayout.Label(m_ComponentLabel);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            */
            serializedObject.Update();

            //GUILayout.BeginVertical(m_Style);
            if (glowType.intValue == 0)
            {
                EditorGUILayout.PropertyField(glowLayer);
            }
            EditorGUILayout.PropertyField(glowMode);
            EditorGUILayout.PropertyField(glowType);
            EditorGUILayout.PropertyField(glowQuality);
            EditorGUILayout.PropertyField(glowCurve);

            //glowMode.enumValueIndex = (int)(MKGlowMode)EditorGUILayout.EnumPopup("Glow Resolution", (MKGlowMode)Enum.GetValues(typeof(MKGlowMode)).GetValue(glowMode.enumValueIndex));
            //glowType.enumValueIndex = (int)(MKGlowType)EditorGUILayout.EnumPopup("Glow Type", (MKGlowType)Enum.GetValues(typeof(MKGlowType)).GetValue(glowType.enumValueIndex));
            //glowQuality.enumValueIndex = (int)(MKGlowQuality)EditorGUILayout.EnumPopup("Glow Quality", (MKGlowQuality)Enum.GetValues(typeof(MKGlowQuality)).GetValue(glowQuality.enumValueIndex));

            if (glowType.intValue == 1)
            {
                fullScreenGlowTint.colorValue = EditorGUILayout.ColorField("Glow Tint", fullScreenGlowTint.colorValue);
            }

            EditorGUILayout.Slider(blurSpread, 0.2f, 2f, "Blur Spread");
            EditorGUILayout.IntSlider(blurIterations, 0, 11, "Blur Iterations");
            EditorGUILayout.Slider(blurOffset, 0f, 4f, "Blur Offset");
            EditorGUILayout.IntSlider(samples, 2, 16, "Blur Samples");
            EditorGUILayout.Slider(glowIntensity, 0f, 1f, "Glow Intensity");
            EditorGUILayout.PropertyField(showTransparent);
            EditorGUILayout.PropertyField(showCutout);
            if (glowMode.enumValueIndex != 2 || samples.intValue != 8 || glowQuality.enumValueIndex != 0 || blurIterations.intValue != 3 || glowCurve.intValue != 0)
            {
                if (glowMode.enumValueIndex == 2)
                {

                    if (GUILayout.Button(new GUIContent("Load Mobile Optimized Settings")))
                    {
                        glowMode.enumValueIndex = 2;
                        samples.intValue = 8;
                        glowQuality.enumValueIndex = 0;
                        blurIterations.intValue = 3;
                        glowCurve.intValue = 0;
                    }

                }
            }
            //GUILayout.EndVertical();

            //GUILayout.BeginVertical(m_Style);
            if (GUILayout.Button(new GUIContent("Copy Values To Scene View", "Copy current values to the Scene View")))
            {

                MKGlowSystemSceneView.SaveSceneViewMKGlowBlock copy = new MKGlowSystemSceneView.SaveSceneViewMKGlowBlock
                    (true, (int)glowLayer.intValue, (int)glowMode.intValue, (int)glowType.intValue, (int)glowQuality.intValue,
                     blurIterations.intValue, samples.intValue, fullScreenGlowTint.colorValue.r, fullScreenGlowTint.colorValue.g,
                     fullScreenGlowTint.colorValue.b, fullScreenGlowTint.colorValue.a, blurSpread.floatValue, glowIntensity.floatValue,
                     blurOffset.floatValue, showCutout.boolValue, showTransparent.boolValue, glowCurve.intValue);
                MKGlowSystemSceneView.Save(ref copy);
                MKGlowSystemSceneView.Load();
            }

            //EditorGUILayout.EndVertical();
            /*
            EditorGUILayout.LabelField("");
            helpToggle = EditorGUILayout.Toggle("Show Help/Tips: ", helpToggle);
            if (helpToggle)
            {
                GUI.contentColor = Color.green;
                EditorGUILayout.LabelField("Performance Tips: ");
                GUI.contentColor = Color.white;
                EditorGUILayout.LabelField("   + low blur iterations");
                EditorGUILayout.LabelField("   + high sampling");
                EditorGUILayout.LabelField("   + low glowresolution");
                EditorGUILayout.LabelField("   + low quality(less garbage)");
            }
            */
            serializedObject.ApplyModifiedProperties();

            //DrawDefaultInspector ();

        }
    }
}
#endif