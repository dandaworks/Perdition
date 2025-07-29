using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine.Rendering;

namespace piqey.PS1
{
	internal partial class PS1Shader : BaseShaderGUI
	{
		// Properties
		private PS1GUI.PS1Properties _ps1Properties;

		// collect properties from the material properties
		public override void FindProperties(MaterialProperty[] properties)
		{
			base.FindProperties(properties);
			_ps1Properties = new PS1GUI.PS1Properties(properties);
		}

		// material changed check
		public override void ValidateMaterial(Material material)
		{
			base.ValidateMaterial(material);

			UpdateSamplerKeywords(material);
		}

		protected void UpdateSamplerKeywords(Material material)
		{
			if (_ps1Properties.sampler == null)
				return;

			PS1GUI.SamplerType samplerType = (PS1GUI.SamplerType)_ps1Properties.sampler.floatValue;

			foreach (KeyValuePair<PS1GUI.SamplerType, string> kv in PS1GUI.SamplerKeywords)
				CoreUtils.SetKeyword(material, kv.Value, kv.Key == samplerType);
		}

		// material main surface options
		public override void DrawSurfaceOptions(Material material)
		{
			if (!material)
				throw new ArgumentNullException(nameof(material));

			// Use default labelWidth
			EditorGUIUtility.labelWidth = 0.0f;

			base.DrawSurfaceOptions(material);
		}

		// material main surface inputs
		public override void DrawSurfaceInputs(Material material)
		{
			base.DrawSurfaceInputs(material);
			DrawEmissionProperties(material, true);
			DrawTileOffset(materialEditor, baseMapProp);

			PS1GUI.Inputs(_ps1Properties, materialEditor);
		}

		public override void DrawAdvancedOptions(Material material)
		{
			// PS1GUI.Advanced(shadingModelProperties);
			base.DrawAdvancedOptions(material);
		}

		public override void FillAdditionalFoldouts(MaterialHeaderScopeList materialScopesList) =>
            materialScopesList.RegisterHeaderScope(PS1GUI.Styles.PS1Inputs, Expandable.Details, _ => PS1GUI.DoPS1Area(_ps1Properties, materialEditor));

		public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
		{
			if (!material)
				throw new ArgumentNullException(nameof(material));

			// _Emission property is lost after assigning Standard shader to the material
			// thus transfer it before assigning the new shader
			if (material.HasProperty("_Emission"))
				material.SetColor("_EmissionColor", material.GetColor("_Emission"));

			base.AssignNewShaderToMaterial(material, oldShader, newShader);

			if (!oldShader || !oldShader.name.Contains("Legacy Shaders/"))
			{
				SetupMaterialBlendMode(material);
				return;
			}

			SurfaceType surfaceType = SurfaceType.Opaque;
			BlendMode blendMode = BlendMode.Alpha;

			if (oldShader.name.Contains("/Transparent/Cutout/"))
			{
				surfaceType = SurfaceType.Opaque;
				material.SetFloat("_AlphaClip", 1);
			}
			else if (oldShader.name.Contains("/Transparent/"))
			{
				// NOTE: legacy shaders did not provide physically based transparency
				// therefore Fade mode
				surfaceType = SurfaceType.Transparent;
				blendMode = BlendMode.Alpha;
			}

			material.SetFloat("_Surface", (float)surfaceType);
			material.SetFloat("_Blend", (float)blendMode);
		}
	}
}
