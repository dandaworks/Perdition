using UnityEngine;
using UnityEditor;
using static UnityEditor.Rendering.MaterialEditorExtension;
using System.Collections.Generic;

namespace piqey.PS1
{
	internal partial class PS1Shader : BaseShaderGUI
	{
		/// <summary>
		/// Editor script for the PS1 shader's material inspector.
		/// </summary>
		public static class PS1GUI
		{
			/// <summary>
			/// Options for specular source.
			/// </summary>
			// public enum SpecularSource
			// {
			// 	/// <summary>
			// 	/// Use this to use specular texture and color.
			// 	/// </summary>
			// 	SpecularTextureAndColor,

			// 	/// <summary>
			// 	/// Use this when not using specular.
			// 	/// </summary>
			// 	NoSpecular
			// }

			/// <summary>
			/// Options to select the texture channel where the smoothness value is stored.
			/// </summary>
			// public enum SmoothnessMapChannel
			// {
			// 	/// <summary>
			// 	/// Use this when smoothness is stored in the alpha channel of the Specular Map.
			// 	/// </summary>
			// 	SpecularAlpha,

			// 	/// <summary>
			// 	/// Use this when smoothness is stored in the alpha channel of the Albedo Map.
			// 	/// </summary>
			// 	AlbedoAlpha,
			// }

			/// <summary>
			/// Internal container for the text and tooltips used to display <see cref="PS1Shader" />'s material properties.
			/// </summary>
			internal static class Styles
			{
				// public static readonly GUIContent Smoothness = EditorGUIUtility.TrTextContent("Smoothness",
				// 	"Smoothness value.\nI believe this is currently useless; I'm probably missing a preprocessor directive in my shader for a lighting feature.");

				// public static readonly GUIContent Specular = EditorGUIUtility.TrTextContent("Specular",
				// 	"Smoothness value.\nI believe this is currently useless; I'm probably missing a preprocessor directive in my shader for a lighting feature.");

				public static readonly GUIContent PS1Inputs = EditorGUIUtility.TrTextContent("PS1 Inputs",
					"These inputs allow for per-material control over several effects implemented to achieve the PS1 aesthetic.");

				public static readonly GUIContent Jitter = EditorGUIUtility.TrTextContent("Vertex Jitter",
					"Enables/disables vertex jitter, i.e., snapping of mesh vertices to a clip-space grid of configurable scale.");

				public static readonly GUIContent JitterGridScale = EditorGUIUtility.TrTextContent("Grid Scale",
					"Grid scale to use for clip-space vertex jitter. Larger values result in less perceived jitter.");

				public static readonly GUIContent JitterPixelSnap = EditorGUIUtility.TrTextContent("Pixel Snapping",
					"Enables/disables the snapping of mesh vertices to the nearest on-screen pixel (this occurs in clip space).");

				public static readonly GUIContent Affine = EditorGUIUtility.TrTextContent("Affine Texture Mapping",
					"Enables/disables affine UV texture mapping.");

				public static readonly GUIContent Sampler = EditorGUIUtility.TrTextContent("Texture Sampling Override",
					"Overrides texture sampling options (as opposed to using the ones specified in the Inspector window when selecting a texture).");

				public static readonly string[] SamplerNames = System.Enum.GetNames(typeof(SamplerType));

				/// <summary>
				/// The text and tooltip for the specular map GUI.
				/// </summary>
				// public static GUIContent specularMapText =
				// 	EditorGUIUtility.TrTextContent("Specular Map", "Designates a Specular Map and specular color determining the apperance of reflections on this Material's surface.");
			}

			public enum SamplerType
			{
				InheritFromTextures = 0,
				PointClamp = 1,
				PointRepeat = 2
			}

			public static readonly Dictionary<SamplerType, string> SamplerKeywords = new()
			{
				{ SamplerType.PointClamp,  "_PS1_SAMPLER_POINTCLAMP"  },
				{ SamplerType.PointRepeat, "_PS1_SAMPLER_POINTREPEAT" },
			};

