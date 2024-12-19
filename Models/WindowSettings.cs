namespace AsaModCleaner.Models
{
    public class WindowSettings
    {
        public double Left { get; set; } = 100; // Default value for Left position
        public double Top { get; set; } = 100; // Default value for Top position
        public double Width { get; set; } = 800; // Default width
        public double Height { get; set; } = 600; // Default height
        public bool IsMaximized { get; set; } = false; // Default to non-maximized state
    }
}
