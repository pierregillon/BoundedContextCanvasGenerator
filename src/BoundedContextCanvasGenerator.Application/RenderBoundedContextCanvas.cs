using BoundedContextCanvasGenerator.Domain.BC;

namespace BoundedContextCanvasGenerator.Application;

public class RenderBoundedContextCanvas
{
    private readonly IBoundedContextCanvasRenderer _boundedContextCanvasRenderer;

    public RenderBoundedContextCanvas(IBoundedContextCanvasRenderer boundedContextCanvasRenderer) => _boundedContextCanvasRenderer = boundedContextCanvasRenderer;

    public async Task<Bytes> Export(BoundedContextCanvas boundedContextCanvas) => await _boundedContextCanvasRenderer.Render(boundedContextCanvas);
}