using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraReader.Contracts
{
    public class Frame
    {
        public int CameraId { get; init; }

        public long FrameId { get; init; }

        public long Timestamp { get; init; }

        public int Width { get; init; }

        public int Height { get; init; }

        public byte[] Data { get; init; } = [];
    }
}
