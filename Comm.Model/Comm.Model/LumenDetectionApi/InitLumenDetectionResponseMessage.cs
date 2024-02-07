namespace Comm.Model.LumenDetectionApi
{
    public class InitLumenDetectionResponseMessage
    {
	    public string MessageHeader { get; set; } = "InitLumenDetectionResponse";
        public string Status { get; set; }

        public InitLumenDetectionResponseMessage()
        {
	        
        }

        public InitLumenDetectionResponseMessage(string status)
        {
	        Status = status;
        }
    }
}
