/// © 2018-2022 Kevin Foley
/// For distribution only on the Unity Asset Store
/// Terms/EULA: https://unity3d.com/legal/as_terms

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Renders the scene using the specified resolution. We use [ExecuteInEditMode] so that changes
/// we make show up in real-time.
/// </summary>
/// <remarks>
/// COMPLEXITY: Advanced
/// CONCEPTS: Render textures, Post-processing, Blitting, Upscaling
/// 
/// https://docs.unity3d.com/Manual/class-RenderTexture.html
/// https://docs.unity3d.com/ScriptReference/Graphics.Blit.html
/// https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnRenderImage.html
/// 
/// This class configures the camera so that the image is rendered to a render texture at the desired 
/// render resolution. The rendered image is then blitted into the display backbuffer to be displayed 
/// at the actual window resolution. This is necessary if we want to render at a very low 
/// retro-resolution (e.g. 256x224) without blurring.
/// 
/// Normally, when Unity is rendered at a lower resolution than the monitor, the image is upscaled
/// with bilinear filtering, which can make the image extremely blurry if there is a significant
/// difference between the render resolution and the monitor/window resolution. Blitting from a 
/// RenderTexture to the display backbuffer allows us to use point (nearest-neighbor) filtering,
/// which preserves the sharp pixels we desire.
/// </remarks>
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PixelCamera : MonoBehaviour {
	[Min(32)]
	[SerializeField] private int verticalResolution = 244;
	[SerializeField] private bool verboseLogging = false;
	protected RenderTexture rt;

	protected Camera _camera;
	virtual protected Camera Camera {
		get {
			if (_camera == null) {
				_camera = GetComponent<Camera>();
			}
			return _camera;
		}
	}

	virtual public int VerticalResolution {
		get => verticalResolution;
		set {
			Assert.IsFalse(value < 1, "Vertical resolution must be positive");
			verticalResolution = value;
		}
	}

	// Use this for initialization
	virtual protected void Start () {
		UpdateSettings();
	}

	/// <summary>
	/// Apply settings from the Render Settings asset
	/// </summary>
	virtual protected void UpdateSettings() {
		if (Screen.width == 0 || Screen.height == 0) {
			Debug.Log("Screen size is 0; this can happen when the Editor first launches.");
			return;
		}
		float aspect = Screen.width / (float)Screen.height;
		int targetWidth = Mathf.FloorToInt(verticalResolution * aspect);
		if (rt == null) {
			rt = new RenderTexture(targetWidth, verticalResolution, 24);
			Camera.targetTexture = rt;
		}

		if (rt.width != targetWidth || rt.height != verticalResolution) {
			rt.Release();
			rt.width = targetWidth;
			rt.height = verticalResolution;
			if (verboseLogging) Debug.Log("Set RenderTexture width to " + rt.width + ", aspect " + aspect);
		}
	}

	virtual protected void OnPreRender() {
		if (Camera == null) return;

		//constantly check settings in the editor so we can see render-settings changes in real-time
#if UNITY_EDITOR
		UpdateSettings();
#endif
		//we null the camera's targetTexture in OnRenderImage(), and must re-apply it here
		Camera.targetTexture = rt;
	}

	//see https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnRenderImage.html
	virtual protected void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (Camera == null) return;

		Camera.targetTexture = null; //this is necessary so that our rendered image actually appears onscreen
		//copy our rendered image from the RenderTexture to the display backbuffer. this resizes the rendered image without blurring it
		source.filterMode = FilterMode.Point;
		Graphics.Blit(source, (RenderTexture)null);
		RenderTexture.active = rt;
	}
}
