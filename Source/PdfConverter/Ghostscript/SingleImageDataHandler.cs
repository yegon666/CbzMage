﻿using CbzMage.Shared.ManagedBuffers;
using System.Collections.Concurrent;

namespace PdfConverter.Ghostscript
{
    public class SingleImageDataHandler : IImageDataHandler
    {
        private readonly BlockingCollection<ManagedBuffer> _queue = new();

        public ManagedBuffer WaitForImageDate()
        {
            var buffer = _queue.Take();

            _queue.Dispose();

            return buffer;
        }

        public void HandleImageData(ManagedBuffer image)
        {
            if (image == null)
            {
                return;
            }

            _queue.Add(image);
        }
    }
}