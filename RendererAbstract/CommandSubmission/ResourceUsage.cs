using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearSight.RendererAbstract.CommandSubmission
{
    public struct ResourceUsage
    {
        public ResourceUsage(ReadOnlyUsage usage)
        {
            ReadOnlyFlags = usage;
            ReadWriteFlag = ReadWriteUsage.None;
        }
        public ResourceUsage(ReadWriteUsage usage)
        {
            ReadOnlyFlags = ReadOnlyUsage.None;
            ReadWriteFlag = usage;
        }

        public static implicit operator ResourceUsage(ReadOnlyUsage usage)
        {
            return new ResourceUsage(usage);
        }
        public static implicit operator ResourceUsage(ReadWriteUsage usage)
        {
            return new ResourceUsage(usage);
        }

        [Flags]
        public enum ReadOnlyUsage
        {
            None = 0,
            VertexAndConstantBuffer = 0x001,
            IndexBuffer = 0x002,
            NonPixelShaderResource = 0x004,
            PixelShaderResource = 0x008,
            IndirectArgument = 0x00F,
            CopySource = 0x010,
            ResolveSource = 0x020,
            DepthRead = 0x040,
        }

        public enum ReadWriteUsage
        {
            None,
            CopyDest,
            RenderTargetColor,
            UnorderedAccess,
            DepthWrite,

            Present
        }

        /// <summary>
        /// A combination of ReadOnlyUsage flags. If anything is set, ReadWriteFlag needs to be none.
        /// </summary>
        public ReadOnlyUsage ReadOnlyFlags { get; private set; }

        /// <summary>
        /// A single read write flag. Any other than ReadWriteUsage.None requires ReadOnlyFlags to be none.
        /// </summary>
        public ReadWriteUsage ReadWriteFlag { get; private set; }

        public static bool operator ==(ResourceUsage a, ResourceUsage b)
        {
            return a.ReadOnlyFlags == b.ReadOnlyFlags && a.ReadWriteFlag == b.ReadWriteFlag;
        }
        public static bool operator !=(ResourceUsage a, ResourceUsage b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            var resourceUsage = obj as ResourceUsage?;
            return resourceUsage != null && this == resourceUsage.Value;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)ReadOnlyFlags * 397) ^ (int)ReadWriteFlag;
            }
        }
    }
}
