using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ClearSight.RendererAbstract.CommandSubmission;
using SharpDX.Direct3D12;
using CommandList = ClearSight.RendererAbstract.CommandSubmission.CommandList;
using CommandListType = ClearSight.RendererAbstract.CommandSubmission.CommandListType;
using CommandListTypeDX = SharpDX.Direct3D12.CommandListType;

using DescriptorHeap = ClearSight.RendererAbstract.Binding.DescriptorHeap;
using DescriptorHeapDX = ClearSight.RendererDX12.Binding.DescriptorHeap;

using Resource = ClearSight.RendererAbstract.Memory.Resource;
using ResourceDX = ClearSight.RendererDX12.Memory.Resource;

namespace ClearSight.RendererDX12
{
    internal static class InternalUtils
    {
        static internal SharpDX.Mathematics.Interop.RawVector4 ConvVec4(Vector4 v)
        {
            return new SharpDX.Mathematics.Interop.RawVector4 {X = v.X, Y = v.Y, Z = v.Z, W = v.W};
        }
        static internal SharpDX.Mathematics.Interop.RawColor4 ConvVec4Color(Vector4 v)
        {
            return new SharpDX.Mathematics.Interop.RawColor4 { R = v.X, G = v.Y, B = v.Z, A = v.W };
        }

        static public CommandListTypeDX GetDXCommandListType(CommandListType type)
        {
            switch (type)
            {
                case CommandListType.Graphics:
                    return CommandListTypeDX.Direct;
                //      case CommandListType.Compute:
                //          return CommandListTypeDX.Compute;
                //       case CommandListType.Copy:
                //           return CommandListTypeDX.Copy;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        static public ResourceStates GetResourceStateDX(ResourceUsage usage)
        {
            ResourceStates statesOut;

            switch (usage.ReadWriteFlag)
            {
                case ResourceUsage.ReadWriteUsage.None:
                    statesOut = ResourceStates.Common;
                    break;
                case ResourceUsage.ReadWriteUsage.CopyDest:
                    statesOut = ResourceStates.CopyDestination;
                    break;
                case ResourceUsage.ReadWriteUsage.RenderTargetColor:
                    statesOut = ResourceStates.RenderTarget;
                    break;
                case ResourceUsage.ReadWriteUsage.UnorderedAccess:
                    statesOut = ResourceStates.UnorderedAccess;
                    break;
                case ResourceUsage.ReadWriteUsage.DepthWrite:
                    statesOut = ResourceStates.DepthWrite;
                    break;
                case ResourceUsage.ReadWriteUsage.Present:
                    statesOut = ResourceStates.Present;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if ((usage.ReadOnlyFlags & ResourceUsage.ReadOnlyUsage.VertexAndConstantBuffer) != 0)
                statesOut |= ResourceStates.VertexAndConstantBuffer;
            if ((usage.ReadOnlyFlags & ResourceUsage.ReadOnlyUsage.IndexBuffer) != 0)
                statesOut |= ResourceStates.IndexBuffer;
            if ((usage.ReadOnlyFlags & ResourceUsage.ReadOnlyUsage.NonPixelShaderResource) != 0)
                statesOut |= ResourceStates.NonPixelShaderResource;
            if ((usage.ReadOnlyFlags & ResourceUsage.ReadOnlyUsage.PixelShaderResource) != 0)
                statesOut |= ResourceStates.PixelShaderResource;
            if ((usage.ReadOnlyFlags & ResourceUsage.ReadOnlyUsage.IndirectArgument) != 0)
                statesOut |= ResourceStates.IndirectArgument;
            if ((usage.ReadOnlyFlags & ResourceUsage.ReadOnlyUsage.CopySource) != 0)
                statesOut |= ResourceStates.CopySource;
            if ((usage.ReadOnlyFlags & ResourceUsage.ReadOnlyUsage.ResolveSource) != 0)
                statesOut |= ResourceStates.ResolveSource;
            if ((usage.ReadOnlyFlags & ResourceUsage.ReadOnlyUsage.DepthRead) != 0)
                statesOut |= ResourceStates.DepthRead;

            return statesOut;
        }
    }
}
