using System;
using System.Collections.Generic;
using Accord.Video.FFMPEG;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using System.Drawing.Imaging;
using System.IO;

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
			return _frames;
		}

		public byte[] ConvertFrameToBytes(Bitmap frame)
		{
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
				return frameAsBytes;
				// Process frame here (save it, send it over network, etc.)
				
			}

			return Array.Empty<byte>();
		}

		public Bitmap ConvertFrameFromBytes(byte[] frameAsBytes)
		{
			try
			{
				using (MemoryStream ms = new MemoryStream(frameAsBytes))
				{
					Bitmap bitmap = new Bitmap(ms);
					return bitmap;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error converting frame to Bitmap: {ex.Message}");
				return null; // or throw an exception based on your application's requirements
			}
		}

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
