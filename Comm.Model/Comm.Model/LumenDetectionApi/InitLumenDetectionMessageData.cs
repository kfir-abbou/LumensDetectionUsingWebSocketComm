
namespace Comm.Model
{
	public class InitLumenDetectionMessageData
	{
		public string ModelCheckpointsFilePath { get; set; }
		public string ModelTrainingConfigurationFilePath { get; set; }
		public string PostProcessingConfigurationFilePath { get; set; }
		public string CameraCalibrationFilePath { get; set; }
	}
}
