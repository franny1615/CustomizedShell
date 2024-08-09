namespace FDMobile.Pages;

public class DetectionsDrawable : IDrawable
{
    public  MoveNetPacket? Packet { get; set; } 

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // The seventeen points (y, x, predictionConfidence)
		// [ nose, left eye, right eye, left ear, right ear, 
		//   left shoulder, right shoulder, left elbow, 
		//   right elbow, left wrist, right wrist, left hip, 
		//   right hip, left knee, right knee, left ankle, right ankle ]
		//   y,x come in as percentage from 0..1

		if (Packet == null)
			return;
		
		var height = dirtyRect.Height;
		var width = dirtyRect.Width;

		var noseY = Packet.Detections[0] * height;
		var noseX = Packet.Detections[1] * width;
		var noseConfidence = Packet.Detections[2];
		
		var leftEyeY = Packet.Detections[3] * height;
		var leftEyeX = Packet.Detections[4] * width;
		var leftEyeConfidence = Packet.Detections[5];

		var rightEyeY = Packet.Detections[6] * height; 
		var rightEyeX = Packet.Detections[7] * width;
		var rightEyeConfidence = Packet.Detections[8];

		canvas.FillColor = Colors.Red;
		canvas.FillCircle(noseX, noseY, 5f);
		canvas.FillCircle(leftEyeX, leftEyeY, 5f);
		canvas.FillCircle(rightEyeX, rightEyeY, 5f);
    }
}