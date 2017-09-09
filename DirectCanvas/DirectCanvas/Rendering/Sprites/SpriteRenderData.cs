using DirectCanvas.Rendering.Materials;

namespace DirectCanvas.Rendering.Sprites
{
    class SpriteRenderData
    {
        public SpriteRenderData()
        {
            drawData = new SpriteDrawData();
        }

        public SpriteDrawData drawData;
        public ShaderResourceTexture texture;
    }
}