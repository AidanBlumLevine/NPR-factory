// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using UnityEditor;
// using UnityEngine;

// public class Thumbnailer : MonoBehaviour
// {
//     public string filename;
//     public int width = 265, height = 256, border = 100;
// }
// #if UNITY_EDITOR
// [CustomEditor(typeof(Thumbnailer))]
// class ThumbnailerEditor : Editor
// {
//     SerializedProperty filename, width, height, border;
//     void OnEnable()
//     {
//         filename = serializedObject.FindProperty("filename");
//         width = serializedObject.FindProperty("width");
//         height = serializedObject.FindProperty("height");
//         border = serializedObject.FindProperty("border");
//     }

//     public override void OnInspectorGUI()
//     {
//         serializedObject.Update();
//         EditorGUILayout.PropertyField(filename);
//         EditorGUILayout.PropertyField(width);
//         EditorGUILayout.PropertyField(border);
//         EditorGUILayout.PropertyField(height);
//         serializedObject.ApplyModifiedProperties();

//         if (GUILayout.Button("Snap"))
//         {
//             Thumbnailer tn = (Thumbnailer)target;
//             Camera cam = tn.GetComponent<Camera>();

//             var bak_cam_targetTexture = cam.targetTexture;
//             var bak_cam_clearFlags = cam.clearFlags;
//             var bak_RenderTexture_active = RenderTexture.active;

//             var tex_transparent = new Texture2D(tn.width, tn.height, TextureFormat.ARGB32, false);
//             // Must use 24-bit depth buffer to be able to fill background.
//             var render_texture = RenderTexture.GetTemporary(tn.width + tn.border * 2, tn.height + tn.border * 2, 24, RenderTextureFormat.ARGB32);
//             var grab_area = new Rect(tn.border, tn.border, tn.width, tn.height);

//             RenderTexture.active = render_texture;
//             cam.targetTexture = render_texture;
//             cam.clearFlags = CameraClearFlags.SolidColor;

//             // Simple: use a clear background
//             cam.backgroundColor = Color.clear;
//             cam.Render();
//             tex_transparent.ReadPixels(grab_area, 0, 0);
//             tex_transparent.Apply();

//             // Encode the resulting output texture to a byte array then write to the file
//             byte[] pngShot = ImageConversion.EncodeToPNG(tex_transparent);
//             File.WriteAllBytes(Application.dataPath + "/Thumbnails/captures/" + tn.filename + ".png", pngShot);

//             cam.clearFlags = bak_cam_clearFlags;
//             cam.targetTexture = bak_cam_targetTexture;
//             RenderTexture.active = bak_RenderTexture_active;
//             RenderTexture.ReleaseTemporary(render_texture);

//             Texture2D.DestroyImmediate(tex_transparent);
//             Debug.Log("Saved to: " + Application.dataPath + "/Thumbnails/captures/" + tn.filename + ".png");
//         }
//     }
// }
// #endif