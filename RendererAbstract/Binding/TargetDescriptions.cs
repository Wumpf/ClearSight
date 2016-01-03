using System.Runtime.InteropServices;
using ClearSight.RendererAbstract.Memory;

namespace ClearSight.RendererAbstract.Binding
{
    [StructLayout(LayoutKind.Explicit)]
    public struct RenderTargetViewDescription
    {
        [FieldOffset(0)] public Format Format;
        [FieldOffset(4)] public Dimension Dimension;

        [FieldOffset(8)] public BufferSubresourceDesc Buffer;
        [FieldOffset(8)] public TextureSubresourceDesc Texture;
    }

    public struct TextureSubresourceDesc
    {
        public TextureSubresourceDesc(uint mipSlice = 0)
        {
            MipSlice = mipSlice;
            FirstSlice = 0;
            SliceCount = 0;
        }

        public uint MipSlice;

        /// <summary>
        /// For 3D texture this is the depth slice. For array textures this is the array level.
        /// Otherwise this field has no meaning.
        /// </summary>
        public uint FirstSlice;
        public uint SliceCount;

        // Currently not supported: "PlaneSlice" which is used to target only certain format channels.
    }

    public struct BufferSubresourceDesc
    {
        public uint FirstElement;
        public uint ElementCount;
    }
}