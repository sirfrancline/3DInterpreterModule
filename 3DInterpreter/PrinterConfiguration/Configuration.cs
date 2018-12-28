namespace PrinterConfiguration
{
    public class Configuration
    {
        public float XStepsPerMM { get; set; } = 200;

        public float XMaxSpeedPerMM { get; set; } = 20;
        public int XMaxAcceleration { get; set; } = 40;

        public float YStepsPerMM { get; set; } = 200;
        public float ZStepsPerMM { get; set; } = 200;

        public int XJerk { get; set; } = 20;
        public int YMaxAcceleration { get; set; } = 60;
        public int ZMaxAcceleration { get; set; } = 60;
    }
}