			/// <summary>
			/// Container for the properties used in the <see cref="PS1Shader" /> editor script.
			/// </summary>
			public struct PS1Properties
			{
				public MaterialProperty jitter;
				public MaterialProperty jitterGridScale;

				public MaterialProperty pixelSnap;

				public MaterialProperty affine;

				public MaterialProperty sampler;

				/// <summary>
				/// The MaterialProperty for normal map.
				/// </summary>
				public MaterialProperty bumpMapProp;
				public MaterialProperty bumpScaleProp;

				/// <summary>
				/// Constructor for the <see cref="PS1Properties" /> container struct.
				/// </summary>
				/// <param name="properties"></param>
				public PS1Properties(MaterialProperty[] properties)
				{
					// Specular Properties
					// smoothness = FindProperty("_Smoothness", properties, false);
					// specular = FindProperty("_Specular", properties, false);

					// PS1 Properties

					jitter = FindProperty("_Jitter", properties, false);
					jitterGridScale = FindProperty("_JitterGridScale", properties, false);
					pixelSnap = FindProperty("_PixelSnap", properties, false);

					affine = FindProperty("_Affine", properties, false);

					sampler = FindProperty("_PS1_SAMPLER", properties, false);

					// Misc.

					bumpMapProp = FindProperty("_BumpMap", properties, false);
					bumpScaleProp = FindProperty("_BumpScale", properties, false);
				}
			}

			/// <summary>
			/// Adds to the surface inputs GUI.
			/// </summary>
			/// <param name="properties"></param>
			/// <param name="materialEditor"></param>
			public static void Inputs(PS1Properties properties, MaterialEditor materialEditor)
			{
				// DoSpecularArea(properties, materialEditor);
				DrawNormalArea(materialEditor, properties.bumpMapProp, properties.bumpScaleProp);
			}

			/// <summary>
			/// Draws the specular area GUI.
			/// </summary>
			/// <param name="properties"></param>
			/// <param name="materialEditor"></param>
			// public static void DoSpecularArea(PS1Properties properties, MaterialEditor materialEditor)
			// {
				// SpecularSource specSource = (SpecularSource)properties.specHighlights.floatValue;
				// // EditorGUI.BeginDisabledGroup(specSource == SpecularSource.NoSpecular);
				// BaseShaderGUI.TextureColorProps(materialEditor, Styles.specularMapText, properties.specGlossMap, properties.specColor, true);
				// LitGUI.DoSmoothness(materialEditor, material, properties.smoothness, properties.smoothnessMapChannel, LitGUI.Styles.specularSmoothnessChannelNames);
				// EditorGUI.EndDisabledGroup();

				// materialEditor.ShaderProperty(properties.smoothness, Styles.Smoothness);
				// materialEditor.ShaderProperty(properties.specular, Styles.Specular);
			// }

			/// <summary>
			/// Draws the PS1 area GUI.
			/// </summary>
			/// <param name="properties"></param>
			/// <param name="materialEditor"></param>
			public static void DoPS1Area(PS1Properties properties, MaterialEditor materialEditor)
			{
				materialEditor.ShaderProperty(properties.jitter, Styles.Jitter);

				bool isJitterDisabled = properties.jitter.floatValue == 0.0f;
				EditorGUI.BeginDisabledGroup(isJitterDisabled);
					materialEditor.ShaderProperty(properties.jitterGridScale, Styles.JitterGridScale, 1);
					materialEditor.ShaderProperty(properties.pixelSnap, Styles.JitterPixelSnap, 1);
				EditorGUI.EndDisabledGroup();

				materialEditor.ShaderProperty(properties.affine, Styles.Affine);

				DoPopup(materialEditor, Styles.Sampler, properties.sampler, Styles.SamplerNames);
				// if (properties.sampler != null && (SamplerType)properties.sampler.floatValue == )
			}

			internal static void DoPopup(MaterialEditor materialEditor, GUIContent label, MaterialProperty property, string[] options)
			{
				if (property != null)
					materialEditor.PopupShaderProperty(property, label, options);
			}
		}
	}
}
