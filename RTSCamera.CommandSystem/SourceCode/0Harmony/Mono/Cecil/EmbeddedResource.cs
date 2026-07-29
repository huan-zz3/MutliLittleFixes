using System;
using System.IO;

namespace Mono.Cecil
{
	// Token: 0x02000231 RID: 561
	internal sealed class EmbeddedResource : Resource
	{
		// Token: 0x17000276 RID: 630
		// (get) Token: 0x06000C09 RID: 3081 RVA: 0x0001B6C7 File Offset: 0x000198C7
		public override ResourceType ResourceType
		{
			get
			{
				return ResourceType.Embedded;
			}
		}

		// Token: 0x06000C0A RID: 3082 RVA: 0x0002A6F0 File Offset: 0x000288F0
		public EmbeddedResource(string name, ManifestResourceAttributes attributes, byte[] data)
			: base(name, attributes)
		{
			this.data = data;
		}

		// Token: 0x06000C0B RID: 3083 RVA: 0x0002A701 File Offset: 0x00028901
		public EmbeddedResource(string name, ManifestResourceAttributes attributes, Stream stream)
			: base(name, attributes)
		{
			this.stream = stream;
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x0002A712 File Offset: 0x00028912
		internal EmbeddedResource(string name, ManifestResourceAttributes attributes, uint offset, MetadataReader reader)
			: base(name, attributes)
		{
			this.offset = new uint?(offset);
			this.reader = reader;
		}

		// Token: 0x06000C0D RID: 3085 RVA: 0x0002A730 File Offset: 0x00028930
		public Stream GetResourceStream()
		{
			if (this.stream != null)
			{
				return this.stream;
			}
			if (this.data != null)
			{
				return new MemoryStream(this.data);
			}
			if (this.offset != null)
			{
				return new MemoryStream(this.reader.GetManagedResource(this.offset.Value));
			}
			throw new InvalidOperationException();
		}

		// Token: 0x06000C0E RID: 3086 RVA: 0x0002A790 File Offset: 0x00028990
		public byte[] GetResourceData()
		{
			if (this.stream != null)
			{
				return EmbeddedResource.ReadStream(this.stream);
			}
			if (this.data != null)
			{
				return this.data;
			}
			if (this.offset != null)
			{
				return this.reader.GetManagedResource(this.offset.Value);
			}
			throw new InvalidOperationException();
		}

		// Token: 0x06000C0F RID: 3087 RVA: 0x0002A7EC File Offset: 0x000289EC
		private static byte[] ReadStream(Stream stream)
		{
			int num3;
			if (stream.CanSeek)
			{
				int num = (int)stream.Length;
				byte[] array = new byte[num];
				int num2 = 0;
				while ((num3 = stream.Read(array, num2, num - num2)) > 0)
				{
					num2 += num3;
				}
				return array;
			}
			byte[] array2 = new byte[8192];
			MemoryStream memoryStream = new MemoryStream();
			while ((num3 = stream.Read(array2, 0, array2.Length)) > 0)
			{
				memoryStream.Write(array2, 0, num3);
			}
			return memoryStream.ToArray();
		}

		// Token: 0x0400039F RID: 927
		private readonly MetadataReader reader;

		// Token: 0x040003A0 RID: 928
		private uint? offset;

		// Token: 0x040003A1 RID: 929
		private byte[] data;

		// Token: 0x040003A2 RID: 930
		private Stream stream;
	}
}
