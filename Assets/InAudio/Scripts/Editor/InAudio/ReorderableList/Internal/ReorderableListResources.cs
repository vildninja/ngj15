// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using UnityEngine;

namespace InAudioSystem.ReorderableList.Internal {

	/// <summary>
	/// Resources to assist with reorderable list control.
	/// </summary>
	internal static class ReorderableListResources {

		#region Texture Resources

		/*private enum ResourceName {
			add_button = 0,
			add_button_active,
			container_background,
			grab_handle,
			remove_button,
			remove_button_active,
			title_background,
		}*/


		/// <summary>
		/// Gets light or dark texture "add_button.png".
		/// </summary>
		public static Texture2D texAddButton {
			//get { /*return s_Cached[(int)ResourceName.add_button];*/ }
            get { return null; }
		}
		/// <summary>
		/// Gets light or dark texture "add_button_active.png".
		/// </summary>
		public static Texture2D texAddButtonActive {
			//get {/* return s_Cached[(int)ResourceName.add_button_active]; */}
            get { return null; }
		}
		/// <summary>
		/// Gets light or dark texture "container_background.png".
		/// </summary>
		public static Texture2D texContainerBackground {
			//get { return s_Cached[(int)ResourceName.container_background]; }
            get { return null; }
		}
		/// <summary>
		/// Gets light or dark texture "grab_handle.png".
		/// </summary>
		public static Texture2D texGrabHandle {
			//get { return s_Cached[(int)ResourceName.grab_handle]; }
            get { return null; }
		}
		/// <summary>
		/// Gets light or dark texture "remove_button.png".
		/// </summary>
		public static Texture2D texRemoveButton {
			//get { /*return s_Cached[(int)ResourceName.remove_button]; */}
            get { return null; }
		}
		/// <summary>
		/// Gets light or dark texture "remove_button_active.png".
		/// </summary>
		public static Texture2D texRemoveButtonActive {
			//get {/* return s_Cached[(int)ResourceName.remove_button_active]; */}
            get { return null; }
		}
		/// <summary>
		/// Gets light or dark texture "title_background.png".
		/// </summary>
		public static Texture2D texTitleBackground {
			//get { return s_Cached[(int)ResourceName.title_background]; }
            get { return null; }
		}

		#endregion

		#region Generated Resources


		/// <summary>
		/// Create 1x1 pixel texture of specified color.
		/// </summary>
		/// <param name="name">Name for texture object.</param>
		/// <param name="color">Pixel color.</param>
		/// <returns>
		/// The new <c>Texture2D</c> instance.
		/// </returns>
		public static Texture2D CreatePixelTexture(string name, Color color) {
			var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			tex.name = name;
			tex.hideFlags = HideFlags.HideAndDontSave;
			tex.filterMode = FilterMode.Point;
			tex.SetPixel(0, 0, color);
			tex.Apply();
			return tex;
		}

		#endregion

		#region Load PNG from Base-64 Encoded String

		//private static Texture2D[] s_Cached; 


		/// <summary>
		/// Read width and height if PNG file in pixels.
		/// </summary>
		/// <param name="imageData">PNG image data.</param>
		/// <param name="width">Width of image in pixels.</param>
		/// <param name="height">Height of image in pixels.</param> 
		private static void GetImageSize(byte[] imageData, out int width, out int height) {
			width = ReadInt(imageData, 3 + 15);
			height = ReadInt(imageData, 3 + 15 + 2 + 2);
		}

		private static int ReadInt(byte[] imageData, int offset) {
			return (imageData[offset] << 8) | imageData[offset + 1];
		}

		#endregion

	}

}