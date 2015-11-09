///////////////////////////////////////////////
// MKGlowSystem								 //
//											 //
// Created by Michael Kremmel on 23.12.2014  //
// Copyright © 2015 All rights reserved.     //
///////////////////////////////////////////////

#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using MKGlowSystem;

namespace MKGlowSystemSV
{
	public class MKSceneGlow : MKGlow 
	{
		protected override void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			if (!gameObject.activeSelf || !this.enabled)
				return;

			try
			{
				if (GlowType == MKGlowType.Selective)
				{
					PerformSelectiveGlow(ref src, ref dest);
				}
				else
				{
					PerformFullScreenGlow(ref src, ref dest);
				}
			}
			catch{}
		}
	}
}
#endif
