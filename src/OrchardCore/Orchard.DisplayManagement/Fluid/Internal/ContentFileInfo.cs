﻿using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.FileProviders;

namespace Orchard.DisplayManagement.Fluid.Internal
{
    public class ContentFileInfo : IFileInfo
    {
        private readonly byte[] _content;
        private readonly string _name;

        public ContentFileInfo(string name, string content)
        {
            _name = name;
            _content = Encoding.UTF8.GetBytes(content);
        }

        public bool Exists => true;

        public bool IsDirectory => false;

        public DateTimeOffset LastModified
        {
            get
            {
                return DateTimeOffset.MinValue;
            }
        }

        public long Length
        {
            get
            {
                return _content.Length;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string PhysicalPath => null;

        public Stream CreateReadStream()
        {
            return new MemoryStream(_content);
        }
    }
}
