using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows;
using Comm.Model;

namespace LumenDetection.Tests.CommonHelper
{
	public static class DrawLumensHelper
	{
		public static IEnumerable<LumensCoordinates> ConvertDataToFitScreen(IEnumerable<LumensCoordinates> lumens, double width = 640, double height = 480)
		{
			var fitLumens = new List<LumensCoordinates>();
			foreach (var data in lumens)
			{
				var x = data.X * width;
				var y = data.Y * height;
				if (x + data.Radius > width)
				{
					x -= data.Radius;
				}

				if (y + data.Radius > height)
				{
					y -= data.Radius;
				}

				fitLumens.Add(new LumensCoordinates(x,y,data.Radius));
			}

			return fitLumens;
		}

		public static BitmapSource ConvertVideoFrameByteArrayToBitmapSource(byte[] videoFrameBytes)
		{
			using (var memoryStream = new MemoryStream(videoFrameBytes))
			{
				// Create a Bitmap object from the byte array
				Bitmap bitmap = new Bitmap(memoryStream);

				// Convert the Bitmap to a BitmapSource using CreateBitmapSourceFromHBitmap
				IntPtr hBitmap = bitmap.GetHbitmap();
				BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
					hBitmap,
					IntPtr.Zero,
					Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions()
				);
				return bitmapSource;
			}
		}

		public static BitmapSource ConvertVideoFrameStringToBitmapSource(string videoFrameStr)
		{

			var videoFrameBytes = Convert.FromBase64String(videoFrameStr);
			using (var memoryStream = new MemoryStream(videoFrameBytes))
			{
				// Create a Bitmap object from the byte array
				Bitmap bitmap = new Bitmap(memoryStream);

				// Convert the Bitmap to a BitmapSource using CreateBitmapSourceFromHBitmap
				IntPtr hBitmap = bitmap.GetHbitmap();
				BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
					hBitmap,
					IntPtr.Zero,
					Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions()
				);
				return bitmapSource;
			}
		}

	}
}
