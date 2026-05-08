using DevTrack.Domain.Common;

namespace DevTrack.Domain.DTOs.QuickCapture;

public class QuickCaptureRequest
{
    public OwnerReference Owner { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
}
