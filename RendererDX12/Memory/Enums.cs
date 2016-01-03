using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClearSight.RendererAbstract.Memory;

namespace ClearSight.RendererDX12.Memory
{
    public static class Enums
    {
        /// <summary>
        /// Conversion lookup table to convert own format to DXGI format.
        /// </summary>
        public static readonly SharpDX.DXGI.Format[] ToDXGIFormat = new SharpDX.DXGI.Format[Enum.GetNames(typeof (ClearSight.RendererAbstract.Memory.Format)).Length];

        /// <summary>
        /// Conversion lookup table from memory dimension definition to DX RTV dimension
        /// </summary>
        public static readonly SharpDX.Direct3D12.RenderTargetViewDimension[] ToRTVDimension = new SharpDX.Direct3D12.RenderTargetViewDimension[Enum.GetNames(typeof(ClearSight.RendererAbstract.Memory.Dimension)).Length];

        internal static void InitLookupTables()
        {
            // Default of ToRTVDimension is 0 which is "Unknown" which usually leads to fails.
            ToRTVDimension[(int)Dimension.Buffer] = SharpDX.Direct3D12.RenderTargetViewDimension.Buffer;
            ToRTVDimension[(int)Dimension.Texture1D] = SharpDX.Direct3D12.RenderTargetViewDimension.Texture1D;
            ToRTVDimension[(int)Dimension.Texture1DArray] = SharpDX.Direct3D12.RenderTargetViewDimension.Texture1DArray;
            ToRTVDimension[(int)Dimension.Texture2D] = SharpDX.Direct3D12.RenderTargetViewDimension.Texture2D;
            ToRTVDimension[(int)Dimension.Texture2DArray] = SharpDX.Direct3D12.RenderTargetViewDimension.Texture2DArray;
            ToRTVDimension[(int)Dimension.Texture2DMs] = SharpDX.Direct3D12.RenderTargetViewDimension.Texture2DMultisampled;
            ToRTVDimension[(int)Dimension.Texture2DMsArray] = SharpDX.Direct3D12.RenderTargetViewDimension.Texture2DMultisampledArray;
            ToRTVDimension[(int)Dimension.Texture3D] = SharpDX.Direct3D12.RenderTargetViewDimension.Texture3D;

            // Default of ToDXGIFormat is 0 which is "Unknown".
            // Direct assignment is way safer against changes than a compile time array.
            ToDXGIFormat[(int)Format.R32G32B32A32_Typeless] = SharpDX.DXGI.Format.R32G32B32A32_Typeless;
            ToDXGIFormat[(int)Format.R32G32B32A32_Float] = SharpDX.DXGI.Format.R32G32B32A32_Float;
            ToDXGIFormat[(int)Format.R32G32B32A32_UInt] = SharpDX.DXGI.Format.R32G32B32A32_UInt;
            ToDXGIFormat[(int)Format.R32G32B32A32_SInt] = SharpDX.DXGI.Format.R32G32B32A32_SInt;
            ToDXGIFormat[(int)Format.R32G32B32_Typeless] = SharpDX.DXGI.Format.R32G32B32_Typeless;
            ToDXGIFormat[(int)Format.R32G32B32_Float] = SharpDX.DXGI.Format.R32G32B32_Float;
            ToDXGIFormat[(int)Format.R32G32B32_UInt] = SharpDX.DXGI.Format.R32G32B32_UInt;
            ToDXGIFormat[(int)Format.R32G32B32_SInt] = SharpDX.DXGI.Format.R32G32B32_SInt;
            ToDXGIFormat[(int)Format.R16G16B16A16_Typeless] = SharpDX.DXGI.Format.R16G16B16A16_Typeless;
            ToDXGIFormat[(int)Format.R16G16B16A16_Float] = SharpDX.DXGI.Format.R16G16B16A16_Float;
            ToDXGIFormat[(int)Format.R16G16B16A16_UNorm] = SharpDX.DXGI.Format.R16G16B16A16_UNorm;
            ToDXGIFormat[(int)Format.R16G16B16A16_UInt] = SharpDX.DXGI.Format.R16G16B16A16_UInt;
            ToDXGIFormat[(int)Format.R16G16B16A16_SNorm] = SharpDX.DXGI.Format.R16G16B16A16_SNorm;
            ToDXGIFormat[(int)Format.R16G16B16A16_SInt] = SharpDX.DXGI.Format.R16G16B16A16_SInt;
            ToDXGIFormat[(int)Format.R32G32_Typeless] = SharpDX.DXGI.Format.R32G32_Typeless;
            ToDXGIFormat[(int)Format.R32G32_Float] = SharpDX.DXGI.Format.R32G32_Float;
            ToDXGIFormat[(int)Format.R32G32_UInt] = SharpDX.DXGI.Format.R32G32_UInt;
            ToDXGIFormat[(int)Format.R32G32_SInt] = SharpDX.DXGI.Format.R32G32_SInt;
            ToDXGIFormat[(int)Format.R32G8X24_Typeless] = SharpDX.DXGI.Format.R32G8X24_Typeless;
            ToDXGIFormat[(int)Format.D32_Float_S8X24_UInt] = SharpDX.DXGI.Format.D32_Float_S8X24_UInt;
            ToDXGIFormat[(int)Format.R32_Float_X8X24_Typeless] = SharpDX.DXGI.Format.R32_Float_X8X24_Typeless;
            ToDXGIFormat[(int)Format.X32_Typeless_G8X24_UInt] = SharpDX.DXGI.Format.X32_Typeless_G8X24_UInt;
            ToDXGIFormat[(int)Format.R10G10B10A2_Typeless] = SharpDX.DXGI.Format.R10G10B10A2_Typeless;
            ToDXGIFormat[(int)Format.R10G10B10A2_UNorm] = SharpDX.DXGI.Format.R10G10B10A2_UNorm;
            ToDXGIFormat[(int)Format.R10G10B10A2_UInt] = SharpDX.DXGI.Format.R10G10B10A2_UInt;
            ToDXGIFormat[(int)Format.R11G11B10_Float] = SharpDX.DXGI.Format.R11G11B10_Float;
            ToDXGIFormat[(int)Format.R8G8B8A8_Typeless] = SharpDX.DXGI.Format.R8G8B8A8_Typeless;
            ToDXGIFormat[(int)Format.R8G8B8A8_UNorm] = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
            ToDXGIFormat[(int)Format.R8G8B8A8_UNorm_SRgb] = SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb;
            ToDXGIFormat[(int)Format.R8G8B8A8_UInt] = SharpDX.DXGI.Format.R8G8B8A8_UInt;
            ToDXGIFormat[(int)Format.R8G8B8A8_SNorm] = SharpDX.DXGI.Format.R8G8B8A8_SNorm;
            ToDXGIFormat[(int)Format.R8G8B8A8_SInt] = SharpDX.DXGI.Format.R8G8B8A8_SInt;
            ToDXGIFormat[(int)Format.R16G16_Typeless] = SharpDX.DXGI.Format.R16G16_Typeless;
            ToDXGIFormat[(int)Format.R16G16_Float] = SharpDX.DXGI.Format.R16G16_Float;
            ToDXGIFormat[(int)Format.R16G16_UNorm] = SharpDX.DXGI.Format.R16G16_UNorm;
            ToDXGIFormat[(int)Format.R16G16_UInt] = SharpDX.DXGI.Format.R16G16_UInt;
            ToDXGIFormat[(int)Format.R16G16_SNorm] = SharpDX.DXGI.Format.R16G16_SNorm;
            ToDXGIFormat[(int)Format.R16G16_SInt] = SharpDX.DXGI.Format.R16G16_SInt;
            ToDXGIFormat[(int)Format.R32_Typeless] = SharpDX.DXGI.Format.R32_Typeless;
            ToDXGIFormat[(int)Format.D32_Float] = SharpDX.DXGI.Format.D32_Float;
            ToDXGIFormat[(int)Format.R32_Float] = SharpDX.DXGI.Format.R32_Float;
            ToDXGIFormat[(int)Format.R32_UInt] = SharpDX.DXGI.Format.R32_UInt;
            ToDXGIFormat[(int)Format.R32_SInt] = SharpDX.DXGI.Format.R32_SInt;
            ToDXGIFormat[(int)Format.R24G8_Typeless] = SharpDX.DXGI.Format.R24G8_Typeless;
            ToDXGIFormat[(int)Format.D24_UNorm_S8_UInt] = SharpDX.DXGI.Format.D24_UNorm_S8_UInt;
            ToDXGIFormat[(int)Format.R24_UNorm_X8_Typeless] = SharpDX.DXGI.Format.R24_UNorm_X8_Typeless;
            ToDXGIFormat[(int)Format.X24_Typeless_G8_UInt] = SharpDX.DXGI.Format.X24_Typeless_G8_UInt;
            ToDXGIFormat[(int)Format.R8G8_Typeless] = SharpDX.DXGI.Format.R8G8_Typeless;
            ToDXGIFormat[(int)Format.R8G8_UNorm] = SharpDX.DXGI.Format.R8G8_UNorm;
            ToDXGIFormat[(int)Format.R8G8_UInt] = SharpDX.DXGI.Format.R8G8_UInt;
            ToDXGIFormat[(int)Format.R8G8_SNorm] = SharpDX.DXGI.Format.R8G8_SNorm;
            ToDXGIFormat[(int)Format.R8G8_SInt] = SharpDX.DXGI.Format.R8G8_SInt;
            ToDXGIFormat[(int)Format.R16_Typeless] = SharpDX.DXGI.Format.R16_Typeless;
            ToDXGIFormat[(int)Format.R16_Float] = SharpDX.DXGI.Format.R16_Float;
            ToDXGIFormat[(int)Format.D16_UNorm] = SharpDX.DXGI.Format.D16_UNorm;
            ToDXGIFormat[(int)Format.R16_UNorm] = SharpDX.DXGI.Format.R16_UNorm;
            ToDXGIFormat[(int)Format.R16_UInt] = SharpDX.DXGI.Format.R16_UInt;
            ToDXGIFormat[(int)Format.R16_SNorm] = SharpDX.DXGI.Format.R16_SNorm;
            ToDXGIFormat[(int)Format.R16_SInt] = SharpDX.DXGI.Format.R16_SInt;
            ToDXGIFormat[(int)Format.R8_Typeless] = SharpDX.DXGI.Format.R8_Typeless;
            ToDXGIFormat[(int)Format.R8_UNorm] = SharpDX.DXGI.Format.R8_UNorm;
            ToDXGIFormat[(int)Format.R8_UInt] = SharpDX.DXGI.Format.R8_UInt;
            ToDXGIFormat[(int)Format.R8_SNorm] = SharpDX.DXGI.Format.R8_SNorm;
            ToDXGIFormat[(int)Format.R8_SInt] = SharpDX.DXGI.Format.R8_SInt;
            ToDXGIFormat[(int)Format.A8_UNorm] = SharpDX.DXGI.Format.A8_UNorm;
            ToDXGIFormat[(int)Format.R1_UNorm] = SharpDX.DXGI.Format.R1_UNorm;
            ToDXGIFormat[(int)Format.R9G9B9E5_Sharedexp] = SharpDX.DXGI.Format.R9G9B9E5_Sharedexp;
            ToDXGIFormat[(int)Format.R8G8_B8G8_UNorm] = SharpDX.DXGI.Format.R8G8_B8G8_UNorm;
            ToDXGIFormat[(int)Format.G8R8_G8B8_UNorm] = SharpDX.DXGI.Format.G8R8_G8B8_UNorm;
            ToDXGIFormat[(int)Format.BC1_Typeless] = SharpDX.DXGI.Format.BC1_Typeless;
            ToDXGIFormat[(int)Format.BC1_UNorm] = SharpDX.DXGI.Format.BC1_UNorm;
            ToDXGIFormat[(int)Format.BC1_UNorm_SRgb] = SharpDX.DXGI.Format.BC1_UNorm_SRgb;
            ToDXGIFormat[(int)Format.BC2_Typeless] = SharpDX.DXGI.Format.BC2_Typeless;
            ToDXGIFormat[(int)Format.BC2_UNorm] = SharpDX.DXGI.Format.BC2_UNorm;
            ToDXGIFormat[(int)Format.BC2_UNorm_SRgb] = SharpDX.DXGI.Format.BC2_UNorm_SRgb;
            ToDXGIFormat[(int)Format.BC3_Typeless] = SharpDX.DXGI.Format.BC3_Typeless;
            ToDXGIFormat[(int)Format.BC3_UNorm] = SharpDX.DXGI.Format.BC3_UNorm;
            ToDXGIFormat[(int)Format.BC3_UNorm_SRgb] = SharpDX.DXGI.Format.BC3_UNorm_SRgb;
            ToDXGIFormat[(int)Format.BC4_Typeless] = SharpDX.DXGI.Format.BC4_Typeless;
            ToDXGIFormat[(int)Format.BC4_UNorm] = SharpDX.DXGI.Format.BC4_UNorm;
            ToDXGIFormat[(int)Format.BC4_SNorm] = SharpDX.DXGI.Format.BC4_SNorm;
            ToDXGIFormat[(int)Format.BC5_Typeless] = SharpDX.DXGI.Format.BC5_Typeless;
            ToDXGIFormat[(int)Format.BC5_UNorm] = SharpDX.DXGI.Format.BC5_UNorm;
            ToDXGIFormat[(int)Format.BC5_SNorm] = SharpDX.DXGI.Format.BC5_SNorm;
            ToDXGIFormat[(int)Format.B5G6R5_UNorm] = SharpDX.DXGI.Format.B5G6R5_UNorm;
            ToDXGIFormat[(int)Format.B5G5R5A1_UNorm] = SharpDX.DXGI.Format.B5G5R5A1_UNorm;
            ToDXGIFormat[(int)Format.B8G8R8A8_UNorm] = SharpDX.DXGI.Format.B8G8R8A8_UNorm;
            ToDXGIFormat[(int)Format.B8G8R8X8_UNorm] = SharpDX.DXGI.Format.B8G8R8X8_UNorm;
            ToDXGIFormat[(int)Format.R10G10B10_Xr_Bias_A2_UNorm] = SharpDX.DXGI.Format.R10G10B10_Xr_Bias_A2_UNorm;
            ToDXGIFormat[(int)Format.B8G8R8A8_Typeless] = SharpDX.DXGI.Format.B8G8R8A8_Typeless;
            ToDXGIFormat[(int)Format.B8G8R8A8_UNorm_SRgb] = SharpDX.DXGI.Format.B8G8R8A8_UNorm_SRgb;
            ToDXGIFormat[(int)Format.B8G8R8X8_Typeless] = SharpDX.DXGI.Format.B8G8R8X8_Typeless;
            ToDXGIFormat[(int)Format.B8G8R8X8_UNorm_SRgb] = SharpDX.DXGI.Format.B8G8R8X8_UNorm_SRgb;
            ToDXGIFormat[(int)Format.BC6H_Typeless] = SharpDX.DXGI.Format.BC6H_Typeless;
            ToDXGIFormat[(int)Format.BC6H_Uf16] = SharpDX.DXGI.Format.BC6H_Uf16;
            ToDXGIFormat[(int)Format.BC6H_Sf16] = SharpDX.DXGI.Format.BC6H_Sf16;
            ToDXGIFormat[(int)Format.BC7_Typeless] = SharpDX.DXGI.Format.BC7_Typeless;
            ToDXGIFormat[(int)Format.BC7_UNorm] = SharpDX.DXGI.Format.BC7_UNorm;
            ToDXGIFormat[(int)Format.BC7_UNorm_SRgb] = SharpDX.DXGI.Format.BC7_UNorm_SRgb;
            ToDXGIFormat[(int)Format.AYUV] = SharpDX.DXGI.Format.AYUV;
            ToDXGIFormat[(int)Format.Y410] = SharpDX.DXGI.Format.Y410;
            ToDXGIFormat[(int)Format.Y416] = SharpDX.DXGI.Format.Y416;
            ToDXGIFormat[(int)Format.NV12] = SharpDX.DXGI.Format.NV12;
            ToDXGIFormat[(int)Format.P010] = SharpDX.DXGI.Format.P010;
            ToDXGIFormat[(int)Format.P016] = SharpDX.DXGI.Format.P016;
            ToDXGIFormat[(int)Format.YUV420_OPAQUE] = SharpDX.DXGI.Format.Opaque420;
            ToDXGIFormat[(int)Format.YUY2] = SharpDX.DXGI.Format.YUY2;
            ToDXGIFormat[(int)Format.Y210] = SharpDX.DXGI.Format.Y210;
            ToDXGIFormat[(int)Format.Y216] = SharpDX.DXGI.Format.Y216;
            ToDXGIFormat[(int)Format.NV11] = SharpDX.DXGI.Format.NV11;
            ToDXGIFormat[(int)Format.AI44] = SharpDX.DXGI.Format.AI44;
            ToDXGIFormat[(int)Format.IA44] = SharpDX.DXGI.Format.IA44;
            ToDXGIFormat[(int)Format.P8] = SharpDX.DXGI.Format.P8;
            ToDXGIFormat[(int)Format.A8P8] = SharpDX.DXGI.Format.A8P8;
            ToDXGIFormat[(int)Format.B4G4R4A4_UNorm] = SharpDX.DXGI.Format.B4G4R4A4_UNorm;
            ToDXGIFormat[(int)Format.P208] = SharpDX.DXGI.Format.P208;
            ToDXGIFormat[(int)Format.V208] = SharpDX.DXGI.Format.V208;
            ToDXGIFormat[(int)Format.V408] = SharpDX.DXGI.Format.V408;
        }
    }
}
