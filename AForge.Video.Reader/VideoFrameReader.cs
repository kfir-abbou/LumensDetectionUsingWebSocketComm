using System;
using System.Collections.Generic;
using System.Diagnostics;
using Accord.Video.FFMPEG;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AForge.Video.Reader
{
	public class VideoFrameReader
	{
		private VideoFileReader _reader = new VideoFileReader();
		private readonly IList<byte[]> _frames = new List<byte[]>();

		public IList<Bitmap> GetVideoFrames(string file)
		{
			byte[] frameAsBytes = null;

			if (_reader == null)
			{
				_reader = new VideoFileReader();
			}

			_reader.Open(file);
			var frames = new List<Bitmap>();
			for (int i = 0; i < _reader.FrameCount; i++)
			{
				// Read next video frame
				Bitmap frame = _reader.ReadVideoFrame();
				frames.Add(frame);
			}
			return frames;
		}

		public IList<byte[]> GetVideoFileBytes(string file)
		{
			if (_reader == null)
			{
				_reader = new VideoFileReader();
			}

			_reader.Open(file);

			for (int i = 0; i < _reader.FrameCount; i++)
			{
				// Read next video frame
				Bitmap frame = _reader.ReadVideoFrame();

				// If the frame is not null then convert it to byte array
				var frameAsBytes = ConvertFrameToBytes(frame);
				_frames.Add(frameAsBytes);
				frame.Dispose();
			}

			_reader.Close();

			Span<byte[]> bytes = _frames.ToArray();
			return _frames;
		}
		public byte[] ConvertFrameToBytes(Bitmap frame)
		{
			_sw.Start();
			byte[] frameAsBytes;
			if (frame != null)
			{
				MemoryStream ms = new MemoryStream();
				var jpegEncoder = GetEncoder(ImageFormat.Jpeg);

				// Create an Encoder object for the Quality parameter category
				Encoder myEncoder = Encoder.Quality;

				// Create an EncoderParameters object
				EncoderParameters myEncoderParameters = new EncoderParameters(1);

				// Save the bitmap as a JPEG file with quality level
				EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L); // Adjust quality level here
				myEncoderParameters.Param[0] = myEncoderParameter;
				frame.Save(ms, jpegEncoder, myEncoderParameters);

				frameAsBytes = ms.ToArray();
				_sw.Stop();
				var t = _sw.Elapsed.TotalMilliseconds;
				return frameAsBytes;
				// Process frame here (save it, send it over network, etc.)
				
			}

			return Array.Empty<byte>();
		}
		private readonly MemoryStream _ms = new MemoryStream();

		// public Bitmap ConvertFrameFromBytes(byte[] frameAsBytes)
		// {
		// 	_ms.SetLength(0); //clear stream
		// 	_ms.Write(frameAsBytes, 0, frameAsBytes.Length);
		// 	_ms.Position = 0;
		//
		// 	using (var bitmap = new Bitmap(_ms))
		// 	{
		// 		//process bitmap
		// 		return bitmap;
		// 	}
		// }

		private Stopwatch _sw = new Stopwatch();

		public Bitmap ConvertFrameFromBytes(byte[] frameAsBytes)
		{
			_sw.Start();
			
			var width = 640;
			var height = 480;
			using (var bitmap = new Bitmap(frameAsBytes.Length, width, height, PixelFormat.Format24bppRgb, IntPtr.Zero))
			{
				BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
				Marshal.Copy(frameAsBytes, 0, bmpData.Scan0, frameAsBytes.Length);
				bitmap.UnlockBits(bmpData);

				//process bitmap
				_sw.Stop();
				var total = _sw.Elapsed;
				return bitmap;
			}
		}


		// public Bitmap ConvertFrameFromBytes(byte[] frameAsBytes)
		// {
		// 	try
		// 	{
		// 		using (MemoryStream ms = new MemoryStream(frameAsBytes))
		// 		{
		// 			Bitmap bitmap = new Bitmap(ms);
		// 			return bitmap;
		// 		}
		// 	}
		// 	catch (Exception ex)
		// 	{
		// 		Console.WriteLine($"Error converting frame to Bitmap: {ex.Message}");
		// 		return null; // or throw an exception based on your application's requirements
		// 	}
		// }

		private static ImageCodecInfo GetEncoder(ImageFormat format)
		{
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

			foreach (ImageCodecInfo codec in codecs)
			{
				if (codec.FormatID == format.Guid)
				{
					return codec;
				}
			}

			return null;
		}
	}
}
